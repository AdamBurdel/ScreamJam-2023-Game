using System;
using System.Collections.Generic;
using System.Linq;
namespace CandiceAIforGames.AI
{
    /// <summary>
    /// A node used in Goal-oriented action planning (GOAP).
    /// </summary>
    class CandiceGOAPNode
    {
        public CandiceGOAPNode parent; // The parent node of the current node
        public int cost; // The cost to reach the current node
        public CandiceGOAPState state; // The state of the world at the current node
        public CandiceGOAPAction action; // The action taken to reach the current node

        /// <summary>
        /// Initializes a new instance of the CandiceGOAPNode class.
        /// </summary>
        /// <param name="parent">The parent node of the current node.</param>
        /// <param name="cost">The cost to reach the current node.</param>
        /// <param name="state">The state of the world at the current node.</param>
        /// <param name="action">The action taken to reach the current node.</param>
        public CandiceGOAPNode(CandiceGOAPNode parent, int cost, CandiceGOAPState state, CandiceGOAPAction action)
        {
            this.parent = parent;
            this.cost = cost;
            this.state = state;
            this.action = action;
        }

        /// <summary>
        /// Checks whether the current node satisfies a given goal state.
        /// </summary>
        /// <param name="goal">The goal state to check for satisfaction.</param>
        /// <returns>True if the current node satisfies the goal state, otherwise false.</returns>
        public bool Satisfies(CandiceDictionary<string, int> goal)
        {
            foreach (KeyValuePair<string, int> condition in goal)
            {
                // Check if the current node's state contains the goal state condition and satisfies it
                if (!state.state.ContainsKey(condition.Key) || state.state[condition.Key] < condition.Value)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Calculates the heuristic value for the current node based on the given goal state.
        /// </summary>
        /// <param name="goal">The goal state to calculate the heuristic for.</param>
        /// <returns>The heuristic value for the current node.</returns>
        public int Heuristic(CandiceDictionary<string, int> goal)
        {
            int heuristicValue = 0;

            // Iterate through each condition in the goal state
            foreach (KeyValuePair<string, int> condition in goal)
            {
                // If the current node's state does not contain the condition, add it to the heuristic value
                if (!state.state.ContainsKey(condition.Key))
                {
                    heuristicValue += condition.Value;
                }
                // If the current node's state does contain the condition, but not enough to satisfy the goal state, add the difference to the heuristic value
                else if (state.state[condition.Key] < condition.Value)
                {
                    heuristicValue += condition.Value - state.state[condition.Key];
                }
            }
            return heuristicValue;
        }
    }
}
