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

    //class to handle player animation overrides that interact with CandiceAI system    
    public class CandicePlayerOverrides
    {
        GameObject AttackTarget = new GameObject();
        //original scale of the root parent gameObject should always be 1
        public Vector3 originalScale = new Vector3(1f, 1f, 1f);
        public float boostedScale = 2f;
        public static float bufferMousePositionz = 10f;
        //projectile speeds should be in the 00s and 000s and should be maintained in these ranges as these values are optimal values tested against various projectile types
        public float projectileMoveSpeed = 3000f;
        public float projectileBoostedMoveSpeed = 5000f;

        //PATRAN_CANDICEAI IS SHORT FOR (PLAYER ATTACK RANGED USING CANDICEAI)
        //this uses the candice ai projectile to performed a ranged attack. It requires that CandiceAIController script be also attached to Player.
        public void PATRAN_CANDICEAI(CandiceAIController player)
        {
            //get mouse position from input
            Vector3 mousePos = Input.mousePosition;
            GameObject AttackTarget = new GameObject();
            if (mousePos != null)
            {
                //buffer mouse position z
                mousePos.z = bufferMousePositionz;
                //create new attack target from reticule position
                AttackTarget.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
            }

            if (player != null)
            {
                //adjust projectile scale (scale is always 1 on normal projectile)
                player.projectile.gameObject.transform.localScale = OriginalScale(player);

                //adjust projectile move speed
                player.projectile.gameObject.GetComponent<CandiceProjectile>().moveSpeed = projectileMoveSpeed;

                //assign newly created attack target to player
                player.AttackTarget = AttackTarget;

                //perform ranged attack using CandiceAIController (player must have a CandiceAIController script attached wtih a projectile prefab attached at a minimum for this to work)
                player.AttackRanged();
            }
        }

        //PATRAN_BOOSTED_CANDICEAI IS SHORT FOR (PLAYER BOOSTED ATTACK RANGED USING CANDICEAI)
        //this uses the candice ai projectile to performed a ranged attack. It requires that CandiceAIController script be also attached to Player.
        public void PATRAN_BOOSTED_CANDICEAI(CandiceAIController player)
        {
            //get mouse position from input
            Vector3 mousePos = Input.mousePosition;
            GameObject AttackTarget = new GameObject();
            if (mousePos != null) {
                //buffer mouse position z
                mousePos.z = bufferMousePositionz;
                //create new attack target from reticule position
                if (Camera.main != null) {
                    AttackTarget.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
                }
                
            }

            if (player != null) {
                //adjust projectile scale
                player.projectile.gameObject.transform.localScale = new Vector3(boostedScale, boostedScale, boostedScale);

                //adjust projectile move speed
                player.projectile.gameObject.GetComponent<CandiceProjectile>().moveSpeed = projectileBoostedMoveSpeed;

                //assign newly created attack target to player
                player.AttackTarget = AttackTarget;

                //perform ranged attack using CandiceAIController (player must have a CandiceAIController script attached wtih a projectile prefab attached at a minimum for this to work)
                player.AttackRanged();
            }
        }

        //returns original projectile scale
        public Vector3 OriginalScale(CandiceAIController original) {
            originalScale = original.projectile.gameObject.transform.localScale;
            return originalScale;
        }
    }
}