//System
using System;
using System.Collections;
using System.Collections.Generic;
//Unity
using UnityEngine;
using UnityEngine.UI;
//Candice AI
using CandiceAIforGames.AI;

namespace CandiceAIforGames.AI
{
    public class CandiceUI : CandiceMiddleware
    {
        //UI
        public GameObject HealthBar;
        public GameObject thisAgent;

        public void UpdateHealthUI(string dependencyName, float attackDamage)
        {            

            //get health bar dependecy middleware
            //Type dependency = GetDependency(dependencyName);

            if (HealthBar.GetComponent<CandiceHealthBar>() != null)
            {
                //get health indicator (progress bar) value
                var hlth = HealthBar.GetComponent<CandiceHealthBar>();
                if (hlth != null)
                {
                    hlth.m_FillAmount -= (hlth.m_FillAmount / thisAgent.GetComponent<CandiceAIController>().hitPoints * attackDamage);
                }
            }
            else {

                Debug.Log("You're missing the " + dependencyName + " prefab. Check The UI folder under prefabs.");
            }


        }

        public void ResetBar(GameObject healthBar, float hitPoints)
        {
            //get health indicator (progress bar) value
            var hlth = healthBar.GetComponent<CandiceHealthBar>();
            if (hlth != null)
            {
                hlth.m_FillAmount = hitPoints / hitPoints;
            }
        }

    }
}