using UnityEngine;

public class LimitedRotationConstraint : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _limit = 360f;

    private Quaternion _lastTargetRotation = Quaternion.identity;

    private void FixedUpdate()
    {
        if (!_target) return;

        var delta = _target.rotation * Quaternion.Inverse(_lastTargetRotation);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, delta * transform.rotation, _limit * Time.fixedDeltaTime);
        _lastTargetRotation = _target.rotation;
    }
}
