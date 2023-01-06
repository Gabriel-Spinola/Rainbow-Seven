using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M45Meusoc : Weapon
{
    protected sealed override void Update()
    {
        base.Update();

        if (Input.ShootTap && CurrentAmmo > 0 && !IsReloading) {
            base.Shoot();
        }
    }
}
