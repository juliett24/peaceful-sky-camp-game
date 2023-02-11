using UnityEngine;
using UnityEngine.InputSystem;


public class HeroFollow : MonoBehaviour
{
    [SerializeField] private Transform _follow;
    [SerializeField] private float _followDistance = 20f;
    [SerializeField] private ScrapballContents _ball;

    private Vector2 _moveInput;
    private Vector3 _destPosition;
    private Quaternion _destRotation;

    public void Awake()
    {
        _destPosition = transform.position;
        _destRotation = transform.rotation;
    }

    public void Update()
    {
        var ratio = 0.1f; //Not precise, TODO: use the precise DT-based formula (if needed)

        _destRotation.SetFromToRotation(Vector3.right, new Vector3(_moveInput.x, 0, _moveInput.y));
        transform.rotation = Quaternion.Lerp(transform.rotation, _destRotation, ratio);

        var pos1 = transform.position;
        _destPosition = _follow.position;
        transform.position = Vector3.Lerp(pos1, _destPosition, ratio)
            + _destRotation * new Vector3(_ball.CoreRadius + _followDistance, 0, 0);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }
}
