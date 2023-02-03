using UnityEngine;
using UnityEngine.InputSystem;


public class HeroController : MonoBehaviour
{
    [SerializeField] private float _camSensitivity = 0.5f;
    [SerializeField] private float _moveSpeed = 30f;
    [SerializeField] private Rigidbody _body;
    [SerializeField] private Transform _cam;

    private Vector2 _moveInput;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        _body.velocity += Time.deltaTime * _cam.TransformDirection(new Vector3(_moveInput.x, 0, _moveInput.y)) * _moveSpeed;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();
        _cam.Rotate(new Vector3(-delta.y, delta.x, 0) * _camSensitivity);
        var clampedRotation = _cam.localRotation;
        clampedRotation.x = Mathf.Clamp(clampedRotation.x, -80, 80);
        _cam.localRotation = clampedRotation;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }
}
