using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.InputSystem;

public class R4C : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo _weaponInfo;

    private int _currentAmmo;
    private float _nextShoot;
    private float _fireRateCalculated;

    private InputManager _input;

    // Start is called before the first frame update
    void Start()
    {
        _currentAmmo = _weaponInfo.Capacity;

        Debug.Assert(!_weaponInfo.FireRate.Enabled, "R4-C is an automatic weapon, for this reason the FireRate Optional float must be enabled");
        _fireRateCalculated = _weaponInfo.FireRate.Value / 60f;
    }

    // Update is called once per frame
    void Update()
    {
       // if (inputAction.ShootHold.ReadValue<float>() > 0f) {
            if (Time.time >= _nextShoot) {
                Shoot();

                _nextShoot = Time.time + 1f / _fireRateCalculated;
            }
        //}
    }

    public void Shoot()
    {
        
    }

    public void Reload() => throw new System.NotImplementedException();
}
