using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CandiceAIforGames.Data
{
    public class CandiceWeapon
    {
        private int weaponID;
        private string weaponName;
        private string weaponType;
        private double weaponDamage;

        public CandiceWeapon(int weaponID, string weaponName, string weaponType, double weaponDamage)
        {
            this.weaponID = weaponID;
            this.weaponName = weaponName;
            this.weaponType = weaponType;
            this.weaponDamage = weaponDamage;
        }
        public CandiceWeapon(Dictionary<string, object> dict)
        {
            WeaponName = dict["WPN_NAME"].ToString();
            WeaponType = dict["WPN_TYPE"].ToString();
            WeaponDamage = (double)dict["WPN_DAMAGE"];
        }

        public int WeaponID { get => weaponID; }
        public string WeaponName { get => weaponName; set => weaponName = value; }
        public string WeaponType { get => weaponType; set => weaponType = value; }
        public double WeaponDamage { get => weaponDamage; set => weaponDamage = value; }

    }

}
