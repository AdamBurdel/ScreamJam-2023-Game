//System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
//Unity
using UnityEngine;
using UnityEngine.UI;
//Candice AI
using CandiceAIforGames.AI;

namespace CandiceAIforGames.AI
{    
    //class to create animations actions delegates
    public class CandiceCreateAnimationActions
    {

        //Cumulative list of actions being returned        
        public List<Action> AnimationActions;

        //Create an actions list from provided action
        public void AddAction (Action action) {
            //create a new action
            List<Action> newAction = new List<Action>();
            //add new action
            newAction.Add(action);
            //add to cumulative actions list
            AnimationActions.AddRange(newAction);

        }

        //Return cumulative actions list
        public List<Action> ReturnActions() {

            return AnimationActions;
        
        }

        //Clear cumulative actions list
        public void ClearActions() {
            AnimationActions.Clear();
        }

        //RETURN ALL ACTIONS
        public MethodInfo[] GetAllActions(Type type)
        {
            MethodInfo[] theseActions = type.GetMethods();
            return theseActions;
        }

        public string GetMethodName(MethodInfo info)
        {
            return info.Name;
        }
    }
}