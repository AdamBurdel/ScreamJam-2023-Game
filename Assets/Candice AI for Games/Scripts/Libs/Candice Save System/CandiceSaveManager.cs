using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CandiceAIforGames.Data
{
    public class CandiceSaveManager : MonoBehaviour
    {
        public bool enableAutoSave;
        public bool enableFastMode;
        public int autoSaveInterval;
        public KeyCode fastSaveKey;
        public KeyCode fastLoadKey;
        public string storagePath;
        public string folderName;
        public GameObject savePanel;
        public GameObject container;
        public GameObject saveObject;
        public string saveType;
        private const string SAVEFILEEXTENSION = ".cndc";//The extension of the save file that will be generated.
        Vector3 pos;

        //public static ObjectsBL oBL;
        [HideInInspector]
        public static bool bProviderSelected;

        // Start is called before the first frame update
        void Start()
        {
            CandiceSaveSystem.Instance.Initialise(storagePath);
            InitialiseWeaponDB();
            LoadSaveItems();
            GetWeapons();
        }
        // Update is called once per frame
        void Update()
        {

        }


        public void Save(object obj, Text filename)
        {
            if (filename.text.Length < 1)
            {
                Debug.LogWarning("ERROR: Please enter a save filename");
                return;
            }
            CandiceSaveSystem.Instance.SaveToFile(obj, folderName + "/" + filename.text + SAVEFILEEXTENSION);
            LoadSaveItems();
        }







        void InitialiseWeaponDB()
        {
            CandiceWeapon weapon = new CandiceWeapon(0, "Long Sword2", "Sword", 22.5);
            AddWeaponToDB(weapon);
            weapon = new CandiceWeapon(1, "Short Sword", "Sword", 15.5);
            AddWeaponToDB(weapon);
        }
        public List<CandiceWeapon> GetWeapons()
        {
            string query = "SELECT * FROM Weapons";
            CandiceSaveSystem.Instance.SetQuery(query);
            List<CandiceWeapon> weapons = new List<CandiceWeapon>();
            List<object> obj = CandiceSaveSystem.Instance.SelectAll();
            foreach (object o in obj)
            {
                weapons.Add(o as CandiceWeapon);
                Debug.Log("Name: " + (o as CandiceWeapon).WeaponName);
            }
            return weapons;
        }
        private void AddWeaponToDB(CandiceWeapon weapon)
        {
            if (!CandiceSaveSystem.Instance.DatabaseExists("TestDB"))
            {
                CandiceSaveSystem.Instance.CreateDatabase("TestDB");
            }
            Dictionary<object, object> parameters = new Dictionary<object, object>();
            parameters.Add("@WPN_ID", weapon.WeaponID);
            parameters.Add("@WPN_NAME", weapon.WeaponName);
            parameters.Add("@WPN_TYPE", weapon.WeaponType);
            parameters.Add("@WPN_DAMAGE", weapon.WeaponDamage);
            string query = "INSERT INTO Weapons ([WPN_ID], [WEAPON_NAME], [WEAPON_TYPE], [WEAPON_DAMAGE])" +
                " VALUES (@WPN_ID, @WEAPON_NAME, @WEAPON_TYPE, @WEAPON_DAMAGE)";
            CandiceSaveSystem.Instance.SetQuery(query);
            CandiceSaveSystem.Instance.Insert(parameters);
        }

        public void LoadSaveItems()
        {
            ClearChildren(container);
            Vector3 parentScale = new Vector3(container.transform.localScale.x, container.transform.localScale.y, container.transform.localScale.z);

            pos = new Vector3(parentScale.x / 2, 140, parentScale.z / 2);
            string[] filenames = CandiceSaveSystem.Instance.GetFileNames(folderName);
            foreach (string file in filenames)
            {
                GameObject obj = Instantiate(saveObject, pos, Quaternion.identity);
                obj.transform.SetParent(container.transform, false);
                string[] names = file.Split('/');
                obj.GetComponent<CandiceSaveItem>().text.text = names[names.Length - 1].Split('.')[0];
                obj.GetComponent<CandiceSaveItem>().path = folderName + "/" + names[names.Length - 1];
                pos.y -= 35f;
            }
        }
        private void ClearChildren(GameObject parent)
        {
            int count = parent.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                Destroy(parent.transform.GetChild(i).gameObject);
            }
        }
        
    }
}

