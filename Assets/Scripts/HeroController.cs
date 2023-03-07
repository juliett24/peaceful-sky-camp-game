using UnityEngine;
using UnityEngine.InputSystem;


public class HeroController : MonoBehaviour
{
    [SerializeField] private float _linearAccel = 30f;
    [SerializeField] private float _angularAccel = 90f;
    [SerializeField] private Rigidbody _body;
    [SerializeField] private Transform _cam;

    private Vector2 _moveInput;

    public void Update()
    {
        var camRotation = Quaternion.Euler(0, _cam.rotation.eulerAngles.y, 0);
        _body.velocity += Time.deltaTime * (camRotation * new Vector3(_moveInput.x, 0, _moveInput.y) * _linearAccel);
        _body.angularVelocity += Time.deltaTime * (camRotation * new Vector3(_moveInput.y, 0, -_moveInput.x) * _angularAccel);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }
}
