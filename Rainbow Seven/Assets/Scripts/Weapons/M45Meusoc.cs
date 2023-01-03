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

    protected sealed override void Shoot()
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
