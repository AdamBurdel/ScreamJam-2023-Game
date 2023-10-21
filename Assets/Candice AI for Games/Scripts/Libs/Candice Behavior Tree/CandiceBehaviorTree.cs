using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CandiceAIforGames.AI
{
    // Define a delegate type for the completion event
    public delegate void CandiceBehaviorTreeEvent(Action<CandiceBehaviorTreeEventData> eventData);
    public delegate void CandiceBehaviorTreeCompletedEvent();

    [Serializable]
    public class CandiceBehaviorTree
    {
        [SerializeField]
        public string _name;
        public CandiceBehaviorNode rootNode;
        [SerializeField]
        public List<CandiceBehaviorNodeS> nodes;
        
        [SerializeField]
        List<MethodInfo> lstFunctions;

        // Define the completion event
        public event Action<CandiceBehaviorTreeEvent> OnComplete;

        public void Initialise()
        {
            List<Type> behaviorTypes = new List<Type>();
            lstFunctions = new List<MethodInfo>();
            MethodInfo[] arrMethodInfos;
            behaviorTypes.Add(typeof(CandiceDefaultBehaviors));
            foreach (Type type in behaviorTypes)
            {
                arrMethodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                lstFunctions.AddRange(arrMethodInfos);
            }
        }

        public CandiceBehaviorStates Evaluate()
        {
            CandiceBehaviorStates state = CandiceBehaviorStates.RUNNING;
            if (rootNode != null)
            {
                state = rootNode.Evaluate();
            }
            else
            {
                state = CandiceBehaviorStates.FAILURE;
            }
            return state;
        }
        public CandiceBehaviorNode CreateBehaviorTree(CandiceAIController agent, Action<CandiceBehaviorTreeEventData> _behviorTreeCallback)
        {
            //Initialise();
            rootNode = null;
            CandiceBehaviorNodeS _rootNode = null;
            //nodes = behaviorTree.GetNodes();
            if(nodes == null || nodes.Count == 0)
            {
                return null;
            }

            _rootNode = nodes[0];

            switch (_rootNode.type)
            {
                case CandiceAIManager.NODE_TYPE_SELECTOR:
                    rootNode = new CandiceBehaviorSelector();
                    rootNode.id = _rootNode.id;
                    rootNode.Initialise(agent.transform, agent);
                    (rootNode as CandiceBehaviorSelector).SetNodes(GetChildren(nodes, _rootNode));
                    break;
                case CandiceAIManager.NODE_TYPE_SEQUENCE:
                    rootNode = new CandiceBehaviorSequence();
                    rootNode.id = _rootNode.id;
                    rootNode.Initialise(agent.transform, agent);
                    (rootNode as CandiceBehaviorSequence).SetNodes(GetChildren(nodes, _rootNode));
                    break;
            }
            rootNode.SetCallback(_behviorTreeCallback);

            return rootNode;
        }
        List<CandiceBehaviorNode> GetChildren(List<CandiceBehaviorNodeS> nodes, CandiceBehaviorNodeS node)
        {
            List<CandiceBehaviorNode> children = new List<CandiceBehaviorNode>();
            CandiceBehaviorNodeS _node = null;
            if (node.childrenIDs.Count < 1)
            {
                return children;
            }
            foreach (int id in node.childrenIDs)
            {
                CandiceBehaviorNode newNode = null;
                if (GetNodeWithID(id, nodes) != null)
                {
                    _node = GetNodeWithID(id, nodes);

                    switch (_node.type)
                    {
                        case CandiceAIManager.NODE_TYPE_SELECTOR:
                            newNode = new CandiceBehaviorSelector();
                            (newNode as CandiceBehaviorSelector).SetNodes(GetChildren(nodes, _node));
                            break;
                        case CandiceAIManager.NODE_TYPE_SEQUENCE:
                            newNode = new CandiceBehaviorSequence();
                            (newNode as CandiceBehaviorSequence).SetNodes(GetChildren(nodes, _node));
                            break;
                        case CandiceAIManager.NODE_TYPE_ACTION:
                            int index = getFunctionByName(_node.function);
                            CandiceBehaviorAction action = new CandiceBehaviorAction((CandiceBehaviorAction.ActionNodeDelegate)lstFunctions[index].CreateDelegate(typeof(CandiceBehaviorAction.ActionNodeDelegate)), rootNode);
                            newNode = action;
                            break;
                    }
                    children.Add(newNode);
                }
            }
            return children;
        }

        public int getFunctionByName(string name)
        {
            int index = 0;
            int foundIndex = -1;
            while(index < lstFunctions.Count-1 || foundIndex > -1)
            {
                index++;
                MethodInfo func = lstFunctions[index];
                if(func.Name == name)
                {
                    foundIndex = index;
                }
            }

            return foundIndex;
        }
        CandiceBehaviorNodeS GetNodeWithID(int id, List<CandiceBehaviorNodeS> nodes)
        {
            CandiceBehaviorNodeS node = null;
            bool isFound = false;
            int count = 0;
            while (!isFound && count < nodes.Count)
            {
                if (nodes[count].id == id)
                {
                    node = nodes[count];
                    isFound = true;
                }
                count++;
            }
            return node;
        }


    }
}
