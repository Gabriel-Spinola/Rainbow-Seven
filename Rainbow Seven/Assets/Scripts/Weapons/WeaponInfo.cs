using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[CreateAssetMenu(menuName = "Loadout/Weapon")]
public class WeaponInfo : ScriptableObject
{
    public GameObject BulletImpactPrefab;

    public float Damage;
    public Optional<float> FireRate;
    public float Mobility;
    public int Capacity;
    public float TacticalReloadTime;
    public float EmptyReloadTime;
}
