//System
using System.Collections;
using System.Collections.Generic;
//Unity
using UnityEngine;
using UnityEngine.UI;
//Candice AI
using CandiceAIforGames.AI;

namespace CandiceAIforGames.AI
{
    public class Handicaper : MonoBehaviour
    {

        [HideInInspector]
        public TMPro.TMP_Dropdown HandicapDrop;
        public static string SelectedHandicap;

        // Start is called before the first frame update
        void Start()
        {
            //Get available handicaps
            HandicapDrop = GetHandicaps();
            //When changing Handicap
            if (HandicapDrop != null) {
                HandicapDrop.onValueChanged.AddListener(delegate { ChangeHandicap(); });
            }            

        }

        public TMPro.TMP_Dropdown GetHandicaps() {

            return GetComponent<TMPro.TMP_Dropdown>();

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void ChangeHandicap()
        {
            if (HandicapDrop.value == 0) {
                SelectedHandicap = "None";
            }
            else if (HandicapDrop.value == 1) {
                SelectedHandicap = "Bleed";
            }            
        }
    }
}