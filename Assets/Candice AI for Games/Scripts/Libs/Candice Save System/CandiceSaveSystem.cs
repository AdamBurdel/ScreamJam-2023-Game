using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace CandiceAIforGames.Data
{
    public class CandiceSaveSystem
    {
        //
        // Program : CandiceSaveSystem
        // Programmer: M Natuyamba
        // Name : CandiceSaveSystem
        // Purpose : This class provides the link to the DAL
        private static CandiceSaveSystem _instance;
        private string storagePath;
        //private string saveFilename = "SaveFile.bin";
        private string databaseName = "TestDB";
        private string defaultExtension = ".bin";
        private CandiceProviderBase providerBase;

        public static CandiceSaveSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CandiceSaveSystem();

                }
                return _instance;
            }
        }
        public void Initialise(string storagePath)
        {
            try
            {
                if (storagePath.Equals(""))
                {
                    storagePath = Application.persistentDataPath + "/";
                }
                else
                {
                    storagePath = storagePath.TrimEnd('/');
                    storagePath = storagePath + "/";
                }
                this.storagePath = storagePath;
                if (!File.Exists(storagePath))
                {
                    Directory.CreateDirectory(storagePath);
                }
                providerBase = new CandiceSQLiteProvider(@"Data Source=" + storagePath + databaseName + ".s3db;Version=3");
            }
            catch (Exception e)
            {
                Debug.Log("ERROR: " + e.Message);
            }

        }
        public CandiceSaveSystem()
        {

        }

        public void SaveToFile(object userData, string fileName)
        {
            //
            //Method Name : SaveToFile(object userData, string fileName)
            //Purpose     : Saves the object to a file using BinaryFormatter.
            //Re-use      : none
            //Input       : object userData, string fileName
            //Output      : none
            //
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                if (File.Exists(storagePath + fileName))
                {
                    File.Delete(storagePath + fileName);
                }
                FileStream file = File.Create(storagePath + fileName);
                bf.Serialize(file, userData);
                file.Close();
            }
            catch (Exception e)
            {
                Debug.Log("ERROR: " + e.Message);
            }

        }
        public string[] GetFileNames(string folderName = "")
        {
            //List<string> files = new List<string>();
            string[] files = null;
            try
            {
                if(folderName != "" && folderName != " ")
                {
                    folderName = folderName + "/";
                }
                else
                {
                    folderName = "";
                }
                files = Directory.GetFiles(storagePath + folderName);

            }
            catch (Exception e)
            {
                Debug.LogWarning("WARNING: " + e.Message);
            }
            return files;
        }
        public object LoadFromFile(string fileName)
        {
            //
            //Method Name : LoadFromFile()
            //Purpose     : Reads the object from a file using BinaryFormatter.
            //Re-use      : none
            //Input       : none
            //Output      : UserData userData
            //
            object obj = null;
            try
            {
                if (File.Exists(storagePath + fileName))
                {
                    Debug.Log("File esists, attempting to read... ");
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream file = File.Open(storagePath + fileName, FileMode.Open);
                    obj = bf.Deserialize(file);
                    file.Close();
                }
                else
                {
                    Debug.Log("File does not exist: " + storagePath + fileName);
                }
            }
            catch (Exception e)
            {
                Debug.Log("ERROR: " + e.Message);
            }
            return obj;
        }


        public void SetQuery(string query)
        {
            if (providerBase is CandiceSQLiteProvider)
            {
                (providerBase as CandiceSQLiteProvider).SetQuery(query);
            }
        }
        public void ChangeDatabaseName(string dbName)
        {
            databaseName = dbName;
            if (providerBase is CandiceSQLiteProvider)
            {
                (providerBase as CandiceSQLiteProvider).ChangeConnectionString(@"Data Source=" + storagePath + databaseName + ".s3db;Version=3");
            }
        }
        /// <summary>
        /// This method gets the list of all the business objects from the datastore.
        /// It returns the list of business objects
        /// </summary>
        public List<object> SelectAll()
        {
            return providerBase.SelectAll();
        } // end method
          /// <summary>
          /// This method gets a single object from the datastore.
          /// It returns 0 to indicate the object was loaded from the datastore, or
          /// -1 to indicate that no object was loaded from the datastore.
          /// </summary>
          /// <param name="serialNr">The object ID of the object to load from the datastore.</param>
          /// 
        public int SelectObject(string serialNr, ref Dictionary<object,object> obj)
        {
            return providerBase.SelectObject(ref obj, serialNr);
        } // end method

        /// <summary>
        /// This method inserts a record in the datastore.
        /// It returns 0 to indicate the object was inserted into datastore, or
        /// -1 to indicate the object was not inserted because a duplicate was found
        /// </summary>
        /// <param name="newVehicle">The object to add to the datastore.</param>
        public int Insert(Dictionary<object, object> newObj)
        {
            return providerBase.Insert(newObj);
        } // end method
          /// <summary>
          /// This method updates a record in the datastore.
          /// It returns 0 to indicate the object was found and updated successfully, or
          /// -1 to indicate the object was not updated because the record was not found
          /// </summary>
          /// <param name="newObj">The new  data for the record in the datastore.</param>
        public int Update(Dictionary<object, object> obj)
        {
            return providerBase.Update(obj);
        } // end method
          /// <summary>
          /// This method deletes a record in the datastore.
          /// It returns 0 to indicate the object was found and deleted successfully, or
          /// -1 to indicate the object was not deleted because the record was not found
          /// </summary>
          /// <param name="ID">The object ID of the object to delete in the datastore.</param>
        public int Delete(string ID)
        {
            return providerBase.Delete(ID);
        } // end method
        /*
        private void setupProviderBase(string Provider)
        {
            //
            //Method Name : void setupProviderBase()
            //Purpose : Helper method to select the correct data provider
            //Re-use : None
            //Input : string Provider
            // - The name of the data provider to use
            //Output : None
            //
            if (Provider == "XMLProvider")
            {
                //providerBase = new XMLProvider();
            } // end if
            else
            {
                if (Provider == "CandiceSQLiteProvider")
                {
                    providerBase = new CandiceSQLiteProvider(Application.persistentDataPath + "/" + databaseName + ".s3db;Version=3");
                } // end if
            } // end else
        } // end method
        */
        public int CreateDatabase(string databaseName)
        {
            ChangeDatabaseName(databaseName);
            //this.databaseName = databaseName;
            List<CandiceColumnInfo> columnInfos = new List<CandiceColumnInfo>();
            CandiceColumnInfo info = new CandiceColumnInfo("WPN_ID", "INTEGER", "", true, true, true);
            columnInfos.Add(info);
            info = new CandiceColumnInfo("WPN_NAME", CandiceDatatypes.TYPE_STRING, "", true, false, false);
            columnInfos.Add(info);
            info = new CandiceColumnInfo("WPN_TYPE", CandiceDatatypes.TYPE_STRING, "", true, false, false);
            columnInfos.Add(info);
            info = new CandiceColumnInfo("WPN_DAMAGE", CandiceDatatypes.TYPE_DOUBLE, "", true, false, false);
            columnInfos.Add(info);
            return CreateTable("weapon", columnInfos);
        }
        public int CreateTable(string tableName, List<CandiceColumnInfo> columnInfos)
        {
            int rc = 0;
            if (providerBase is CandiceSQLiteProvider)
            {
                string columnParameters = "";
                if (columnInfos != null)
                {
                    columnParameters = " (";

                    for (int i = 0; i < columnInfos.Count; i++)
                    {
                        CandiceColumnInfo info = columnInfos[i];
                        string nonNull = "";
                        string autoincrement = "";
                        string pk = "";
                        if (info.Pk)
                        {
                            pk = " PRIMARY KEY";
                        }
                        if (info.Ai)
                        {
                            autoincrement = " AUTOINCREMENT";
                        }
                        if (info.NotNull)
                        {
                            nonNull = " NOT NULL";
                        }

                        string fragment = info.Name + " " + info.Type + pk + autoincrement + nonNull;
                        if (i != columnInfos.Count - 1)
                        {
                            fragment += ",";
                            columnParameters += fragment;
                            columnParameters += " ";
                        }
                        else
                        {
                            columnParameters += fragment;
                        }


                    }
                    columnParameters += ")";
                }
                rc = (providerBase as CandiceSQLiteProvider).CreateTable(tableName, columnParameters);
            }
            else
            {
                rc = -1;
            }
            return rc;
        }
        public int DeleteTable(string tableName)
        {
            int rc = -1;
            if (providerBase is CandiceSQLiteProvider)
            {
                rc = (providerBase as CandiceSQLiteProvider).DeleteTable(tableName);
            }
            return rc;
        }
        public int AddColumn(string tableName, CandiceColumnInfo info)
        {
            int rc = 0;
            if (providerBase is CandiceSQLiteProvider)
            {
                string columnParameters = " (";
                string nonNull = "";
                string autoincrement = "";
                string pk = "";
                if (info.Pk)
                {
                    pk = " PRIMARY KEY";
                }
                if (info.Ai)
                {
                    autoincrement = " AUTOINCREMENT";
                }
                if (info.NotNull)
                {
                    nonNull = " NOT NULL";
                }
                columnParameters += info.Name + " " + info.Type + pk + autoincrement + nonNull + ")";
                rc = (providerBase as CandiceSQLiteProvider).CreateTable(tableName, columnParameters);
            }
            else
            {
                rc = -1;
            }
            return rc;
        }
        public bool DatabaseExists(string dbName)
        {
            bool isExist = false;
            if (File.Exists(storagePath + dbName + ".s3db"))
            {
                isExist = true;
            }

            return isExist;
        }
        public List<string> GetTableNames(string dbName)
        {
            ChangeDatabaseName(dbName);
            List<string> tableNames = null;
            if (providerBase is CandiceSQLiteProvider)
            {
                tableNames = (providerBase as CandiceSQLiteProvider).GetTableNames();
            }
            else
            {

            }
            return tableNames;
        }
        public List<CandiceColumnInfo> GetColumnInfo(string tableName)
        {
            List<CandiceColumnInfo> columnInfos = new List<CandiceColumnInfo>();
            if (providerBase is CandiceSQLiteProvider)
            {
                columnInfos = (providerBase as CandiceSQLiteProvider).GetColumnInfo(tableName);
            }

            return columnInfos;
        }

        public List<string> GetDatabaseNames()
        {
            List<string> dbNames = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(storagePath);
            FileInfo[] info = dir.GetFiles("*.*");
            foreach (FileInfo f in info)
            {
                string extension = ".s3db";
                if(providerBase is CandiceSQLiteProvider)
                {
                    extension = ".s3db";
                }
                if (f.Extension.Equals(extension))
                {
                    dbNames.Add(f.Name.Split('.')[0]);
                }
            }

            return dbNames;
        }

        public void DeleteFile(string fileName)
        {
            File.Delete(storagePath + fileName);
        }
    }
}

