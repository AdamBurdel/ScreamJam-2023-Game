using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace CandiceAIforGames.AI
{
    public class BasicSaveSystem
    {
        string storagePath;
        public BasicSaveSystem(string filename)
        {
            storagePath = Application.dataPath + "//Candice Behavior Designer/Resources/Datastore/" + filename + ".bin";
        }
        public bool SaveToFile(object data)
        {

            bool isSaved = false;
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                if (File.Exists(storagePath))
                {
                    File.Delete(storagePath);
                }
                FileStream file = File.Create(storagePath);
                bf.Serialize(file, data);
                file.Close();
                isSaved = true;
            }
            catch (Exception e)
            {
                Debug.Log("ERROR: " + e.Message);
            }
            return isSaved;

        }

        public object LoadFromFile()
        {
            object obj = null;
            try
            {
                if (File.Exists(storagePath))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream file = File.Open(storagePath, FileMode.Open);
                    obj = bf.Deserialize(file);
                    file.Close();
                }
            }
            catch (Exception e)
            {
                Debug.Log("ERROR: " + e.Message);
            }
            return obj;
        }

    }
}
