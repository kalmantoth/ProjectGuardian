using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGuardian
{
    public enum WeaponType
    {
        TWO_HANDED_SWORD = 0,
        BOW = 1
    }
    public class Weapon
    {
        public int weaponTypeID;
        public WeaponType weaponType;
        public string weaponName;
        public int attackDamage;
        public float attackSpeed;

        public static readonly Weapon[] weapons = new Weapon[] {
        new Weapon(WeaponType.TWO_HANDED_SWORD, "Two Handed Sword", 50, 1f),
        new Weapon(WeaponType.BOW, "Bow", 25, 0.5f)
    };

        public Weapon(WeaponType weaponType, string weaponName, int attackDamage, float attackSpeed)
        {
            this.weaponType = weaponType;
            this.weaponTypeID = (int)this.weaponType;
            this.weaponName = weaponName;
            this.attackDamage = attackDamage;
            this.attackSpeed = attackSpeed;
        }
    }
}

