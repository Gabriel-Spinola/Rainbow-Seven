using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(WeaponRecoil))]
public class R4C : Weapon
{
    [SerializeField] private CinemachineVirtualCamera _cinemachineCamera;

    private float _nextShoot;
    private float _fireRateCalculated;

    protected sealed override void Start()
    {
        base.Start();

        Debug.Assert(WeaponInfo.FireRate.Enabled, "R4-C is an automatic weapon, for this reason the FireRate Optional float must be enabled");
        _fireRateCalculated = WeaponInfo.FireRate.Value / 60f;
    }

    protected sealed override void Update()
    {
        base.Update();

       if (Input.ShootHold && CurrentAmmo > 0 && !IsReloading) {
           if (Time.time >= _nextShoot) {
               Shoot();

               _nextShoot = Time.time + 1f / _fireRateCalculated;
           }
       }
    }
}
