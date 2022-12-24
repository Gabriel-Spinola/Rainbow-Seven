using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    private static InputManager s_instance;

    public static InputManager Instance { get => s_instance; }

    public MainInputActions InputActions { private set; get; }

    [Header("Player Movement")]
    [SerializeField] private Vector2 _moveDir;
    [SerializeField] private Vector2 _mouseDelta;

    [SerializeField] private bool _playerJumped;
    [SerializeField] private bool _playerSprint;
    
    public Vector2 MoveDir => _moveDir;
    public Vector2 MouseDelta => _mouseDelta;

    public bool PlayerJumped => _playerJumped;
    public bool PlayerSprint => _playerSprint;

    private void Awake()
    {
        if (s_instance != null && s_instance != this) {
            Destroy(this.gameObject);
        }
        else {
            s_instance = this;
        }

        InputActions = new MainInputActions();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable() => InputActions.Enable();

    private void OnDisable() => InputActions.Disable();

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.RightAlt)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Cursor.lockState is CursorLockMode.Locked && Input.GetKeyDown(KeyCode.LeftAlt)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Cursor.lockState is CursorLockMode.None && Input.GetKeyDown(KeyCode.LeftAlt)) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
#endif

        _moveDir = InputActions.Player.Move.ReadValue<Vector2>();
        _mouseDelta = InputActions.Player.Look.ReadValue<Vector2>();

        _playerJumped = InputActions.Player.Jump.triggered;
        _playerSprint = InputActions.Player.Sprint.triggered;
    }
}
