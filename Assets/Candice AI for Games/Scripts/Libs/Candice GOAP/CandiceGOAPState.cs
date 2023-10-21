using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CandiceAIforGames.AI
{
    /// <summary>
    /// Represents the current state of an agent in the world for goal-oriented action planning.
    /// </summary>
    /// 
    [Serializable]
    public class CandiceGOAPState
    {
        /// <summary>
        /// The state of the world, represented as a dictionary mapping resource names to resource quantities.
        /// </summary>
        public CandiceDictionary<string, int> state;
        public string stateName;

        /// <summary>
        /// Constructs a new CandiceGOAPState.
        /// </summary>
        /// <param name="state">The initial state of the world.</param>
        public CandiceGOAPState(Dictionary<string, int> state, string stateName = "Current State")
        {
            this.state = ConvertToCandiceDictionary(state);
            this.stateName = stateName;
        }

        public override string ToString()
        {
            string str = String.Format("------------\n{0}\n------------\n", stateName);
            foreach (KeyValuePair<string, int> condition in state)
            {
                str += String.Format("{0}: {1}\n", condition.Key, condition.Value);
            }
            str += String.Format("------------", stateName);
            return str;
        }
        /// <summary>
        /// Determines whether this state satisfies the given goal.
        /// </summary>
        /// <param name="goal">The goal state to satisfy.</param>
        /// <returns>True if the goal is satisfied, false otherwise.</returns>
        public bool SatisfiesGoal(CandiceGOAPState goal)
        {
            // Check each resource in the goal state and ensure it is present in this state with the required quantity
            foreach (KeyValuePair<string, int> resource in goal.state)
            {
                if (!state.ContainsKey(resource.Key) || state[resource.Key] < resource.Value)
                {
                    return false;
                }
            }
            return true;
        }

        public int CalculateHeuristicValue(CandiceGOAPState goal)
        {
            int heuristicValue = 0;

            // Iterate through each condition in the goal state
            foreach (KeyValuePair<string, int> condition in goal.state)
            {
                // If the current node's state does not contain the condition, add it to the heuristic value
                if (!state.ContainsKey(condition.Key))
                {
                    heuristicValue += condition.Value;
                }
                // If the current node's state does contain the condition, but not enough to satisfy the goal state, add the difference to the heuristic value
                else if (state[condition.Key] < condition.Value)
                {
                    heuristicValue += condition.Value - state[condition.Key];
                }
            }
            return heuristicValue;
        }

        /// <summary>
        /// Applies the effects of the given action to this state.
        /// </summary>
        /// <param name="action">The action whose effects to apply.</param>
        public void ApplyActionEffects(CandiceGOAPAction action)
        {
            // Apply each effect of the action to this state
            foreach (KeyValuePair<string, int> effect in action.effects)
            {
                if (state.ContainsKey(effect.Key))
                {
                    state[effect.Key] += effect.Value;
                }
                else
                {
                    state[effect.Key] = effect.Value;
                }
            }
        }

        /// <summary>
        /// Returns a new state object that results from applying the given action to this state.
        /// </summary>
        /// <param name="action">The action to apply.</param>
        /// <returns>The new state resulting from the action.</returns>
        public CandiceGOAPState GetActionResult(CandiceGOAPAction action)
        {
            // Create a new state object with the same state as this state
            CandiceGOAPState result = new CandiceGOAPState(ConvertToNormalDictionary(state));

            // Apply the effects of the action to the new state
            result.ApplyActionEffects(action);

            return result;
        }

        /// <summary>
        /// Determines whether this state is achievable in the given state.
        /// </summary>
        /// <param name="otherState">The state to compare against.</param>
        /// <returns>True if this state is achievable in the other state, false otherwise.</returns>
        public bool IsAchievable(CandiceGOAPState otherState)
        {
            // Check each resource in this state and ensure it is present in the other state with the required quantity
            foreach (KeyValuePair<string, int> resource in state)
            {
                if (!otherState.state.ContainsKey(resource.Key) || otherState.state[resource.Key] < resource.Value)
                {
                    return false;
                }
            }
            return true;
        }

        public void AddState(string key, int value)
        {
            if (!state.ContainsKey(key))
            {
                state.Add(key, value);
            }
            else
            {
                state[key] = value;
            }
        }

        public void RemoveState(string key)
        {
            if (state.ContainsKey(key))
            {
                state.Remove(key);
            }
        }

        public int GetState(string key)
        {
            if (state.ContainsKey(key))
            {
                return state[key];
            }
            else
            {
                return 0;
            }
        }

        public bool HasState(string key)
        {
            return state.ContainsKey(key);
        }

        public bool HasState(string key, int value)
        {
            return state.ContainsKey(key) && state[key] == value;
        }

        public bool HasStates(Dictionary<string, int> conditions)
        {
            foreach (KeyValuePair<string, int> condition in conditions)
            {
                if (!state.ContainsKey(condition.Key) || state[condition.Key] != condition.Value)
                {
                    return false;
                }
            }
            return true;
        }

        public bool HasAnyState(Dictionary<string, int> conditions)
        {
            foreach (KeyValuePair<string, int> condition in conditions)
            {
                if (state.ContainsKey(condition.Key) && state[condition.Key] == condition.Value)
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyFrom(CandiceGOAPState other)
        {
            state.Clear();
            foreach (KeyValuePair<string, int> entry in other.state)
            {
                state[entry.Key] = entry.Value;
            }
        }
        public static CandiceDictionary<string, int> ConvertToCandiceDictionary(Dictionary<string, int> dictionary)
        {
            CandiceDictionary<string, int> serializableDictionary = new CandiceDictionary<string, int>();

            foreach (var kvp in dictionary)
            {
                serializableDictionary.Add(kvp.Key, kvp.Value);
            }

            return serializableDictionary;
        }
        public static Dictionary<string, int> ConvertToNormalDictionary(CandiceDictionary<string, int> serializableDictionary)
        {
            Dictionary<string, int> normalDictionary = new Dictionary<string, int>();

            foreach (KeyValuePair <string, int> kvp in serializableDictionary)
            {
                normalDictionary[kvp.Key] = kvp.Value;
            }

            return normalDictionary;
        }
    }

    [System.Serializable]
    public class CandiceDictionary<TKey, TValue>
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();

        [SerializeField]
        private List<TValue> values = new List<TValue>();

        public int Count => keys.Count;

        public TValue this[TKey key]
        {
            get
            {
                int index = keys.IndexOf(key);
                if (index >= 0)
                {
                    return values[index];
                }
                else
                {
                    throw new KeyNotFoundException($"Key not found: {key}");
                }
            }
            set
            {
                int index = keys.IndexOf(key);
                if (index >= 0)
                {
                    values[index] = value;
                }
                else
                {
                    keys.Add(key);
                    values.Add(value);
                }
            }
        }

        public bool ContainsKey(TKey key)
        {
            return keys.Contains(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = keys.IndexOf(key);
            if (index >= 0)
            {
                value = values[index];
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (!ContainsKey(key))
            {
                keys.Add(key);
                values.Add(value);
            }
            else
            {
                Debug.LogWarning($"Key '{key}' already exists in the dictionary.");
            }
        }

        public bool Remove(TKey key)
        {
            int index = keys.IndexOf(key);
            if (index >= 0)
            {
                keys.RemoveAt(index);
                values.RemoveAt(index);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < keys.Count; i++)
            {
                yield return new KeyValuePair<TKey, TValue>(keys[i], values[i]);
            }
        }
    }


}

