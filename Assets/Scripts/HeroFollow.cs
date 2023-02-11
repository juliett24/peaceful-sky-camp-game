using UnityEngine;
using UnityEngine.InputSystem;


public class HeroFollow : MonoBehaviour
{
    [SerializeField] private ScrapballContents _ball;
    [SerializeField] private Transform _cam;
    [SerializeField] private Transform _follow;

    [Header("Dimensions")]
    [SerializeField] private float _followDistance = 2f;
    [SerializeField] private float _fromGround = 1f;
    [SerializeField] private float _bodyRadius = 0.5f;

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
        UpdatePosition();
        PushOutWalls();

        var interpDelta = 0.1f; // Not precise, TODO: use the precise DT-based formula (if needed)
        // var interpDelta = 1f;
        transform.rotation = Quaternion.Lerp(transform.rotation, _destRotation, interpDelta);
        transform.position = Vector3.Lerp(transform.position, _destPosition, interpDelta);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void UpdatePosition()
    {
        var inputXformed = _cam.TransformDirection(new Vector3(_moveInput.x, 0, _moveInput.y));
        inputXformed = new Vector3(inputXformed.x, 0, inputXformed.z).normalized;

        if (_moveInput != Vector2.zero)
        {
            _destRotation.SetFromToRotation(Vector3.right, inputXformed);
        }
        _destPosition = _follow.position - transform.rotation * new Vector3(_ball.MaxRadius + _followDistance, 0, 0);
    }

    private void PushOutWalls()
    {
        var hitDown = Physics.SphereCast(
            transform.position + Vector3.up * _bodyRadius, _bodyRadius,
            Vector3.down, out var hitDownInfo, 100f);
        if (hitDown)
        {
            transform.position = hitDownInfo.point + Vector3.up * _fromGround;
        }

        var dirToFollow = (_follow.position - transform.position).normalized;
        var hitToward = Physics.SphereCast(
            dirToFollow, _bodyRadius,
            Vector3.down, out var hitTowardInfo, _ball.MaxRadius + _followDistance);
        if (hitToward)
        {
            transform.position = _follow.position + dirToFollow * (_ball.MaxRadius + _followDistance - hitTowardInfo.distance);
        }
    }
}
