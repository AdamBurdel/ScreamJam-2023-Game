using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CandiceAIforGames.Data
{
    public abstract class CandiceProviderBase
    {
        //
        // Program : VGS Cross Platform SaveSystem
        // Programmer: M.N Natuyamba
        // Name : ProviderBase
        // Purpose : This class implements the provider base class for the Object classes
        //
        /// <summary>
        /// This method gets the list of all the business objects from the datastore.
        /// It returns the list of business objects
        ///  </summary>
        public abstract List<object> SelectAll();
        /// <summary>
        /// This method gets a single object from the datastore.
        /// It returns 0 to indicate the object was loaded from datastore, or
        /// -1 to indicate that no object was loaded from the datastore (not found).
        /// </summary>
        /// <param name="serialNr">The serialNr of the object to load from the datastore.</param>
        /// <param name="object">The object loaded from the datastore.</param>

        public abstract int SelectObject(ref Dictionary<object, object> obj, string serialNr);
        /// <summary>
        /// This method inserts a record in the datastore.
        /// It returns 0 to indicate the object was inserted into datastore, or
        /// -1 to indicate the object was not inserted because a duplicate was found
        /// </summary>
        /// <param name="obj">The object to add to the datastore.</param>
        public abstract int Insert(Dictionary<object,object> newObj);
        /// <summary>
        /// This method updates a record in the datastore.
        /// It returns 0 to indicate the object was found and updated successfully, or
        /// -1 to indicate the object was not updated because the record was not found
        /// </summary>
        /// <param name="newObj">The new object data for the record in the datastore.</param>

        public abstract int Update(Dictionary<object, object> obj);
        /// <summary>
        /// This method deletes a record in the datastore.
        /// It returns 0 to indicate the object was found and deleted successfully, or
        /// -1 to indicate the object was not deleted because the record was not found
        /// </summary>
        /// <param name="serialNr">The object serialNr of the object to delete in the datastore.</param>

        public abstract int Delete(string serialNr);
    }
}

