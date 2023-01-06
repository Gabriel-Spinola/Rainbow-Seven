using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponInfo _weaponInfo;
    [SerializeField] private Camera _camera;

    public WeaponInfo WeaponInfo { get => _weaponInfo; }

    protected InputManager Input;
    protected WeaponRecoil WeaponRecoil;

    protected bool IsReloading;
    protected int CurrentAmmo;

    protected virtual void Awake()
    {
        Input = InputManager.Instance;

        WeaponRecoil = GetComponent<WeaponRecoil>();
    }

    protected virtual void Start()
    {
        CurrentAmmo = WeaponInfo.Capacity;
    }

    protected virtual void Update()
    {
        if (Input.ShootTap) {
            WeaponRecoil.ResetRecoil();
        }

        if (CurrentAmmo <= 0 && Input.ShootHold) {
            StartCoroutine(Reload(WeaponInfo.EmptyReloadTime));
        }

        if (Input.ReloadKey && CurrentAmmo <= WeaponInfo.Capacity && !IsReloading) {
            StartCoroutine(Reload(WeaponInfo.TacticalReloadTime));
        }
    }

    protected virtual IEnumerator Reload(float time)
    {
        IsReloading = true;

        Debug.Log("Reloading");

        yield return new WaitForSeconds(time);

        IsReloading = false;
        CurrentAmmo = _weaponInfo.Capacity;
    }

    protected virtual void Shoot()
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
