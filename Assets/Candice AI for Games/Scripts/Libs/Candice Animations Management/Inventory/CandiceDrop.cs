using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CandiceAIforGames.AI;

namespace CandiceAIforGames.AI
{

    #region ENUMS
    public enum DropType
    {
        Weapon,
        Tool,
        Resource,
        Health,
        Invincibility,
        Speed, 
        Damage
    }
    #endregion

    public class CandiceDrop : MonoBehaviour
    {

        public DropType dropType = new DropType();        
        public DropType dType { get => dropType; set => dropType = value; }
        public float boostValue = 0.0f;
        public float boostDuration = 10f;

        //
        [HideInInspector]
        public GameObject agentToBoost;
        private float agentHealth;
        private float agentDamage;
        private float agentSpeed;

        //UI
        private CandiceUI candiceUI;

        //VSFX
        private Transform VSFX;

        void Start() {
            candiceUI = new CandiceUI();
        }

        // Start is called before the first frame update
        public void AssessDrop()
        {            
            candiceUI = new CandiceUI();
            candiceUI.thisAgent = agentToBoost;
            candiceUI.HealthBar = GameObject.FindWithTag("PlayerHealthBar");            

            agentHealth = agentToBoost.GetComponent<CandiceAIController>().hitPoints;
            agentDamage = agentToBoost.GetComponent<CandiceAIController>().attackDamage;
            agentSpeed = agentToBoost.GetComponent<CandiceAIController>().moveSpeed;
            switch (dType)
            {
                case DropType.Weapon:
                    //no Weapon inventory support yet
                    break;
                case DropType.Tool:
                    //no Tool inventory support yet
                    break;
                case DropType.Resource:
                    //no Resource inventory support yet
                    break;
                case DropType.Health:
                    //boost is permanent and is agent type independent (which means your enemies can snatch up the drops), it's only fair.
                    agentToBoost.GetComponent<CandiceAIController>().hitPoints += boostValue;
                    Debug.Log(agentToBoost.name + " boosted with health of " + boostValue);
                    candiceUI.UpdateHealthUI("ClassicProgressBar", -boostValue);
                    break;
                case DropType.Invincibility:
                    //invincibility is timed and is only applied to the player agent (we don't want enemy or npc agents to get this boost). Here, there is no fairness.
                    if (agentToBoost.tag == "Player")
                    {
                        agentToBoost.GetComponent<CandiceAIController>().hitPoints += boostValue;
                        candiceUI.UpdateHealthUI("ClassicProgressBar", -boostValue);                        
                        StartCoroutine(revertToNormal("Player Health", agentToBoost, boostDuration));
                        Debug.Log(agentToBoost.name + " boosted with invincibility for " + boostDuration.ToString());
                    }
                    else {
                        Debug.Log(agentToBoost.name + " Cannot receive the invincibility boost. It is an Enemy or NPC type agent.");
                    }
                    break;
                case DropType.Speed:
                    if (agentToBoost.tag == "Player")
                    {
                        agentSpeed = agentToBoost.GetComponent<CandiceAIPlayerController>().speed;
                        agentToBoost.GetComponent<CandiceAIPlayerController>().speed += boostValue;
                        StartCoroutine(revertToNormal("Player Speed", agentToBoost, boostDuration));                        
                    }
                    else {
                        agentToBoost.GetComponent<CandiceAIController>().moveSpeed += boostValue;
                        StartCoroutine(revertToNormal("Speed", agentToBoost, boostDuration));
                    }
                    Debug.Log(agentToBoost.name + " boosted with movement speed of " + agentToBoost.GetComponent<CandiceAIController>().moveSpeed.ToString() + " for " + boostDuration.ToString());
                    break;
                case DropType.Damage:

                    break;
            }

            VSFX = transform.Find("VSFX");
            if (VSFX != null)
            {
                foreach (Transform fx in VSFX)
                {
                    GameObject fxClone = Instantiate(fx.gameObject, agentToBoost.transform.position, Quaternion.identity);
                    Destroy(fxClone, 5f);
                }
            }

            Destroy(transform.gameObject, 1f);

        }

        // Update is called once per frame
        void Update()
        {
        }

        public IEnumerator revertToNormal(string what, GameObject agent, float waitTime) {

            yield return new WaitForSeconds(waitTime);
            if (what == "Weapon")
            {

            }
            else if (what == "Health")
            {
                //in case we don't want to make the Health boost permanent (on any agentType or specific agent types)
            }
            else if (what == "Speed") {
                agent.GetComponent<CandiceAIController>().moveSpeed = agentSpeed;
            }
            else if (what == "Player Health")
            {
                agent.GetComponent<CandiceAIController>().hitPoints = agentHealth;
                candiceUI.UpdateHealthUI("ClassicProgressBar", agentHealth);
            }
            else if (what == "Player Speed")
            {
                agent.GetComponent<CandiceAIPlayerController>().speed = agentSpeed;
            }


        }

        void OnTriggerEnter(Collider col) {

            agentToBoost = col.gameObject;
            if (agentToBoost != null && agentToBoost.tag != "Projectile")
            {                
                AssessDrop();
            }            
        }

    }
}