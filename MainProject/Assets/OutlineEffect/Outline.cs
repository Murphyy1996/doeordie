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
        private OutlineEffect mainCameraEffectScript;
        [SerializeField]
        private LayerMask layerMask;
        private bool outlineEnabled = true;
        private int errorCounter = 0;

        private void Awake()
        {
            Renderer = GetComponent<Renderer>();
            if (hideBehindObjects == true)
            {
                InvokeRepeating("RenderTick", 0, 0.2f);
            }
        }

        private void OnEnable()
        {
            try
            {
                Camera.main.GetComponent<OutlineEffect>().AddOutline(this);
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
            }
            catch
            {
                errorCounter++;
            }
        }

        //Stop them being visible through walls

        private void RenderTick()
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
                    }
                    else
                    {
                        eraseRenderer = true;
                    }
                }
            }
        }

        private bool CheckIfPlayerCanBeSeen() //Will raycast and return if this obj is visible
        {
            Vector3 direction = mainCameraEffectScript.transform.position - transform.position;
            RaycastHit raycastHit;
            if (Physics.Raycast(transform.position, direction, out raycastHit, 20f, layerMask))
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