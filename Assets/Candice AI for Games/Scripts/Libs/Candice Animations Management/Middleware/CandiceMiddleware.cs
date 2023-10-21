//System
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
//Unity
using UnityEngine;
using UnityEngine.UI;
//Candice AI
using CandiceAIforGames.AI;

namespace CandiceAIforGames.AI
{
    //Class to handle any 3rd party plugins, assets, free or otherwise
    public class CandiceMiddleware
    {
        //some of our favorite middleware
        public enum SupportedMiddleware {

            FirstGearGames, //Most FirstGearGames related starter products
            SmoothCameraShaker, //FirstGearGames
            CameraShakerHandler, //FirstGearGames SmoothCameraShaker
            Shake, //FirstGearGames SmoothCameraShaker Shake Method

            Cinemachine, //Probably the most popular, freely accessible cinematic camera experience for Unity
            CinemachineVirtualCamera, //Cinemamachine virtual camera
            LookAt, //Cinemamachine virtual camera LookAtMethod

            ClassicProgressBar, //Excellent health bars and progress bars free asset
            m_FillAmount //ClassicProgressBar displayed health amount

        }

        //assembly check
        //use for dependencies
        public bool CheckForDependency(string dependency)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Namespace == dependency)
                        return true;
                }
            }
            return false;
        }

        //return dependency assembly
        public Type GetDependency(string dependency)
        {

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Namespace == dependency)
                    {
                        return type;
                    }

                }
            }
            return null;
        }

        //return dependency as Invokable object (method wrapper)
        public MethodInfo NewDependency(Type newDependency, string methodName) {
            if (newDependency != null) {
                if (methodName != null)
                {
                    return newDependency.GetMethod(methodName);                    
                }
                
            }
            return null;
        }

        //REFLECTIONS
        //REFERENCE TO OBJECT
        //TO VALUE OF OBJECT
        //GET AND SET FIELDS
        public static T GetReference<T>(object inObj, string fieldName) where T : class
        {
            return GetField(inObj, fieldName) as T;
        }


        public static T GetValue<T>(object inObj, string fieldName) where T : struct
        {
            return (T)GetField(inObj, fieldName);
        }


        public static void SetField(object inObj, string fieldName, object newValue)
        {
            FieldInfo info = inObj.GetType().GetField(fieldName);
            if (info != null)
                info.SetValue(inObj, newValue);
        }


        private static object GetField(object inObj, string fieldName)
        {
            object ret = null;
            FieldInfo info = inObj.GetType().GetField(fieldName);
            if (info != null)
                ret = info.GetValue(inObj);
            return ret;
        }

    }
}