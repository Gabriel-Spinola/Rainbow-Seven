using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class WeaponRecoil : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _cinemachineCamera;
    [SerializeField] private Camera _camera;

    private CinemachineImpulseSource _cameraShake;
    private WeaponInfo _weaponInfo;

    private float _recoilTime;
    private int _index;

    private float _verticalRecoil;
    private float _horizontalRecoil;

    private void Awake()
    {
        _weaponInfo = GetComponentInParent<IWeapon>().WeaponInfo;
        _cameraShake = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        if (_recoilTime > 0) {
            PlayerController.Pov.m_VerticalAxis.Value -= (_verticalRecoil * Time.deltaTime) / _weaponInfo.RecoilDuration;
            PlayerController.Pov.m_HorizontalAxis.Value -= (_horizontalRecoil * Time.deltaTime) / _weaponInfo.RecoilDuration;
            _recoilTime -= Time.deltaTime;
        }
    }

    public void GenerateRecoil()
    {
        _recoilTime = _weaponInfo.RecoilDuration;

        _horizontalRecoil = _weaponInfo.RecoilPattern[_index].x;
        _verticalRecoil = _weaponInfo.RecoilPattern[_index].y;

        _index = NextIndex(_index);

        _cameraShake.GenerateImpulse(_cinemachineCamera.transform.forward);
    }

    public void ResetRecoil() => _index = 0;

    private int NextIndex(int index) => (index + 1) % _weaponInfo.RecoilPattern.Length;
}
