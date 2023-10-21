//System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
//Unity
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Animations;
//Candice AI
using CandiceAIforGames.AI;

namespace CandiceAIforGames.AI
{
    //class to handle input to animation stage, check the existence of required triggers
    public class CandiceModuleAnimations : CandiceCreateAnimationActions
    {   
        public Animator TemplateAnimator;
        public AnimatorController controller;
        public AnimatorControllerParameter[] Triggers;
        public CandiceStandardActions StandardActions;
        public CandiceHumanoidMelee HumanoidMeleeActions;
        public CandicePlayerOverrides PlayerOverrides;

        //check Model (rig) type if any
        public ModelImporterAnimationType GetRigType(ModelImporter importer) {
            ModelImporterAnimationType thisType = importer.animationType;
            return thisType;
        }

        //new controller
        public AnimatorController AnimationController(ModelImporter importer) {

            //bring in some Animation Actions
            //CandiceCreateAnimationActions CreateActions = new CandiceCreateAnimationActions();
            CandiceStandardActions StandardActions = new CandiceStandardActions();
            CandiceHumanoidMelee HumanoidMeleeActions = new CandiceHumanoidMelee();
            CandicePlayerOverrides PlayerOverrides = new CandicePlayerOverrides();

            //assign Animator to Actions
            StandardActions.TemplateAnimator = TemplateAnimator;
            HumanoidMeleeActions.TemplateAnimator = TemplateAnimator;
            controller = TemplateAnimator.runtimeAnimatorController as AnimatorController;

            //check if controller exists
            if (controller == null) {
                //supply new controller
                controller = new AnimatorController();
            }

            //bring in the standard set of animation actions (2D,3D with VR upcoming)
            foreach (MethodInfo info in GetAllActions(StandardActions.GetType())) {
                Action action = info.CreateDelegate(info.GetType()) as Action;
                AddAction(action);
            }

            //Action Support based on model (rig)
            //check if rig is attached & add some actions
            if (importer != null) {
                //next check animation model (rig) type
                //switch (GetRigType(importer)) {
                //    case ModelImporterAnimationType.None:
                //        //continue, if no animation type is specified means no model is supplied with the agent
                //        break;
                //    case ModelImporterAnimationType.Legacy:
                //        //add Legacy melee animation actions
                //        //add Legacy ranged animation actions
                //        break;
                //    case ModelImporterAnimationType.Generic:
                //        //add Generic melee animation actions
                //        //add Generic ranged animation actions                        
                //        break;
                //    case ModelImporterAnimationType.Human:
                        //add humanoid melee animation actions
                        foreach (MethodInfo info in GetAllActions(HumanoidMeleeActions.GetType())) {
                            Action action = info.CreateDelegate(info.GetType()) as Action;
                            AddAction(action);
                        };
                        ////add humanoid ranged animation actions
                        //break;

                //}
            }

            //for each trigger in the provided controller
            AnimatorControllerParameter[] triggerParams = GetTriggers();
            if (triggerParams != null) {
                foreach (AnimatorControllerParameter parameter in triggerParams)
                {
                    //if trigger does not exist
                    if (!CheckTrigger(parameter))
                    {
                        //add trigger
                        AddParameter(controller, parameter.name, AnimatorControllerParameterType.Trigger);
                    }
                }
            }

            //return the controller
            return controller;
        }

        //get controller triggers
        public AnimatorControllerParameter[] GetTriggers() {
            if (TemplateAnimator != null) {
                Triggers = TemplateAnimator.parameters;
            }
            return Triggers;
        }

        //check if required triggers are present in controller
        public bool CheckTrigger(AnimatorControllerParameter parameter) {
            bool isTrigger = false;
            //check against master animations actions list
            foreach (Action action in AnimationActions) {
                //if match found
                if (parameter.name == action.Method.Name)
                {
                    isTrigger = true;
                }                
            }
            return isTrigger;
        }

        //modify a trigger
        public void ModifyParameter(int parameterIndex, string newName)
        {            
            AnimatorControllerParameter[] parameters = GetTriggers();
            parameters[parameterIndex].name = newName;
            //TemplateAnimator.parameters = parameters;
        }

        //add a trigger to the specified controller
        public void AddParameter(AnimatorController controller, string newName, AnimatorControllerParameterType type) {

            AnimatorControllerParameter param = new AnimatorControllerParameter();
            param.name = newName;
            param.type = type;
            controller.AddParameter(param);

        }

    }
}