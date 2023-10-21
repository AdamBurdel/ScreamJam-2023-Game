using UnityEngine;
using UnityEngine.UI;

namespace CandiceAIforGames.Data
{
    public class CandiceSaveItem : MonoBehaviour
    {
        public Text text;
        public string path;
        private CandiceDummyObject dummyObject;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void LoadGame()
        {
            dummyObject = (CandiceDummyObject)CandiceSaveSystem.Instance.LoadFromFile(path);
            Debug.Log("Name: " + dummyObject.Name + "\nFaction: " + dummyObject.Faction);
        }
    }
}