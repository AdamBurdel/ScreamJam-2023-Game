using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/*
 * ToDo: Create Apply function in GOAPAgent to handle the Plan calulation. Refactor Apply function in GOAPAction to Evaluate, as it must evaluate a behavior tree. 
 *       Behavior Designer must have "Completion node" property.
 */
namespace CandiceAIforGames.AI
{
    public class CandiceGOAPAgent: MonoBehaviour
    {
        public CandiceAIController aiController;
        public CandiceGOAPState gameState = new CandiceGOAPState(new Dictionary<string, int>
        {
            {"pickaxe", 1},
            {"axe", 0}
        }, "Game State");
        public CandiceGOAPState goalState = new CandiceGOAPState(new Dictionary<string, int>
            {
                {"stone", 5},
            }, "Goal State");
        public List<CandiceGOAPActionS> availableActionsS = new List<CandiceGOAPActionS>();
        public List<CandiceGOAPAction> availableActions = new List<CandiceGOAPAction>();
        /*public List<CandiceGOAPAction> availableActions = new List<CandiceGOAPAction>
        {
            new CandiceGOAPAction(aiController, "GetLog", 1, new Dictionary<string, int> { }, new Dictionary<string, int> { {"log", 1} }),
            new CandiceGOAPAction(aiController, "ChopWood", 1, new Dictionary<string, int> { {"axe", 1},{"log", 1} }, new Dictionary<string, int> { {"wood", 1}, {"log", -1} }),
            new CandiceGOAPAction(aiController, "DropWood", 1, new Dictionary<string, int>(), new Dictionary<string, int> { {"wood", -1} }),
            new CandiceGOAPAction(aiController, "AcquireAxe", 1, new Dictionary<string, int>(), new Dictionary<string, int> { {"axe", 1} }),
            new CandiceGOAPAction(aiController, "DropAxe", 1, new Dictionary<string, int>(), new Dictionary<string, int> { {"axe", -1} })
        };*/
        static Queue<CandiceGOAPAction> planQueue = new Queue<CandiceGOAPAction>();
        static CandiceGOAPAction currentAction = null;

        // Start is called before the first frame update
        void Start()
        {
            aiController.AddRegistrationListener(onAgentReady);
            
        }

        public void onAgentReady(bool isRegistered, int agentID)
        {
            

            availableActions.Clear();
            foreach (CandiceGOAPActionS actionS in availableActionsS)
            {
                CandiceGOAPAction action = actionS.ConvertToGOAPAction(aiController);
                availableActions.Add(action);
                Debug.Log(action.name);
            }
            List<CandiceGOAPAction> plan = Plan(availableActions, gameState, goalState);
            if (plan == null)
            {
                Debug.Log("No plan found.");
                return;
            }

            Debug.Log("Plan found:");
            foreach (CandiceGOAPAction action in plan)
            {
                Console.WriteLine("- " + action.name);
                planQueue.Enqueue(action);

            }
            currentAction = planQueue.Dequeue();
        }

        // Update is called once per frame
        void Update()
        {
            if(currentAction != null)
            {
                CandiceBehaviorStates behaviorState = currentAction.Apply(gameState.state);
                if(currentAction.isComplete)
                {
                    if(planQueue.Count > 0)
                    {
                        currentAction = planQueue.Dequeue();
                    }
                    else
                    {
                        currentAction = null;
                    }
                }
            }
        }

