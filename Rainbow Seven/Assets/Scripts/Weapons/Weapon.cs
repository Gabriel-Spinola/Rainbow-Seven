using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponInfo _weaponInfo;

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

    protected abstract void Shoot();
}
