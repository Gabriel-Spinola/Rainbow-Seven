using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[CreateAssetMenu(menuName = "Loadout/Weapon")]
public class WeaponInfo : ScriptableObject
{
    public float Damage;
    public Optional<float> FireRate;
    public float Mobility;
    public int Capacity;
}
