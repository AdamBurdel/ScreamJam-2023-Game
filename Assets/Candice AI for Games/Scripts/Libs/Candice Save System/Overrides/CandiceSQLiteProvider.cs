using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CandiceAIforGames.Data
{
    public class CandiceSQLiteProvider : CandiceProviderBase
    {
        private string query = "";
        private string conStr = "";
        //private int OBJECT_TYPE = ObjectTypes.OBJECT_NONE;

        public CandiceSQLiteProvider(string conStr)
        {
            this.conStr = conStr;
        }
        #region CRUD METHODS
        public override int Delete(string serialNr = "")
        {
            //
            //Name            : int Delete(string serialNr)
            //Purpose         : Try to delete a row from the datastore
            //Re-use          : none
            //Input Parameter : string serialNr
            //                   - the serialNr of the object to delete in the  datastore
            //Output Type     : - int
            //                 0 : object found and deleted successfully
            //                -1 : object not deleted because the record was not found
            //
            int rc = 0;
            SqliteConnection sqlCon = null;
            SqliteCommand sqlCmd = null;
            string deleteQuery = "";
            try
            {
                if (!query.Equals(""))
                {

                    sqlCon = new SqliteConnection(conStr);
                    sqlCon.Open();
                    //deleteQuery = string.Format("DELETE FROM Objects WHERE [serialNr] = '{0}'", serialNr);
                    deleteQuery = string.Format(query);
                    sqlCmd = sqlCon.CreateCommand();
                    sqlCmd.CommandText = deleteQuery;
                    rc = sqlCmd.ExecuteNonQuery();
                    if (rc == 0)
                    {
                        rc = -1;
                    } // end if
                    else
                    {
                        rc = 0;
                    } // end else
                    sqlCmd.Dispose();
                    sqlCon.Dispose();
                }
                else
                {
                    Debug.LogError("Please call SetQuery() before calling Delete().");

                }




            } // end try 
            catch (Exception ex)
            {
                throw ex;
            } // end catch
            return rc;
        } // end method

        public override int Insert(Dictionary<object, object> parameters)
        {
            //
            //Name            : int Insert(object newObj)
            //Purpose         : Try to insert a row in the datastore
            //Re-use          : none
            //Input Parameter : object newObj
            //                  - The object to add to the datastore
            //Output Type     : - int
            //                  0 : newObj inserted into datastore
            //                 -1 : newObj not inserted because a duplicate was found
            //			
            int rc = 0;
            SqliteConnection sqlCon = null;
            SqliteCommand sqlCmd = null;
            try
            {

                //insertQuery = "INSERT INTO "+tableName+"([serialNr], [name], [faction], " +
                //    "[experience]) VALUES(@serialNr, @name, @faction, @experience)";
                if (!query.Equals(""))
                {
                    if (parameters != null)
                    {
                        sqlCon = new SqliteConnection(conStr);
                        sqlCon.Open();
                        sqlCmd = new SqliteCommand(query, sqlCon);
                        foreach (KeyValuePair<object, object> p in parameters)
                        {
                            sqlCmd.Parameters.AddWithValue(Convert.ToString(p.Key), p.Value);
                        }
                        sqlCmd.ExecuteNonQuery();
                        sqlCmd.Dispose();
                        sqlCon.Dispose();
                    }
                    else
                    {
                        Debug.LogError("Please call SetParameters() before calling Insert().");
                    }
                }
                else
                {
                    Debug.LogError("Please call SetQuery() before calling Insert().");

                }



            } // end try
            catch (SqliteException ex)
            {
                if (ex.ErrorCode == SQLiteErrorCode.Constraint)
                {
                    rc = -1;
                } // end if
                else
                {
                    throw ex;
                } // end else
            } // end catch
            catch (Exception ex)
            {
                throw ex;
            } // end catch
            return rc;
        } // end method

        public override List<object> SelectAll()
        {
            //
            //Name            : List<object> SelectAll()
            //Purpose         : Try to get all the objects from the datastore
            //Re-use          : none
            //Input Parameter : None        
            //Output Type     : List<object>
            //                 - the collection that will contain the objects loaded from datastore         
            //
            SqliteConnection sqlCon = null;
            List<object> list;
            SqliteCommand sqlCmd = null;
            SqliteDataReader sqlDr = null;


            try
            {
                list = new List<object>();
                if (!query.Equals(""))
                {
                    sqlCon = new SqliteConnection(conStr);
                    sqlCon.Open();
                    //string selectQuery = "SELECT * FROM Objects";
                    sqlCmd = new SqliteCommand(query, sqlCon);
                    sqlDr = sqlCmd.ExecuteReader();
                    while (sqlDr.Read())
                    {
                        Dictionary<object, object> obj = ConvDataToObject(sqlDr);
                        list.Add(obj);
                    } // end while

                    sqlDr.Close();
                    sqlCmd.Dispose();
                    sqlCon.Dispose();

                }
                else
                {
                    Debug.LogError("Please call SetQuery() before calling SelectAll().");
                }


            } //end try
            catch (Exception ex)
            {
                throw ex;
                //throw ex;
            } // end catch
            return list;
        } // end method

        public override int SelectObject(ref Dictionary<object,object> obj, string serialNr = "")
        {
            //
            //Name            : int SelectObject(string serialNr, ref object obj)
            //Purpose         : Try to get a single object from the datastore
            //Re-use          : none
            //Input Parameter : - string serialNr
            //                   - The serialNr of the object to load from the datastore
            //                  - ref object obj
            //                   - The object loaded from the datastore
            //Output Type     : - int
            //                  0 : object loaded from datastore
            //                 -1 : no object was loaded from the datastore (not found)
            //
            int rc = -1;
            SqliteConnection sqlCon = null;
            SqliteCommand sqlCmd = null;
            SqliteDataReader sqlDr = null;
            bool bFound = false;

            try
            {
                if (!query.Equals(""))
                {
                    sqlCon = new SqliteConnection(conStr);
                    sqlCon.Open();
                    //string selectQuery = string.Format("SELECT * FROM Objects WHERE [serialNr] = '{0}'", serialNr);
                    sqlCmd = new SqliteCommand(query, sqlCon);
                    sqlDr = sqlCmd.ExecuteReader();
                    bFound = sqlDr.Read();
                    if (bFound)
                    {
                        obj = ConvDataToObject(sqlDr);


                        rc = 0;
                    } // end if
                    sqlDr.Close();
                    sqlCmd.Dispose();
                    sqlCon.Dispose();
                }
                else
                {
                    Debug.LogError("Please call SetQuery() before calling SelectObject().");
                }

            } // end try
            catch (Exception ex)
            {
                throw ex;
            } // end catch
            return rc;
        } // end method

        public override int Update(Dictionary<object, object> parameters)
        {
            //
            //Name            : int Update(object obj)
            //Purpose         : Try to update a row in the datastore
            //Re-use          : none
            //Input Parameter : object obj
            //                  - The new object data for the row in the datastore
            //Output Type     : - int
            //                  0 : object found and updated successfully
            //                 -1 : object not updated because the record was not found
            //
            int rc = 0;
            SqliteConnection sqlCon = null;
            SqliteCommand sqlCmd = null;

            try
            {
                if (!query.Equals(""))
                {
                    sqlCon = new SqliteConnection(conStr);
                    sqlCon.Open();

                    //updateQuery = string.Format("UPDATE Objects SET [name] = @name, [faction] = @faction, " +
                    //    "[experience] = @experience WHERE [serialNr] = '{0}'", obj.SerialNr);
                    sqlCmd = new SqliteCommand(query, sqlCon);
                    foreach (KeyValuePair<object, object> p in parameters)
                    {
                        sqlCmd.Parameters.AddWithValue(Convert.ToString(p.Key), p.Value);
                    }
                    rc = sqlCmd.ExecuteNonQuery();
                    if (rc == 0)
                    {
                        rc = -1;
                    } // end if
                    else
                    {
                        rc = 0;
                    } // end else
                    sqlCmd.Dispose();
                    sqlCon.Dispose();
                }
                else
                {
                    Debug.LogError("Please call SetQuery() before calling Update().");
                }

            } // end try
            catch (Exception ex)
            {
                throw ex;
            } // end catch
            return rc;
        } // end method
        #endregion
        #region HELPER/PREREQUISITE METHODS
        public void SetQuery(string query)
        {
            this.query = query;
        }

        
        #endregion

        #region DATABASE MANIPULATION HELPER METHODS
        public int CreateTable(string tableName, string columnParameters)
        {
            int rc = -1;
            SqliteConnection sqlCon = null;
            SqliteCommand sqlCmd = null;
            string createQuery = "";
            sqlCon = new SqliteConnection(conStr);
            sqlCon.Open();
            createQuery = "CREATE TABLE IF NOT EXISTS " + tableName + columnParameters;
            Debug.Log(createQuery);

            sqlCmd = new SqliteCommand(createQuery, sqlCon);
            rc = sqlCmd.ExecuteNonQuery();
            sqlCmd.Dispose();
            sqlCon.Dispose();
            try
            {
                
            }//end try
            catch (Exception ex)
            {
                //throw ex;
                Debug.Log("Datastore Creator_Error: " + ex.Message);
            } // end catch



            return rc;
        }
        public int DeleteTable(string tableName)
        {
            int rc = 0;
            SqliteConnection sqlCon = null;
            SqliteCommand sqlCmd = null;
            string createQuery = "";

            try
            {
                sqlCon = new SqliteConnection(conStr);
                sqlCon.Open();
                createQuery = "DROP TABLE IF EXISTS " + tableName + ";";
                sqlCmd = new SqliteCommand(createQuery, sqlCon);
                rc = sqlCmd.ExecuteNonQuery();
                sqlCmd.Dispose();
                sqlCon.Dispose();
            }//end try
            catch (Exception ex)
            {
                //throw ex;
                Debug.Log("Datastore Creator_Error: " + ex.Message);
            } // end catch
            return rc;
        }
        public int AddColumn(string tableName, string columnParameters)
        {
            int rc = 0;
            SqliteConnection sqlCon = null;
            SqliteCommand sqlCmd = null;
            string createQuery = "";

            try
            {
                sqlCon = new SqliteConnection(conStr);
                sqlCon.Open();
                createQuery = "ALTER TABLE " + tableName + " ADD " + columnParameters;
                sqlCmd = new SqliteCommand(createQuery, sqlCon);
                rc = sqlCmd.ExecuteNonQuery();
                sqlCmd.Dispose();
                sqlCon.Dispose();
            }//end try
            catch (Exception ex)
            {
                //throw ex;
                Debug.Log("Datastore Creator_Error: " + ex.Message);
            } // end catch

            return rc;
        }
        public List<string> GetTableNames()
        {
            //
            //Name            : List<string> GetTableNames()
            //Purpose         : Try to get all the table names from the current database
            //Re-use          : none
            //Input Parameter : None        
            //Output Type     : List<string>
            //                 - the string collection that will contain the names of all tables from the database         
            //
            SqliteConnection sqlCon = null;
            List<string> list;
            SqliteCommand sqlCmd = null;
            SqliteDataReader sqlDr = null;


            try
            {
                list = new List<string>();

                sqlCon = new SqliteConnection(conStr);
                sqlCon.Open();
                string selectQuery = "SELECT name FROM sqlite_master WHERE type='table';";
                sqlCmd = new SqliteCommand(selectQuery, sqlCon);
                sqlDr = sqlCmd.ExecuteReader();
                while (sqlDr.Read())
                {
                    string tableName = Convert.ToString(sqlDr["name"]);
                    list.Add(tableName);
                } // end while
                sqlDr.Close();
                sqlCmd.Dispose();
                sqlCon.Dispose();
            } //end try
            catch (Exception ex)
            {
                throw ex;
                //throw ex;
            } // end catch
            return list;
        } // end method

        public List<CandiceColumnInfo> GetColumnInfo(string tableName)
        {

            //
            //Name            : List<ColumnInfo> GetColumnInfo()
            //Purpose         : Try to get all the Column information from the table
            //Re-use          : none
            //Input Parameter : string tableName        
            //Output Type     : List<ColumnInfo>
            //                 - the ColumnInfo collection that will contain the column information from the specified table         
            //
            SqliteConnection sqlCon = null;
            List<CandiceColumnInfo> listColumnInfo = new List<CandiceColumnInfo>();
            SqliteCommand sqlCmd = null;
            SqliteDataReader sqlDr = null;


            try
            {

                sqlCon = new SqliteConnection(conStr);
                sqlCon.Open();
                string selectQuery = "PRAGMA table_info(" + tableName + "); ";
                sqlCmd = new SqliteCommand(selectQuery, sqlCon);
                sqlDr = sqlCmd.ExecuteReader();
                while (sqlDr.Read())
                {
                    string name = Convert.ToString(sqlDr["name"]);
                    string type = Convert.ToString(sqlDr["type"]);
                    string defaultValue = Convert.ToString(sqlDr["dflt_value"]);
                    bool notNull = Convert.ToBoolean(sqlDr["notnull"]);
                    bool pk = Convert.ToBoolean(sqlDr["pk"]);
                    bool ai = false;
                    //bool ai = Convert.ToBoolean(sqlDr["auto"]);
                    CandiceColumnInfo columnInfo = new CandiceColumnInfo(name, type, defaultValue, notNull, pk, ai);
                    listColumnInfo.Add(columnInfo);
                } // end while
                sqlDr.Close();
                sqlCmd.Dispose();
                sqlCon.Dispose();
            } //end try
            catch (Exception ex)
            {
                throw ex;
                //throw ex;
            } // end catch
            return listColumnInfo;
        }

        public void ChangeConnectionString(string conStr)
        {
            this.conStr = conStr;
        }
        #endregion

        private Dictionary<object,object> ConvDataToObject(SqliteDataReader sqlDr)
        {
            //
            //Name            : Dictionary<string,string> ConvDataToObject(SqliteDataReader sqlDr)
            //Purpose         : convert the data stream into a Dictionary object
            //Re-use          : none
            //Input Parameter : SqliteDataReader sqlDr
            //                   - the data reader containing the stream of data to convert
            //Output Type     : - Weapon
            //                 The object that will be used by the user
            //
            Dictionary<object, object> obj = new Dictionary<object, object>();
            try
            {
                List<string> columns = new List<string>();
                for (int i = 0; i < sqlDr.FieldCount; i++)
                {
                    string column = sqlDr.GetName(i);
                    obj.Add(column, Convert.ToString(sqlDr[column]));
                }
            }
            catch (Exception e)
            {
                Debug.LogError("ERROR ConvDataToWeapon(): " + e.Message);
            }


            return obj;
        }
    }
}

