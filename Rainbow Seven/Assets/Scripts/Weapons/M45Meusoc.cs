using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M45Meusoc : Weapon
{
    [SerializeField] private Camera _camera;

    protected sealed override void Update()
    {
        base.Update();

        if (Input.ShootTap && CurrentAmmo > 0 && !IsReloading) {
            Shoot();
        }
    }
}
