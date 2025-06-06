using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item
{
    [Header("Weapon Prefab")]
    public GameObject weaponPrefab;
    [Header("Weapon Base Damage")]
    public int physicsDamage = 0;
    public int magicDamage = 0;
    [Header("Stamina Cost")]
    public int baseStaminaCost = 0;

    [Header("Weapon Base Poise Damage")]

    public int basePoiseDamage = 10;

}
