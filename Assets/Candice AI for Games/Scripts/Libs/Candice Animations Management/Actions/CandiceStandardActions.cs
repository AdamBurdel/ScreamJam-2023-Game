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
    //your animations should have triggers synched with Hit Box (see Hit Box technique) for melee type movement, but it is not required.
    //class to handle standard animations
    public class CandiceStandardActions
    {        
        public Animator TemplateAnimator;

        //GENERICS
        //Use Generics in case you don't have 3D animations or if you're making a 2D game. They are used by default in both 3D and 2D.

        //Generic Idle
        public void Idle()
        {
                TemplateAnimator.SetTrigger("Idle");
        }

        //Generic Walk
        public void Walk()
        {            
                TemplateAnimator.SetTrigger("Walk");

        }

        //Generic Jump
        public void Jump()
        {            
                TemplateAnimator.SetTrigger("Jump");
        }

        //Generic Falling
        public void Falling()
        {            
                TemplateAnimator.SetTrigger("Falling");
        }

        //Generic Attack
        public void Attack()
        {            
                TemplateAnimator.SetTrigger("Attack");

        }

        //Generic Hurt
        public void Hurt()
        {           
                TemplateAnimator.SetTrigger("Hurt");

        }

        //Generic Death
        public void Death()
        {            
                TemplateAnimator.SetTrigger("Death");

        }

        //SPECIFICS
        //Use Specifics when you have all your animations, your VFX and SFX, and timing done. Specifics are 3D and VR only.

        //WALKING
        //Sepcific walk forwards
        public void WalkForwards() {            
                TemplateAnimator.SetTrigger("WalkForwards");
                        
        }

        //Specific walk forwards and left
        public void WalkForwardsLeft()
        {            
                TemplateAnimator.SetTrigger("WalkForwardsLeft");
        }

        //Specific walk leftwards
        public void StrafeLeft() {            
                TemplateAnimator.SetTrigger("StrafeLeft");
        }

        //Specific walk forwards and right
        public void WalkForwardsRight()
        {            
                TemplateAnimator.SetTrigger("WalkForwardsRight");
        }

        //Specific walk rightwards
        public void StrafeRight() {
            
                TemplateAnimator.SetTrigger("StrafeRight");
        }

        //Specific walk backwards
        public void WalkBackwards()
        {
            
                TemplateAnimator.SetTrigger("WalkBackwards");
        }

        //Specific walk backwards and left
        public void WalkBackwardsLeft()
        {
         
                TemplateAnimator.SetTrigger("WalkBackwardsLeft");
        }

        //Specific walk backwards and right
        public void WalkBackwardsRight()
        {
            
                TemplateAnimator.SetTrigger("WalkBackwardsRight");
        }

        //RUNNING
        //Specific run forwards
        public void RunForwards()
        {
         
                TemplateAnimator.SetTrigger("RunForwards");
        }

        //Specific run forwards and left
        public void RunForwardsLeft()
        {
         
                TemplateAnimator.SetTrigger("RunForwardsLeft");
        }

        //Specific run forwards and right
        public void RunForwardsRight()
        {
         
                TemplateAnimator.SetTrigger("RunForwardsRight");
        }

        //JUMPING
        //Specific Jump Forwards
        public void JumpForwards()
        {
            
                TemplateAnimator.SetTrigger("JumpForwards");
        }

        public Type GetType()
        {

            return typeof(CandiceStandardActions);

        }

    }
}