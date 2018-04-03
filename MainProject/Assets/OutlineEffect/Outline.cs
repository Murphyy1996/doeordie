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

        private LayerMask layerMask;
        private bool outlineEnabled = true;

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
            IEnumerable<OutlineEffect> effects = Camera.allCameras.AsEnumerable()
                           .Select(c => c.GetComponent<OutlineEffect>())
                           .Where(e => e != null);

            foreach (OutlineEffect effect in effects)
            {
                effect.AddOutline(this);
            }
        }

        private void OnDisable()
        {
            IEnumerable<OutlineEffect> effects = Camera.allCameras.AsEnumerable()
                            .Select(c => c.GetComponent<OutlineEffect>())
                            .Where(e => e != null);

            foreach (OutlineEffect effect in effects)
            {
                effect.RemoveOutline(this);
            }
        }

        //Stop them being visible through walls

        private void RenderTick()
        {
            //Get the main camera script if possible
            if (mainCameraEffectScript == null)
            {
                if (Camera.main != null)
                {
                    mainCameraEffectScript = Camera.main.GetComponent<OutlineEffect>();
                    layerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("No Teleport"));
                }
            }

            //This will hide the outline behind over objects
            if (mainCameraEffectScript != null)
            {
                if (hideBehindObjects == true)
                {
                    //If the renderer can be seen then enable the outline
                    if (CheckIfThisObjIsVisible() == true)
                    {
                        if (outlineEnabled == false)
                        {
                            eraseRenderer = false;
                            outlineEnabled = true;
                        }
                    }
                    else
                    {
                        //if the renderer can't be seen don't enable the outline
                        if (outlineEnabled == true)
                        {
                            eraseRenderer = true;
                            outlineEnabled = false;
                        }
                    }
                }
            }
        }

        private bool CheckIfThisObjIsVisible() //Will raycast and return if this obj is visible
        {
            Vector3 direction = mainCameraEffectScript.transform.position - transform.position;
            RaycastHit raycastHit;
            if (Physics.Raycast(transform.position, direction, out raycastHit, 100f, layerMask))
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