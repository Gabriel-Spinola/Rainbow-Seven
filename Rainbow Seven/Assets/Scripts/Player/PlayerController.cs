using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomExtensions;
using System.Threading.Tasks;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private GameObject _playerHead;
    [SerializeField] private CinemachineRecomposer _cinemachineRecomposer;
    [SerializeField] private CinemachineCameraOffset _cinemachineOffset;
    [SerializeField] private GameObject _weapon;
    [SerializeField] private Animator _animator;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 10f;

    [Header("Leaning")]
    [SerializeField] private float _leanAngle = 25f;
    [SerializeField] private float _leanTime = .5f;
    [SerializeField] private float _leanOffset = .15f;
    [SerializeField] private float _leanOffsetTime = .8f;

    private CharacterController _controller;
    private InputManager _inputs;
    private CinemachineVirtualCamera _cinemachineCamera;
    private CinemachinePOV _pov;

    private Vector3 _movement;
    private Vector3 _playerVelocity;

    private float _currentLeanAngle = 0f;
    private float _currentLeanOffset = 0f;
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

    void Update()
    {
        _cinemachineRecomposer.m_Dutch = _currentLeanAngle;
        _cinemachineOffset.m_Offset.x = _currentLeanOffset;

        Leaning();

        Vector3 a = new Vector3(0.2965603f, -0.145391f, 0.5267898f);
        Vector3 b = new Vector3(0f, 0.07131457f, 0.09836382f);

        _animator.SetBool("isAiming", _inputs.AimKey);
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
        _playerHead.transform.SetPositionAndRotation(
            position: new Vector3(transform.position.x + _currentLeanOffset, _playerHead.transform.position.y, transform.position.z),
            rotation: Quaternion.Euler(_pov.m_VerticalAxis.Value, _pov.m_HorizontalAxis.Value, _currentLeanAngle)
        );

        _controller.Move(_playerVelocity.x * Time.deltaTime * _movement);
        _playerVelocity.y += Physics.gravity.y * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Leaning()
    {
        if ((_isLeaningLeft && _inputs.LeanLeft) || (_isLeaningRight && _inputs.LeanRight)) {
            _isLeaningLeft = false;
            _isLeaningRight = false;

            _isLeaningBack = true;
        }

        if (_isLeaningBack) {
            Lean(0f, 0f);

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

            Lean(_leanAngle, -_leanOffset);
        }
        
        if (_isLeaningRight) {
            if (_cinemachineRecomposer.m_Dutch <= -_leanAngle) {
                _isLeaningRight = false;

                return;
            }

            Lean(-_leanAngle, _leanOffset);
        }
    }

    private void Lean(float leanAngle, float offset)
    {
        _currentLeanAngle = Mathf.Lerp(_cinemachineRecomposer.m_Dutch, leanAngle, _leanTime);
        _currentLeanOffset = Mathf.Lerp(_cinemachineOffset.m_Offset.x, offset, _leanOffsetTime);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"Damage Taken: {damage}");
    }
}  
