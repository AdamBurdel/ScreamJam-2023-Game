//System
using System;
using System.Collections;
using System.Collections.Generic;
//Unity
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Animations;
//Candice AI
using CandiceAIforGames.AI;

namespace CandiceAIforGames.AI
{
    public class CandiceInventoryManager : DeriveMono
    {

        [HideInInspector]
        public GameObject drop;        

        // Update is called once per frame
        void Update()
        {

        }

        public void Drop(Transform t) {
            if (drop != null) {
                Transform indicator = drop.transform.Find("Indicator P2");
                if (indicator != null) {
                    GameObject screenRenderer = GameObject.Find("Renderer P2");
                    if (screenRenderer != null) {
                        /*indicator.gameObject.GetComponent<LincolnCpp.HUDIndicator.IndicatorOnScreen>().renderers[0] = screenRenderer.GetComponent<LincolnCpp.HUDIndicator.IndicatorRenderer>();
                        indicator.gameObject.GetComponent<LincolnCpp.HUDIndicator.IndicatorOffScreen>().renderers[0] = screenRenderer.GetComponent<LincolnCpp.HUDIndicator.IndicatorRenderer>();*/
                    }
                }                
                Instantiate(drop, new Vector3(t.position.x, t.position.y+1f, t.position.z+2f), Quaternion.identity);
            }            
        }
    }
}