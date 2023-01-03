using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(WeaponRecoil))]
public class R4C : Weapon
{
    [SerializeField] private Camera _camera;
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

    protected override void Shoot()
    {
        Ray ray = _camera.ViewportPointToRay(new Vector3(.5f, .5f));
        ray.origin = _camera.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit)) {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(WeaponInfo.Damage);

            Collider[] colliders = Physics.OverlapSphere(hit.point, .3f);

            if (colliders.Length != 0) {
                GameObject bulletImpactObject = Instantiate(
                    WeaponInfo.BulletImpactPrefab, hit.point + hit.normal * 0.001f,
                    Quaternion.LookRotation(hit.normal, Vector3.up) * WeaponInfo.BulletImpactPrefab.transform.rotation
                );

                Destroy(bulletImpactObject, 8f);

                bulletImpactObject.transform.SetParent(colliders[0].transform);
            }
        }

        WeaponRecoil.GenerateRecoil();

        CurrentAmmo--;
    }
}
