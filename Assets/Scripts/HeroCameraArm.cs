using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
public class HeroCameraArm : MonoBehaviour
{
    [SerializeField] private ScrapballContents _ball;
    [SerializeField] private float _camSensitivity = 0.5f;
    [SerializeField] private float _camDistanceBase = 1.5f;
    [SerializeField] private float _camDistanceScale = 0.5f;

    public Vector2 CameraRotation = new Vector2();

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();
        CameraRotation += delta * _camSensitivity;
        CameraRotation.y = Mathf.Clamp(CameraRotation.y, -85f, 85f);
        transform.eulerAngles = new Vector3(-CameraRotation.y, CameraRotation.x, 0f);
    }

    public void OnBallRadiusChanged(float to)
    {
        transform.localScale = (to * _camDistanceScale + _camDistanceBase) * Vector3.one;
    }

    public void OnValidate()
    {
        OnBallRadiusChanged(_ball.ScrapRadius);
    }
}
