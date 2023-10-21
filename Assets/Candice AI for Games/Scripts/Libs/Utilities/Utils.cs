using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CandiceAIforGames.AI.Utils
{
    public class Utils
    {

        public static void LogClassInitialisation(object _class)
        {
            string[] className = (_class.ToString()).Split('.');
            Debug.Log(className[className.Length - 1] + ": Initialised.");
        }

        public static void LogDamageReceived(string name,float damage, float currentHP)
        {
            Debug.Log(name + ": Hit with " + damage + " damage. New Health = " + currentHP + "    Hit Points.");
        }

        public static void LogDamageDealt(string name,float damage)
        {
            Debug.Log(name + ": Dealing " + damage + " damage.");
        }
    }

    public class Enums
    {
        #region ENUMS
        public enum PatrolType
        {
            PatrolPoints,
            Waypoints,
        }
        public enum AttackType
        {
            Melee,
            Range,
        }
        public enum MovementType
        {
            STATIC,
            DYNAMIC,
            TILE_BASED
        }
        public enum PathfindSource
        {
            None,
            Candice,
            UnityNavmesh,
        }
        public enum SensorType
        {
            Sphere,
        }
        public enum AnimationType
        {
            TransitionBased,
            CodeBased,
        }
        #endregion

    }
}