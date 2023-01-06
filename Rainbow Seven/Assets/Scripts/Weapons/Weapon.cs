using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem.HID;

[RequireComponent(typeof(WeaponRecoil), typeof(PhotonView))]  
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

    private PhotonView _photonView;

    protected virtual void Awake()
    {
        Input = InputManager.Instance;

        WeaponRecoil = GetComponent<WeaponRecoil>();
        _photonView = GetComponent<PhotonView>();
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
        if (!_photonView.IsMine)
            return;

        Ray ray = _camera.ViewportPointToRay(new Vector3(.5f, .5f));
        ray.origin = _camera.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit)) {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(WeaponInfo.Damage);
            Debug.Log(hit.collider.gameObject.name);

            //_photonView.RPC(nameof(RPC_Shoot), RpcTarget.All, hit.point, hit.normal);
        }

        WeaponRecoil.GenerateRecoil();

        CurrentAmmo--;
    }
}
