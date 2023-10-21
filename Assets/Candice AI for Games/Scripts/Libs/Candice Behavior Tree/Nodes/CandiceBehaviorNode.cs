using CandiceAIforGames.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CandiceAIforGames.AI
{
    [System.Serializable]
    public abstract class CandiceBehaviorNode
    {
        public Transform transform;
        public CandiceAIController aiController;
        public int patrolCount = 0;
        public int id;
        public bool enableEvents = false;
        public Action<CandiceBehaviorTreeEventData> behaviorTreeCallback;
        public CandiceBTEventTypes eventType;
        /* Delegate that returns the state of the node.*/
        public delegate CandiceBehaviorStates NodeReturn();


        /* The current state of the node */
        protected CandiceBehaviorStates m_nodeState;

        public CandiceBehaviorStates nodeState
        {
            get { return m_nodeState; }
        }

        /* The constructor for the node */
        public CandiceBehaviorNode() { }
        public void SetCallback(Action<CandiceBehaviorTreeEventData> _behviorTreeCallback)
        {
            this.behaviorTreeCallback = _behviorTreeCallback;
        }
        public void Initialise(Transform transform, CandiceAIController aiController)
        {
            this.transform = transform;
            this.aiController = aiController;
        }

        /* Implementing classes use this method to evaluate the desired set of conditions */
        /// <summary>
        /// Inherited classes use this method to evaluate the desired set of conditions
        /// </summary>
        public abstract CandiceBehaviorStates Evaluate();

    }

    public enum CandiceBTEventTypes
    {
        EVENT_TYPE_NONE,
        EVENT_TYPE_COMPLETE,
        EVENT_TYPE_CHECKPOINT,

    }
    public struct CandiceBehaviorTreeEventData
    {
        public string actionName;
        public CandiceBTEventTypes eventType;
        public CandiceBehaviorStates behaviorState;


        public CandiceBehaviorTreeEventData(string actionName, CandiceBTEventTypes eventType, CandiceBehaviorStates behaviorState)
        {
            this.actionName = actionName;
            this.eventType = eventType;
            this.behaviorState = behaviorState;
        }
    }
}
