using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CandiceAIforGames.AI
{
    [Serializable]
    public class CandiceGOAPActionS : ScriptableObject
    {
        public new string name;                                 // Name of the action
        public int cost;                                     // Cost of the action
        public List<CandiceKeyValuePair<string, int>> preconditions;        // Dictionary of preconditions of the action
        public List<CandiceKeyValuePair<string, int>> effects;              // Dictionary of effects of the action
        public CandiceBehaviorTreeS behaviorTree;             // Behavior tree that will be evaluated to execute the action
        public bool isComplete = false;

        public CandiceGOAPActionS(string name, int cost, List<CandiceKeyValuePair<string, int>> preconditions, List<CandiceKeyValuePair<string, int>> effects, CandiceBehaviorTreeS behaviorTree, bool isComplete)
        {
            this.name = name;
            this.cost = cost;
            this.preconditions = preconditions;
            this.effects = effects;
            this.behaviorTree = behaviorTree;
            this.isComplete = isComplete;
        }
        public CandiceGOAPAction ConvertToGOAPAction(CandiceAIController agent)
        {
            Dictionary<string, int> _preconditions = new Dictionary<string, int>();
            Dictionary<string, int> _effects = new Dictionary<string, int>();

            foreach (CandiceKeyValuePair<string, int> item in preconditions)
            {
                if (!_preconditions.ContainsKey(item.key))
                {
                    _preconditions.Add(item.key, item.value);
                }

            }
            foreach (CandiceKeyValuePair<string, int> item in effects)
            {
                if (!_effects.ContainsKey(item.key))
                {
                    _effects.Add(item.key, item.value);
                }
            }
            CandiceGOAPAction action = new CandiceGOAPAction(agent, name, cost, _preconditions, _effects, behaviorTree);
            Debug.Log("Setting behavior tree: " + behaviorTree.name);

            return action;
        }


    }
}
