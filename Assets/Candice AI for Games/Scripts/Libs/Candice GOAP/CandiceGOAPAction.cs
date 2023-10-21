using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CandiceAIforGames.AI
{
    
    public class CandiceGOAPAction
    {
        public string name;                                 // Name of the action
        public int cost;                                     // Cost of the action
        public Dictionary<string, int> preconditions;        // Dictionary of preconditions of the action
        public Dictionary<string, int> effects;              // Dictionary of effects of the action
        public CandiceAIController aiController;             // AI Controller that will execute the action
        public CandiceBehaviorTreeS behaviorTreeS;             // Behavior tree that will be evaluated to execute the action
        private CandiceBehaviorTree behaviorTree;             // Behavior tree that will be evaluated to execute the action
        public bool isComplete = false;

        /// <summary>
        /// Constructor for the CandiceGOAPAction class
        /// </summary>
        /// <param name="aiController">AI Controller that will execute the action</param>
        /// <param name="name">Name of the action</param>
        /// <param name="cost">Cost of the action</param>
        /// <param name="preconditions">Dictionary of preconditions of the action</param>
        /// <param name="effects">Dictionary of effects of the action</param>
        public CandiceGOAPAction(CandiceAIController aiController, string name, int cost, Dictionary<string, int> preconditions, Dictionary<string, int> effects, CandiceBehaviorTreeS _behaviorTreeS)
        {
            this.aiController = aiController;
            this.name = name;
            this.cost = cost;
            this.preconditions = preconditions;
            this.effects = effects;
            // Initialize the behavior tree if it exists
            if (_behaviorTreeS != null)
            {
                behaviorTreeS = _behaviorTreeS;
                Debug.Log("Behavior Tree Not null. Name: " + behaviorTreeS.name);
                behaviorTree = new CandiceBehaviorTree();
                behaviorTree.nodes = behaviorTreeS.GetNodes();
                behaviorTree.Initialise();
                behaviorTree.CreateBehaviorTree(aiController, onBTComplete);
            }
            else
            {
                Debug.Log("Behavior Tree is Null");
            }
            
        }

        /// <summary>
        /// Callback function for the completion of behavior tree evaluation
        /// </summary>
        /// <param name="btEvent">Behavior Tree event object that contains the state of evaluation</param>
        public void onBTComplete(CandiceBehaviorTreeEventData btEvent)
        {
            if(btEvent.behaviorState == CandiceBehaviorStates.SUCCESS)
            {
                // The behavior tree completed successfully, handle accordingly.
                // This function will contain necessary logic that should execute if the behavior tree completes successfully.
                // E.g., move to the next action in the plan, set the action to completed, and so on.

                // If the behavior tree evaluation result is SUCCESS, apply each effect of the action to the state.
                if (btEvent.behaviorState == CandiceBehaviorStates.SUCCESS)
                {
                    foreach (KeyValuePair<string, int> effect in effects)
                    {
                        // If the state already contains the effect key, add the effect value to the existing value.
                        if (aiController.agentState.state.ContainsKey(effect.Key))
                        {
                            aiController.agentState.state[effect.Key] += effect.Value;
                        }
                        // If the state does not contain the effect key, add the effect key-value pair to the state.
                        else
                        {
                            aiController.agentState.state[effect.Key] = effect.Value;
                        }
                    }
                    isComplete = true;
                }

            }
            else if (btEvent.behaviorState == CandiceBehaviorStates.FAILURE)
            {
                // The behavior tree failed to complete, handle accordingly.
                // This function will contain necessary logic that should execute if the behavior tree fails to complete.
                // E.g., re-plan, cancel the plan, and so on.

            }
            else if (btEvent.behaviorState == CandiceBehaviorStates.RUNNING)
            {
                // The behavior tree is still running, handle accordingly.
                // This function will contain necessary logic that should execute if the behavior tree is still running.
                // E.g., update the plan, cancel the plan, and so on.

            }
        }

        /// <summary>
        /// Checks if the current state of the agent satisfies the preconditions of the action
        /// </summary>
        /// <param name="state">Current state of the agent</param>
        /// <returns>Boolean indicating whether the action is achievable in the current state or not</returns>
        public bool IsAchievable(Dictionary<string, int> state)
        {
            // Check each precondition of the action and ensure it is satisfied in the state
            foreach (KeyValuePair<string, int> precondition in preconditions)
            {
                if (!state.ContainsKey(precondition.Key) || state[precondition.Key] < precondition.Value)
                {
                    // If the state does not contain the precondition or if it does not meet the required value, return false.
                    return false;
                }
            }

            // All preconditions satisfied.
            return true;
        }


        /// <summary>
        /// This method applies the effects of the action to the current state if the behavior tree evaluation result is SUCCESS.
        /// </summary>
        /// <param name="state">The current state of the AI.</param>
        /// <returns>The behavior state of the action after the application of the effects to the state.</returns>
        public CandiceBehaviorStates Apply(CandiceDictionary<string, int> state)
        {
            // By default, assume the behavior state of the action is SUCCESS.
            CandiceBehaviorStates behaviorState = CandiceBehaviorStates.SUCCESS;

            // Evaluate the behavior tree if it exists and update the behavior state of the action.
            if (behaviorTree != null)
            {
                behaviorState = behaviorTree.Evaluate();
            }  

            // Only apply the effects if the behavior tree is null. If it is not null, the behavior tree will trigger an event when it completes. The callback function is where the apply effects logic lies.
            if (behaviorState == CandiceBehaviorStates.SUCCESS && behaviorTree == null)
            {
                foreach (KeyValuePair<string, int> effect in effects)
                {
                    // If the state already contains the effect key, add the effect value to the existing value.
                    if (state.ContainsKey(effect.Key))
                    {
                        state[effect.Key] += effect.Value;
                    }
                    // If the state does not contain the effect key, add the effect key-value pair to the state.
                    else
                    {
                        state[effect.Key] = effect.Value;
                    }
                }
                isComplete = true;
            }

            // Return the behavior state of the action after the application of the effects to the state.
            return behaviorState;
        }

        public CandiceBehaviorStates ApplySimulated(Dictionary<string, int> state)
        {
            // By default, assume the behavior state of the action is SUCCESS.
            CandiceBehaviorStates behaviorState = CandiceBehaviorStates.SUCCESS;

            foreach (KeyValuePair<string, int> effect in effects)
            {
                // If the state already contains the effect key, add the effect value to the existing value.
                if (state.ContainsKey(effect.Key))
                {
                    state[effect.Key] += effect.Value;
                }
                // If the state does not contain the effect key, add the effect key-value pair to the state.
                else
                {
                    state[effect.Key] = effect.Value;
                }
            }
            //isComplete = true;

            // Return the behavior state of the action after the application of the effects to the state.
            return behaviorState;
        }
        public CandiceGOAPActionS ConvertToGOAPActionS()
        {
            List<CandiceKeyValuePair<string, int>> _preconditions = new List<CandiceKeyValuePair<string, int>>();
            List<CandiceKeyValuePair<string, int>> _effects = new List<CandiceKeyValuePair<string, int>>();

            foreach (KeyValuePair<string, int> item in preconditions)
            {
                _preconditions.Add(new CandiceKeyValuePair<string, int>(item.Key, item.Value));
            }
            foreach (KeyValuePair<string, int> item in effects)
            {
                _effects.Add(new CandiceKeyValuePair<string, int>(item.Key, item.Value));
            }
            CandiceGOAPActionS action = new CandiceGOAPActionS(name, cost, _preconditions, _effects, behaviorTreeS, isComplete);
            return action;
        }

    }
    

    [Serializable]
    public struct CandiceKeyValuePair<K, V>
    {
        public K key;
        public V value;

        public CandiceKeyValuePair(K key, V value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
