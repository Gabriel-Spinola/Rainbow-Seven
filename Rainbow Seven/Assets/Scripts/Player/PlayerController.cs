using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using System.Threading.Tasks;

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
    [SerializeField] private float _leanAngle = 25f;
    [SerializeField] private float _leanTime = .5f;

    private CharacterController _controller;
    private InputManager _inputs;
    private CinemachineVirtualCamera _cinemachineCamera;
    private CinemachinePOV _pov;

    private Vector3 _movement;
    private Vector3 _playerVelocity;

    private float _currentLeanAngle = 0;
    private bool _isLeaningLeft = false;
    private bool _isLeaningRight = false;
    private bool _isLeaningBack = false;

    private void Awake()
    {
        _inputs = InputManager.Instance;

        _controller = GetComponent<CharacterController>();

        _cinemachineCamera = FindObjectOfType<CinemachineVirtualCamera>();
        _pov = _cinemachineCamera.GetCinemachineComponent<CinemachinePOV>();
    }

    private void Start()
    {
        _currentLeanAngle = 0f;
    }

    void Update()
    {
        Movement();
        Leaning();

        Debug.Log($"Only Debug {Mathf.Lerp(0f, _leanAngle, _leanTime)}");
        Debug.Log($"Current Lean Angle: {_currentLeanAngle}");
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
        _playerHead.transform.rotation = Quaternion.Euler(_pov.m_VerticalAxis.Value, transform.rotation.x, transform.rotation.z);
       // _playerHead.transform.position = new Vector3()

        _controller.Move(_playerVelocity.x * Time.deltaTime * _movement);
        _playerVelocity.y += Physics.gravity.y * Time.deltaTime;
    }

    private void Leaning()
    {
        if ((_isLeaningLeft && _inputs.LeanLeft) || (_isLeaningRight && _inputs.LeanRight)) {
            _isLeaningLeft = false;
            _isLeaningRight = false;

            _isLeaningBack = true;
        }

        if (_isLeaningBack) {
            _cinemachineRecomposer.m_Dutch = Mathf.Lerp(_cinemachineRecomposer.m_Dutch, 0f, _leanTime);
            _cinemachineOffset.m_Offset.x = Mathf.Lerp(_cinemachineOffset.m_Offset.x, 0f, .8f);

            if (_cinemachineRecomposer.m_Dutch >= -1 && _cinemachineRecomposer.m_Dutch <= 1) {
                _isLeaningBack = false;
            }
        }

        if (_inputs.LeanLeft && !_isLeaningBack) {
            _isLeaningLeft = true;
            _isLeaningRight = false;
        } 
        
        if (_inputs.LeanRight && !_isLeaningBack) {
            _isLeaningRight = true;
            _isLeaningLeft = false;
        }

        if (_isLeaningLeft) {
            if (_cinemachineRecomposer.m_Dutch >= _leanAngle) {
                _isLeaningLeft = false;

                return;
            }

            _cinemachineRecomposer.m_Dutch = Mathf.Lerp(_cinemachineRecomposer.m_Dutch, _leanAngle, _leanTime);
            _cinemachineOffset.m_Offset.x = Mathf.Lerp(_cinemachineOffset.m_Offset.x, -.15f, .8f);
        }
        
        if (_isLeaningRight) {
            if (_cinemachineRecomposer.m_Dutch <= -_leanAngle) {
                _isLeaningRight = false;

                return;
            }

            _cinemachineRecomposer.m_Dutch = Mathf.Lerp(_cinemachineRecomposer.m_Dutch, -_leanAngle, _leanTime);
            _cinemachineOffset.m_Offset.x = Mathf.Lerp(_cinemachineOffset.m_Offset.x, .15f, .8f);
        }
    }

    private void Lean()
    {

    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"Damage Taken: {damage}");
    }
}  
