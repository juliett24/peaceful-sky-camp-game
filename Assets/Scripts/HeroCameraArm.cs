using UnityEngine;
using UnityEngine.InputSystem;


public class HeroCameraArm : MonoBehaviour
{
    [SerializeField] private ScrapballContents _ball;
    [SerializeField] private float _camSensitivity = 0.5f;

    public Vector2 CameraRotation = new Vector2();

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();
        CameraRotation += delta * _camSensitivity;
        CameraRotation.y = Mathf.Clamp(CameraRotation.y, -90f, 90f);
        transform.eulerAngles = new Vector3(
            -CameraRotation.y,
            CameraRotation.x,
            0f
        );
    }
}
