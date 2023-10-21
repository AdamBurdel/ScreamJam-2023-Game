//System
using System;
using System.Collections;
using System.Collections.Generic;
//Unity
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;
//Candice AI
using CandiceAIforGames.AI;

public class Possessor : MonoBehaviour
{


    [CustomEditor(typeof(CandiceAIController))]
    public GameObject[] PossessableObjects; //must have an instance of CandiceAIController script attached
    public bool PlayerCanPossessProjectile = false;
    public float ProjectilePossessionTimer = 10f;
    private GameObject[] projectiles;
    private GameObject[] tempProjectile;
    [HideInInspector]
    public CandiceAnimationManager animationManager;
    private Transform vsfx;       
    

    // Start is called before the first frame update
    void Start()
    {
        if (animationManager == null) {
            animationManager = gameObject.AddComponent(typeof(CandiceAnimationManager)) as CandiceAnimationManager;
        }
        vsfx = transform.Find("VSFX");
        foreach (Transform fx in vsfx) {
            if (fx.gameObject.tag == "CandiceVSFX") {
                fx.gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (animationManager.EvaluateInput("Possess", false, true, false)) {
            //if can possess nearest projectile
            if (PlayerCanPossessProjectile) {
                projectiles = GameObject.FindGameObjectsWithTag("Projectile");
                if (projectiles != null && PossessableObjects.Length < 3)
                {
                    Array.Resize<GameObject>(ref projectiles, 1);
                    Array.Resize<GameObject>(ref PossessableObjects, PossessableObjects.Length + 1);
                    projectiles.CopyTo(PossessableObjects, PossessableObjects.Length - 1);

                }
            }
            //now cycle through all assigned possessed
            foreach (GameObject possessed in PossessableObjects) {

                if (possessed != null)
                {
                    if (!possessed.activeSelf)
                    {
                        //in case you are not active, set yourself active                        
                        if (Array.IndexOf(PossessableObjects, possessed)  <= PossessableObjects.Length-1)
                        {
                            possessed.SetActive(true);
                        }
                        else {
                            CandiceAIPlayerController controller = possessed.GetComponent<CandiceAIPlayerController>();
                            if (controller != null)
                            {
                                if (!controller.enabled)
                                {
                                    controller.enabled = true;
                                }
                            }
                        }
                        Transform camParent = possessed.transform.Find("CameraParent");                        
                        if (camParent != null)
                        {
                            if (!camParent.gameObject.activeSelf)
                            {
                                camParent.gameObject.SetActive(true);

                            }
                        }
                        if (vsfx == null)
                        {
                            vsfx = possessed.transform.Find("VSFX").transform;
                        }
                        foreach (Transform fx in vsfx)
                        {
                            if (fx.gameObject.tag == "CandiceVSFX")
                            {
                                GameObject thisfx = Instantiate(fx.gameObject, possessed.transform.position, Quaternion.identity);
                                thisfx.SetActive(true);
                                Destroy(thisfx, 5f);
                            }
                        }

                    }
                    else
                    {
                        //if you are already active
                        possessed.SetActive(false);
                        if (possessed.tag == "Projectile")
                        {
                            Destroy(possessed, ProjectilePossessionTimer);
                        }
                    }
                }
                else {
                    //ensure any projectile possessables have been discarded
                    Array.Resize<GameObject>(ref PossessableObjects, PossessableObjects.Length - 1);
                }


            }

            //quick ui check
            //remove later
            GameObject rendp1 = GameObject.Find("Renderer P1");
            GameObject rendp2 = GameObject.Find("Renderer P2");
            if (rendp1 != null && rendp2 != null)
            {
                /*rendp1.GetComponent<LincolnCpp.HUDIndicator.IndicatorRenderer>().camera = Camera.main;
                rendp2.GetComponent<LincolnCpp.HUDIndicator.IndicatorRenderer>().camera = Camera.main;*/        
            }

        }
    }
}
