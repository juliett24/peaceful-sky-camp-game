using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
public class HeroCameraArm : MonoBehaviour
{
    [SerializeField] private ScrapballContents _ball;
    [SerializeField] private LayerMask _pushOutLayers;
    [SerializeField] private float _pushOutDistance = 0.5f;
    [SerializeField] private float _pushOutFreeRadius = 0.5f;
    [SerializeField] private float _camSensitivity = 0.5f;
    [SerializeField] private float _camDistanceBase = 1.5f;
    [SerializeField] private float _camDistanceScale = 0.5f;

    public Vector2 CameraRotation = new Vector2();

    private float _camDestinationDistance = 1f;
    private float _raycastSourceDistance = 1f;
    private Vector3 _camDirection;


    public void Start()
    {
       Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();
        CameraRotation += delta * _camSensitivity;
        CameraRotation.y = Mathf.Clamp(CameraRotation.y, -85f, 85f);
        _camDirection = Quaternion.Euler(-CameraRotation.y, CameraRotation.x, 0f) * Vector3.back;
    }

    public void OnBallRadiusChanged(float to)
    {
        _camDestinationDistance = to * _camDistanceScale + _camDistanceBase;
        _raycastSourceDistance = to + 1f;
    }

    public void OnValidate()
    {
        OnBallRadiusChanged(_ball.ScrapRadius);
        _camDirection = Quaternion.Euler(-CameraRotation.y, CameraRotation.x, 0f) * Vector3.back;
    }

    private void Update()
    {
        if (Physics.SphereCast(
            _ball.transform.position + _camDirection * _raycastSourceDistance,
            _pushOutFreeRadius,
            _camDirection,
            out var hitInfo,
            _camDestinationDistance - _raycastSourceDistance + _pushOutDistance,
            _pushOutLayers))
        {
            transform.position = hitInfo.point - _camDirection * _pushOutDistance;
        }
        else
        {
            transform.position = _ball.transform.position + _camDirection * _camDestinationDistance;
        }
    }
}
