using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomExtensions;
using System.Threading.Tasks;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private GameObject _playerHead;
    [SerializeField] private CinemachineRecomposer _cinemachineRecomposer;
    [SerializeField] private CinemachineCameraOffset _cinemachineOffset;
    [SerializeField] private GameObject _weapon;
    [SerializeField] private Animator _animator;
    [SerializeField] private CinemachineVirtualCamera _cinemachineCamera;
    [SerializeField] private Weapon[] _weapons;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 10f;

    [Header("Leaning")]
    [SerializeField] private float _leanAngle = 25f;
    [SerializeField] private float _leanTime = .5f;
    [SerializeField] private float _leanOffset = .15f;
    [SerializeField] private float _leanOffsetTime = .8f;

    public static CinemachinePOV Pov;

    private CharacterController _controller;
    private InputManager _inputs;
    private PhotonView _photonView;

    private Vector3 _movement;
    private Vector3 _playerVelocity;

    private int _itemIndex;
    private int _previusItemIndex = -1;

    private float _currentLeanAngle = 0f;
    private float _currentLeanOffset = 0f;

    private bool _isLeaningLeft = false;
    private bool _isLeaningRight = false;
    private bool _isLeaningBack = false;

    private void Awake()
    {
        _inputs = InputManager.Instance;

        _controller = GetComponent<CharacterController>();
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!_photonView.IsMine) {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(_controller);

            return;
        }

        EquipItem(0);

        _cinemachineCamera = Instantiate(_cinemachineCamera.gameObject).GetComponent<CinemachineVirtualCamera>();

        Pov = _cinemachineCamera.GetCinemachineComponent<CinemachinePOV>();
        _cinemachineCamera.Follow = _playerHead.transform;
    }

    void Update()
    {
        if (!_photonView.IsMine)
            return; 

        _cinemachineRecomposer.m_Dutch = _currentLeanAngle;
        _cinemachineOffset.m_Offset.x = _currentLeanOffset;

        Leaning();

        Vector3 a = new Vector3(0.2965603f, -0.145391f, 0.5267898f);
        Vector3 b = new Vector3(0f, 0.07131457f, 0.09836382f);

        _animator.SetBool("isAiming", _inputs.AimKey);

        for (int i = 0; i < _weapons.Length; i++) {
            if (Input.GetKeyDown((i + 1).ToString())) {
                EquipItem(i);

                break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_photonView.IsMine)
            return;

        Movement();
    }

    private void Movement()
    {
        if (Pov == null)
            return;

        if (_controller.isGrounded && _playerVelocity.y < 0f) {
            _playerVelocity.y = 0f;
        }

        _playerVelocity.x = _inputs.PlayerSprint ? _sprintSpeed : _moveSpeed;

        _movement = new Vector3(_inputs.MoveDir.x, 0f, _inputs.MoveDir.y);
        _movement = transform.forward * _movement.z + transform.right * _movement.x + transform.up * _playerVelocity.y;

        _cinemachineCamera.transform.rotation = Quaternion.Euler(Pov.m_VerticalAxis.Value, Pov.m_HorizontalAxis.Value, _currentLeanAngle);

        transform.rotation = Quaternion.Euler(transform.rotation.x, Pov.m_HorizontalAxis.Value, transform.rotation.z);
        _playerHead.transform.SetPositionAndRotation(
            position: new Vector3(transform.position.x + _currentLeanOffset, _playerHead.transform.position.y, transform.position.z),
            rotation: Quaternion.Euler(Pov.m_VerticalAxis.Value, Pov.m_HorizontalAxis.Value, _currentLeanAngle)
        );

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

    private void EquipItem(int index)
    {
        _itemIndex = index;

        _weapons[_itemIndex].gameObject.SetActive(true);

        if (_previusItemIndex != -1) {
            _weapons[_previusItemIndex].gameObject.SetActive(false);
        }

        _previusItemIndex = _itemIndex;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"Damage Taken: {damage}");
    }
}  