        /// <summary>
        /// Given a list of available actions, an initial state, and a goal state, use the A* search algorithm to find
        /// a plan that transforms the initial state into the goal state. Returns a list of actions in the order they should
        /// be executed to achieve the goal state. Returns null if no plan is found.
        /// </summary>
        /// <param name="availableActions">A list of available actions that can be taken.</param>
        /// <param name="initialState">The initial state of the system.</param>
        /// <param name="goalState">The goal state that we want to achieve.</param>
        /// <returns>A list of actions in the order they should be executed to achieve the goal state. Returns null if no plan is found.</returns>
        static List<CandiceGOAPAction> Plan(List<CandiceGOAPAction> availableActions, CandiceGOAPState initialState, CandiceGOAPState goalState)
        {
            // Initialize the frontier with the initial state of the problem.
            List<CandiceGOAPNode> frontier = new List<CandiceGOAPNode> { new CandiceGOAPNode(null, 0, initialState, null) };

            // Keep track of the explored states to avoid cycles and improve efficiency.
            HashSet<Dictionary<string, int>> explored = new HashSet<Dictionary<string, int>>(new DictionaryComparer());

            // Initialize the f and g scores for each node in the frontier.
            CandiceDictionary<CandiceGOAPNode, int> fScore = new CandiceDictionary<CandiceGOAPNode, int>();
            fScore[frontier[0]] = frontier[0].Heuristic(goalState.state);
            CandiceDictionary<CandiceGOAPNode, int> gScore = new CandiceDictionary<CandiceGOAPNode, int>();
            gScore[frontier[0]] = 0;

            while (frontier.Count > 0)
            {
                // Sort the frontier in ascending order of f-score to explore the most promising nodes first.
                frontier = frontier.OrderBy(node => fScore.ContainsKey(node) ? fScore[node] : int.MaxValue).ToList();

                // Select the node with the lowest f-score to expand next.
                CandiceGOAPNode currentNode = frontier[0];

                // If the goal state has been reached, return the optimal plan.
                if (currentNode.Satisfies(goalState.state))
                {
                    List<CandiceGOAPAction> plan = new List<CandiceGOAPAction>();
                    while (currentNode.action != null)
                    {
                        plan.Insert(0, currentNode.action);
                        currentNode = currentNode.parent;
                    }
                    return plan;
                }

                // Remove the current node from the frontier and add it to the explored set.
                frontier.RemoveAt(0);
                explored.Add(CandiceGOAPState.ConvertToNormalDictionary(currentNode.state.state));

                // Generate all possible actions that can be applied to the current state.
                foreach (CandiceGOAPAction action in availableActions)
                {
                    // If the action is not applicable to the current state, skip it.
                    if (!action.IsAchievable(CandiceGOAPState.ConvertToNormalDictionary(currentNode.state.state)))
                    {
                        continue;
                    }

                    // Apply the action to the current state to generate a new child state.
                    CandiceGOAPState childState = new CandiceGOAPState(new Dictionary<string, int>(CandiceGOAPState.ConvertToNormalDictionary(currentNode.state.state)));
                    action.ApplySimulated(CandiceGOAPState.ConvertToNormalDictionary(childState.state));

                    // If the child state has already been explored, skip it.
                    if (explored.Contains(CandiceGOAPState.ConvertToNormalDictionary(childState.state)))
                    {
                        continue;
                    }

                    // Create a new node for the child state and update its g and f scores.
                    CandiceGOAPNode childNode = new CandiceGOAPNode(currentNode, currentNode.cost + action.cost, childState, action);
                    int tentativeGScore = gScore[currentNode] + action.cost;
                    if (!gScore.ContainsKey(childNode) || tentativeGScore < gScore[childNode])
                    {
                        childNode.parent = currentNode;
                        gScore[childNode] = tentativeGScore;
                        fScore[childNode] = gScore[childNode] + childNode.Heuristic(goalState.state);

                        // Add the child node to the frontier for further exploration.
                        frontier.Add(childNode);
                    }
                }
            }

            // If no plan is found, return null.
            return null;
        }

        private static int GetValueOrDefault(Dictionary<CandiceGOAPNode, int> dictionary, CandiceGOAPNode key, int defaultValue)
        {
            int val = 0;
            if (dictionary.TryGetValue(key, out val))
            {
                return val;
            }
            else
            {
                return defaultValue;
            }
        }
    }

    class DictionaryComparer : IEqualityComparer<Dictionary<string, int>>
    {
        public bool Equals(Dictionary<string, int> x, Dictionary<string, int> y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(Dictionary<string, int> obj)
        {
            return obj.Aggregate(0, (hash, pair) => hash ^ (pair.Key.GetHashCode() + pair.Value.GetHashCode()));
        }
    }
}
   