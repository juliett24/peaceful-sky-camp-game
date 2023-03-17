using UnityEngine;
using UnityEngine.InputSystem;


public class HeroFollow : MonoBehaviour
{
    [SerializeField] private ScrapballContents _ball;
    [SerializeField] private Transform _model;
    [SerializeField] private Transform _cam;
    [SerializeField] private Transform _follow;

    [Header("Dimensions")]
    [SerializeField] private Vector2 _inputRotation = new Vector2(20f, 10f);
    [SerializeField] private Vector2 _inputTilt = new Vector2(30f, 15f);
    [SerializeField] private float _followDistance = 2f;
    [SerializeField] private float _fromGround = 1f;
    [SerializeField] private float _maxBallVerticalDifference = 4f;
    [SerializeField] private float _bodyRadius = 0.5f;
    [SerializeField] private LayerMask _layerMask = 0;

    private Vector2 _moveInput;
    private Quaternion _destRotation;
    private Quaternion _modelDestRotation;
    private Vector3 _groundNormal;

    public void Awake()
    {
        _destRotation = transform.rotation;
    }

    public void FixedUpdate()
    {
        UpdatePosition();
        PushOutWalls();

        var interpDelta = 0.1f; // Not precise, TODO: use the precise DT-based formula (if needed)
        transform.rotation = Quaternion.Lerp(transform.rotation, _destRotation, interpDelta);
        _model.localRotation = Quaternion.Lerp(_model.localRotation, _modelDestRotation * Quaternion.FromToRotation(Vector3.up, Quaternion.Inverse(transform.rotation) * _groundNormal), interpDelta);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void UpdatePosition()
    {
        var camRotation = _cam.rotation.eulerAngles;
        camRotation.x = 0f;
        _destRotation = Quaternion.Euler(camRotation) * Quaternion.Euler(0, _moveInput.x * _inputRotation.x - 90f, -_moveInput.y * _inputRotation.y);
        _modelDestRotation = Quaternion.Euler(_moveInput.x * _inputTilt.x, 0f, -_moveInput.y * _inputTilt.y);
        transform.position = _follow.position - transform.rotation * new Vector3(_ball.ScrapRadius + _followDistance, 0, 0);
    }

    private void PushOutWalls()
    {
        // var dirToFollow = (_follow.position - transform.position).normalized;
        // var hitToward = Physics.SphereCast(
        //     transform.position, _bodyRadius,
        //     dirToFollow, out var hitTowardInfo, _ball.ScrapRadius + _followDistance, _layerMask);
        // if (hitToward)
        // {
        //     transform.position = _follow.position + dirToFollow * (_ball.ScrapRadius + _followDistance - hitTowardInfo.distance);
        // }

        var hitDown = Physics.SphereCast(
            transform.position + Vector3.up * (_bodyRadius + _fromGround + _maxBallVerticalDifference), _bodyRadius,
            Vector3.down, out var hitDownInfo, _maxBallVerticalDifference + _maxBallVerticalDifference);
        if (hitDown)
        {
            _groundNormal = hitDownInfo.normal;
            transform.position = hitDownInfo.point + _groundNormal * _fromGround;
        }
    }
}
