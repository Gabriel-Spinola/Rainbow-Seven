using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.InputSystem;
using Cinemachine;

public class R4C : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo _weaponInfo;
    [SerializeField] private Camera _camera;
    [SerializeField] private CinemachineVirtualCamera _cinemachineCamera;

    private InputManager _input;
    private CinemachinePOV _pov;

    private int _currentAmmo;
    private float _nextShoot;
    private float _fireRateCalculated;
    private float _recoilTime;

    private bool _isReloading;

    private void Awake()
    {
        _input = InputManager.Instance;

        _pov = _cinemachineCamera.GetCinemachineComponent<CinemachinePOV>();
    }

    void Start()
    {
        _currentAmmo = _weaponInfo.Capacity;

        Debug.Assert(_weaponInfo.FireRate.Enabled, "R4-C is an automatic weapon, for this reason the FireRate Optional float must be enabled");
        _fireRateCalculated = _weaponInfo.FireRate.Value / 60f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentAmmo <= 0 && _input.ShootHold) {
            StartCoroutine(Reload(_weaponInfo.EmptyReloadTime));
        }

        if (_input.ReloadKey && _currentAmmo <= _weaponInfo.Capacity && !_isReloading) {
            StartCoroutine(Reload(_weaponInfo.TacticalReloadTime));
        }

       if (_input.ShootHold && _currentAmmo > 0 && !_isReloading) {
           if (Time.time >= _nextShoot) {
               Shoot();

               _nextShoot = Time.time + 1f / _fireRateCalculated;
           }
       }

        if (_recoilTime > 0) {
            _pov.m_VerticalAxis.Value -= (_weaponInfo.VerticalRecoil * Time.deltaTime) / _weaponInfo.VerticalRecoilDuration;
            _recoilTime -= Time.deltaTime;
        }
    }

    public void Shoot()
    {
        Ray ray = _camera.ViewportPointToRay(new Vector3(.5f, .5f));
        ray.origin = _camera.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit)) {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(_weaponInfo.Damage);

            Collider[] colliders = Physics.OverlapSphere(hit.point, .3f);

            if (colliders.Length != 0) {
                GameObject bulletImpactObject = Instantiate(
                    _weaponInfo.BulletImpactPrefab, hit.point + hit.normal * 0.001f,
                    Quaternion.LookRotation(hit.normal, Vector3.up) * _weaponInfo.BulletImpactPrefab.transform.rotation
                );

                Destroy(bulletImpactObject, 8f);

                bulletImpactObject.transform.SetParent(colliders[0].transform);
            }
        }

        GenerateRecoil();

        _currentAmmo--;
    }

    public IEnumerator Reload(float time)
    {
        _isReloading = true;

        Debug.Log("Reloading");

        yield return new WaitForSeconds(time);

        _isReloading = false;
        _currentAmmo = _weaponInfo.Capacity;
    }

    private void GenerateRecoil()
    {
        _recoilTime = _weaponInfo.VerticalRecoilDuration;
    }
}
