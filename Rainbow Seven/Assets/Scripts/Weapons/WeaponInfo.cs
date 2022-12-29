using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[CreateAssetMenu(menuName = "Loadout/Weapon")]
public class WeaponInfo : ScriptableObject
{
    [Header("Refs")]
    public GameObject BulletImpactPrefab;

    [Header("Base Stats")]
    public float Damage;
    public Optional<float> FireRate;
    public float Mobility;
    public int Capacity;

    [Header("Realoding")]
    public float TacticalReloadTime;
    public float EmptyReloadTime;

    [Header("Recoil")]
    [Range(0f, .4f)]
    public float RecoilDuration;
    public Vector2[] RecoilPattern;
}
