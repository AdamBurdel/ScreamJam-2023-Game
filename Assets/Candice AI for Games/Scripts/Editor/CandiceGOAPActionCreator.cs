using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using CandiceAIforGames.AI;

namespace CandiceAIforGames.AI
{
    public class CandiceGOAPActionCreator : EditorWindow
    {
        string actionName;
        List<CandiceKeyValuePair<string, int>> effects = new List<CandiceKeyValuePair<string, int>>();
        List<CandiceKeyValuePair<string, int>> requirements = new List<CandiceKeyValuePair<string, int>>();
        static CandiceGOAPActionCreator window;

        [MenuItem("Window/Candice AI for Games/Tools/Candice GOAP Action Creator")]
        public static void ShowWindow()
        {
            window = EditorWindow.GetWindow<CandiceGOAPActionCreator>();
            window.titleContent = new GUIContent("Candice GOAP Action Creator");
        }

        void OnGUI()
        {
            GUILayout.Label("Create New GOAP Action", EditorStyles.boldLabel);

            // Action Name
            GUILayout.Space(10);
            GUILayout.Label("Action Name", EditorStyles.boldLabel);
            actionName = EditorGUILayout.TextField(actionName);

            // Effects
            GUILayout.Space(10);
            GUILayout.Label("Effects", EditorStyles.boldLabel);
            for (int i = 0; i < effects.Count; i++)
            {
                /*string key = effects[i].key;
                int value = effects[i].value;*/
                CandiceKeyValuePair<string, int> effect = effects[i];
                EditorGUILayout.BeginHorizontal();
                effect.key = EditorGUILayout.TextField(effect.key);
                effect.value = EditorGUILayout.IntField(effect.value);
                EditorGUILayout.EndHorizontal();
                effects[i] = effect;
            }
            if (GUILayout.Button("Add Effect"))
            {
                effects.Add(new CandiceKeyValuePair<string, int>("", 0));
            }

            // Requirements
            GUILayout.Space(10);
            GUILayout.Label("Requirements", EditorStyles.boldLabel);
            for (int i = 0; i < requirements.Count; i++)
            {
                CandiceKeyValuePair<string, int> requirement = requirements[i];
                EditorGUILayout.BeginHorizontal();
                requirement.key = EditorGUILayout.TextField(requirement.key);
                requirement.value = EditorGUILayout.IntField(requirement.value);
                EditorGUILayout.EndHorizontal();
                requirements[i] = requirement;
            }
            if (GUILayout.Button("Add Requirement"))
            {
                requirements.Add(new CandiceKeyValuePair<string, int>("", 0));
            }

            // Create Button
            GUILayout.Space(20);
            if (GUILayout.Button("Create"))
            {
                // Create the GOAP action object
                CandiceGOAPActionS newAction = ScriptableObject.CreateInstance<CandiceGOAPActionS>();
                newAction.name = actionName;
                newAction.effects = effects;
                newAction.preconditions = requirements;

                // Save the asset
                string path = EditorUtility.SaveFilePanelInProject("Save GOAP Action", actionName, "asset", "Save GOAP Action");
                if (path != "")
                {
                    AssetDatabase.CreateAsset(newAction, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Debug.Log("GOAP Action created at " + path);
                    Close();
                }
            }
        }
    }
}

