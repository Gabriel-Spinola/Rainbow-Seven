using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon 
{
    WeaponInfo WeaponInfo { get; }

    void Shoot();
    IEnumerator Reload(float time);
}
