using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private GameObject _playerHead;
    [SerializeField] private CinemachineRecomposer _cinemachineRecomposer;
    [SerializeField] private CinemachineCameraOffset _cinemachineOffset;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 10f;

    private CharacterController _controller;
    private InputManager _inputs;
    private CinemachineVirtualCamera _cinemachineCamera;
    private CinemachinePOV _pov;

    private Vector3 _movement;
    private Vector3 _playerVelocity;

    private void Awake()
    {
        _inputs = InputManager.Instance;

        _controller = GetComponent<CharacterController>();

        _cinemachineCamera = FindObjectOfType<CinemachineVirtualCamera>();
        _pov = _cinemachineCamera.GetCinemachineComponent<CinemachinePOV>();
    }

    void Update()
    {
        Movement();
        Leaning();
    }

    private void Movement()
    {
        if (_controller.isGrounded && _playerVelocity.y < 0f) {
            _playerVelocity.y = 0f;
        }

        _playerVelocity.x = _inputs.PlayerSprint ? _sprintSpeed : _moveSpeed;

        _movement = new Vector3(_inputs.MoveDir.x, 0f, _inputs.MoveDir.y);
        _movement = transform.forward * _movement.z + transform.right * _movement.x + transform.up * _playerVelocity.y;

        transform.rotation = Quaternion.Euler(transform.rotation.x, _pov.m_HorizontalAxis.Value, transform.rotation.z);

        _controller.Move(_playerVelocity.x * Time.deltaTime * _movement);
        _playerVelocity.y += Physics.gravity.y * Time.deltaTime;
    }

    private void Leaning()
    {
        if (_inputs.LeanLeft) {
            _cinemachineRecomposer.m_Dutch += Mathf.Lerp(0f, 30f, 1);
            _cinemachineOffset.m_Offset.x = -.15f;
        }
        
        if (_inputs.LeanRight) {
            _cinemachineRecomposer.m_Dutch = Mathf.Lerp(_cinemachineRecomposer.m_Dutch, -30f, 0.1f);
            _cinemachineOffset.m_Offset.x = .15f;
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"Damage Taken: {damage}");
    }
}  
