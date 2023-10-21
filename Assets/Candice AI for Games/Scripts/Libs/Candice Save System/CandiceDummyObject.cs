using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CandiceAIforGames.Data
{
    public class CandiceDummyObject
    {
        private readonly string sSerialNr;
        private string sName;
        private string sFaction;
        private int iExperience;

        public CandiceDummyObject(string sSerialNr, string sName, string sFaction, int iExperience)
        {
            this.sSerialNr = sSerialNr;
            this.sName = sName;
            this.sFaction = sFaction;
            this.iExperience = iExperience;
        }

        public string SerialNr { get => sSerialNr; }
        public string Name { get => sName; set => sName = value; }
        public string Faction { get => sFaction; set => sFaction = value; }
        public int Experience { get => iExperience; set => iExperience = value; }
    }
}
