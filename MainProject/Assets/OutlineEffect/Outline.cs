/*
//  Copyright (c) 2015 José Guerreiro. All rights reserved.
//
//  MIT license, see http://www.opensource.org/licenses/mit-license.php
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
*/
//Edits for hiding behind other objects made by James Murphy

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace cakeslice
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Renderer))]
    public class Outline : MonoBehaviour
    {
        public Renderer Renderer { get; private set; }

        public int color;
        public bool eraseRenderer;
        [SerializeField]
        [Header("Experimental")]
        private bool hideBehindObjects = false;
        [SerializeField]
        private float pickUpOutlineRange = 50;
        private OutlineEffect mainCameraEffectScript;
        [SerializeField]
        private LayerMask layerMask;
        private bool outlineAllowed = true;
        private int errorCounter = 0;
        private GameObject emptyRepresentationPoint;
        private Transform playerTransform;
        private RaycastHit raycastHit;

        private void Awake()
        {
            Renderer = GetComponent<Renderer>();
            if (hideBehindObjects == true)
            {
                //Set up the empty representation point
                emptyRepresentationPoint = new GameObject();
                emptyRepresentationPoint.name = this.gameObject.name + " transform empty";
                emptyRepresentationPoint.transform.SetParent(transform);
                emptyRepresentationPoint.transform.localPosition = new Vector3(0, 0, 0);
                emptyRepresentationPoint.transform.SetParent(null);
                GameObject foundEmpty = GameObject.Find("OutlineEmpty");
                if (foundEmpty == null)
                {
                    foundEmpty = new GameObject();
                    foundEmpty.name = "OutlineEmpty";
                    emptyRepresentationPoint.transform.SetParent(foundEmpty.transform);
                }
                else
                {
                    emptyRepresentationPoint.transform.SetParent(foundEmpty.transform);
                }
                playerTransform = GameObject.Find("Player").transform;
                //Run the render tick
                InvokeRepeating("RenderTick", 0, 0.2f);
            }
        }

        private void OnEnable()
        {
            try
            {
                Camera.main.GetComponent<OutlineEffect>().AddOutline(this);
                outlineAllowed = true;
            }
            catch
            {
                errorCounter++;
            }
        }

        private void OnDisable()
        {
            try
            {
                Camera.main.GetComponent<OutlineEffect>().RemoveOutline(this);
                outlineAllowed = false;
            }
            catch
            {
                errorCounter++;
            }
        }

        //Stop them being visible through walls

        private void RenderTick()
        {
            if (outlineAllowed == true)
            {
                //Get the main camera script if possible
                if (mainCameraEffectScript == null)
                {
                    try
                    {
                        if (Camera.main != null)
                        {
                            mainCameraEffectScript = Camera.main.GetComponent<OutlineEffect>();
                            layerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("No Teleport"));
                        }
                    }
                    catch
                    {
                        errorCounter++;
                    }
                }
                //This will hide the outline behind over objects
                if (mainCameraEffectScript != null)
                {
                    if (hideBehindObjects == true)
                    {
                        //If the renderer can be seen then enable the outline
                        if (CheckIfPlayerCanBeSeen() == true)
                        {
                            eraseRenderer = false;
                            mainCameraEffectScript.AddOutline(this);
                        }
                        else
                        {
                            eraseRenderer = true;
                            mainCameraEffectScript.RemoveOutline(this);
                        }
                    }
                }
            }
        }

        private void FixedUpdate() //Always look at the player
        {
            if (hideBehindObjects == true)
            {
                if (emptyRepresentationPoint != null && playerTransform != null)
                {
                    emptyRepresentationPoint.transform.LookAt(playerTransform.position);
                }
                //If the renderer is turned off then disabled the outline
                if (Renderer.enabled == true)
                {
                    outlineAllowed = true;
                }
                else
                {
                    outlineAllowed = false;
                }
            }
        }

        private bool CheckIfPlayerCanBeSeen() //Will raycast and return if this obj is visible
        {
            if (Physics.Raycast(emptyRepresentationPoint.transform.position, emptyRepresentationPoint.transform.forward, out raycastHit, pickUpOutlineRange, layerMask))
            {
                if (raycastHit.collider.gameObject.tag == "Player")
                {
                    return true;
                }
            }
            return false;
        }
    }
}