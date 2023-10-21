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
    //trigger based animations system
    //your animations should have triggers synched with Hit Box (see Hit Box technique) for melee type attack animations, but it is not required. 
    //We provide a HitBox Component that is enabled by default with a small box collider attached to a parent melee weapon type object
    //class that handles humanoid melee animations
    public class CandiceHumanoidMelee
    {
        public Animator TemplateAnimator;

        //GENERICS
        //Use Generics in case you don't really have 3D animations or if you're making a 2D game. They are used by default in both 3D and 2D.

        //Generic Ground smash aka ground slam
        public void GroundSmash()
        {
            
                TemplateAnimator.SetTrigger("GroundSmash");
        }

        //Generic hook type punch
        public void Hook()
        {
            
                TemplateAnimator.SetTrigger("Hook");
        }

        //Generic haymaker type punch
        public void HeavyPunch()
        {
            
                TemplateAnimator.SetTrigger("HeavyPunch");
        }

        //Generic left jab type punch
        public void LeftJab()
        {
            
                TemplateAnimator.SetTrigger("LeftJab");
        }

        //Generic right jab type punch
        public void RightJab()
        {
            
                TemplateAnimator.SetTrigger("RightJab");
        }

        //Generic backhand type punch
        public void Backhand()
        {
            
                TemplateAnimator.SetTrigger("Backhand");
        }

        //Generic Uppercut type punch
        public void Uppercut()
        {
            
                TemplateAnimator.SetTrigger("Uppercut");
        }

        //Generic Spinning type kick
        public void SpinningKick()
        {
            
                TemplateAnimator.SetTrigger("SpinningKick");
        }

        //Generic Throw
        public void Throw()
        {
            
                TemplateAnimator.SetTrigger("Throw");
        }

        //Generic front type kick
        public void FrontKick()
        {
            
                TemplateAnimator.SetTrigger("FrontKick");
        }

        //Generic high type kick
        public void HighKick()
        {
            
                TemplateAnimator.SetTrigger("HighKick");
        }

        //SPECIFICS
        //Use Specifics when you have all your animations, your VFX and SFX, and timing done. Specifics are 3D and VR only.

        //NO SPECIFICS

        //COMBO CONTROL (timing should be in fractions of a second for smooth movement)
        //LightHandCombo
        public IEnumerator GenericCombo(int attackNumber, float timing)
        {
            yield return new WaitForSeconds(timing);
            switch (attackNumber) {

                case 0:
                    LeftJab();
                    break;
                case 1:
                    RightJab();
                    break;
                case 2:
                    Hook();
                    break;
                case 3:
                    HighKick();
                    break;

            }

        }

        //return this type
        public Type GetType() {

            return typeof(CandiceHumanoidMelee);
                
        }
    }
}