using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class WeaponRecoil : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _cinemachineCamera;

    private CinemachineImpulseSource _cameraShake;
    private CinemachinePOV _pov;
    private WeaponInfo _weaponInfo;

    private float _recoilTime;

    private void Awake()
    {
        _weaponInfo = GetComponentInParent<IWeapon>().WeaponInfo;
        _cameraShake = GetComponent<CinemachineImpulseSource>();
        _pov = _cinemachineCamera.GetCinemachineComponent<CinemachinePOV>();
    }

   private void Update()
    {
        if (_recoilTime > 0) {
            _pov.m_VerticalAxis.Value -= (_weaponInfo.VerticalRecoil * Time.deltaTime) / _weaponInfo.RecoilDuration;
            _pov.m_HorizontalAxis.Value -= (_weaponInfo.HorizontalRecoil * Time.deltaTime) / _weaponInfo.RecoilDuration;
            _recoilTime -= Time.deltaTime;
        }
    }

    public void GenerateRecoil()
    {
        _recoilTime = _weaponInfo.RecoilDuration;
        _cameraShake.GenerateImpulse(Camera.main.transform.forward);
    }
}
