using UnityEngine;
using UnityEngine.InputSystem;


public class HeroController : MonoBehaviour
{
    [SerializeField] private Rigidbody _body;
    [SerializeField] private Transform _cam;

    [Header("Base Parameters")]
    [SerializeField] private float _linearAccel = 30f;
    [SerializeField] private float _linearMaxSpeed = 30f;
    [SerializeField] private float _angularAccel = 90f;
    [SerializeField] private float _highSpeedControlForgiveness = 0.5f;
    [SerializeField] [Range(0, 1)] private float _brakeStrength = 0.5f;

    [Header("Extra Gravity")]
    [SerializeField] private float _extraGravity = 9.8f;
    [TooltipAttribute("If On, Extra Gravity only applies when rolling down.")]
    [SerializeField] private bool _easyUphill = true;
    [TooltipAttribute("If On, Extra Gravity only applies when on ground.")]
    [SerializeField] private bool _extraGravityGroundOnly = true;

    [Header("U-Ramp Boosting")]
    [TooltipAttribute("U-Inertia builds up at 1/sec while falling, slower at slopes.")]
    [SerializeField] private float _maxUInertia = 6f;
    [TooltipAttribute("Ascending depletes U-Inertia similar to how it builds up. This is how fast it happens.")]
    [SerializeField] private float _uInertiaDepletion = 2f;
    [TooltipAttribute("If U-Inertia non-zero, going up increases speed while depleting it.")]
    [SerializeField] private float _uInertiaBoost = 4f;
    [TooltipAttribute("Each mass unit adds this value to the Boost amount.")]
    [SerializeField] private float _uInertiaBoostFromMass = 0.01f;
    [TooltipAttribute("U-Inertia decays by this amount every second.")]
    [SerializeField] private float _uInertiaBaseDecay = 0.2f;

    private bool _onGround = false;
    private Vector2 _moveInput;
    private float _uInertia = 0f;

    public void FixedUpdate()
    {
        var camRotation = Quaternion.Euler(0, _cam.rotation.eulerAngles.y, 0);
        var moveInputRotated = camRotation * new Vector3(_moveInput.x, 0, _moveInput.y);
        var horizontalSpeedBefore = new Vector3(_body.velocity.x, 0f, _body.velocity.z).magnitude;
        var newVelocity = _body.velocity + Time.fixedDeltaTime * GetAcceleration(moveInputRotated, horizontalSpeedBefore);

        newVelocity = LimitVelocity(newVelocity, horizontalSpeedBefore, moveInputRotated);
        if ((!_extraGravityGroundOnly || _onGround) && (!_easyUphill || _body.velocity.y < 0))
        {
            newVelocity += Vector3.down * Time.fixedDeltaTime * _extraGravity;
        }
        newVelocity = ApplyUInertia(newVelocity, Time.fixedDeltaTime);

        _body.velocity = newVelocity;
        _body.angularVelocity += Time.fixedDeltaTime * (camRotation * new Vector3(_moveInput.y, 0, -_moveInput.x) * _angularAccel);
        _onGround = false;
    }

    private Vector3 GetAcceleration(Vector3 moveInputRotated, float linearSpeed)
    {
        var turnPriority = -Vector3.Dot(moveInputRotated, new Vector3(_body.velocity.x, 0f, _body.velocity.z).normalized) + 2f;
        var speedControlForgiveness = 1f;
        if (linearSpeed > _linearMaxSpeed)
        {
            speedControlForgiveness = (linearSpeed / _linearMaxSpeed - 1f) * _highSpeedControlForgiveness + 1f;
        }
        return turnPriority * moveInputRotated * _linearAccel * speedControlForgiveness;
    }

    private Vector3 LimitVelocity(Vector3 velocity, float horizontalSpeedBefore, Vector3 moveInputRotated)
    {
        var velocityHorizontal = new Vector2(velocity.x, velocity.z);
        if (velocityHorizontal.sqrMagnitude > _linearMaxSpeed * _linearMaxSpeed)
        {
            var backwardsPull = Mathf.Max(-Vector2.Dot(velocityHorizontal.normalized, moveInputRotated), 0f);
            velocityHorizontal *= Mathf.Lerp(horizontalSpeedBefore, _linearMaxSpeed, backwardsPull * _brakeStrength) / velocityHorizontal.magnitude;
        }
        return new Vector3(velocityHorizontal.x, velocity.y, velocityHorizontal.y);
    }

    private Vector3 ApplyUInertia(Vector3 velocity, float deltaTime)
    {
        _uInertia -= deltaTime * velocity.normalized.y * (velocity.y < 0f? 1f : _uInertiaDepletion);
        _uInertia = Mathf.Clamp(_uInertia, 0f, _maxUInertia);
        if (_uInertia > 0 && velocity.y > 0)
        {
            velocity += velocity.normalized * deltaTime * (_uInertiaBoost + _body.mass * _uInertiaBoostFromMass);
        }
        _uInertia -= deltaTime * _uInertiaBaseDecay;
        return velocity;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            if (contact.normal.y > 0f)
            {
                _onGround = true;
                break;
            }
        }
    }
}
