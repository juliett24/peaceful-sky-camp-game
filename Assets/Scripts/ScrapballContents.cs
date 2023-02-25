using UnityEngine;
using System.Collections.Generic;


public class ScrapballContents : MonoBehaviour
{
    enum ScaleMode
    {
        None, ColliderOnly, All,
    }

    [TooltipAttribute("The attached Rigidbody.")]
    [SerializeField] private Rigidbody _body;

    [TooltipAttribute("The attached SphereCollider, because this is a ball.")]
    [SerializeField] private SphereCollider _collider;

    [Header("Volume")]
    [TooltipAttribute("How much volume the ball gains from Scrap (Multiplier).")]
    [SerializeField] private float _volumeGain = 1f;
    [TooltipAttribute("How much mass the core Rigidbody gains from Scrap (Multiplier). Non-zero values make heavy scrap heaps easier to control.")]
    [SerializeField] private float _massGain = 0f;

    [SerializeField] private ScaleMode _scaleMode = ScaleMode.None;

    [Header("Scrap")]
    [TooltipAttribute("How closely Scrap objects conform to the sphere. 1 for full effect, 0 to prevent movement.")]
    [SerializeField] [Range(0, 1)] private float _roundness = 1f;

    [TooltipAttribute("How deep Scrap objects are inset into the sphere, if Roundness is 1.")]
    [SerializeField] private float _scrapInset = 1f;

    [TooltipAttribute("The list of attached Scrap objects.")]
    [SerializeField] private List<Transform> _attachedObjects = new List<Transform>();
    [SerializeField] private float _selfDestructForce = 20f;

    public float CoreVolume {
        get => 4f / 3f * Mathf.PI * CoreRadius * CoreRadius * CoreRadius;
        private set => CoreRadius = Mathf.Pow(value / (4f / 3f * Mathf.PI), 0.33333f);
    }
    public float CoreRadius {
        get;
        private set;
    } = 1f;

    public void Awake()
    {
        var scrap = gameObject.AddComponent<AttachableScrap>();
        scrap.Scrapball = this;
        foreach (var x in _attachedObjects)
        {
            AttachObject(x.gameObject, false);
        }
    }

    public void OnSelfDestruct(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var position = transform.position;
            foreach (var x in _attachedObjects)
            {
                var body = x.GetComponent<Rigidbody>();
                var direction = (x.transform.position - position).normalized;
                body.velocity = direction * _selfDestructForce;
            }
            CoreRadius = 1f;
            _attachedObjects.Clear();
        }
    }

    public void AttachObject(GameObject obj, bool addToList = true)
    {
        var scrap = obj.GetComponent<AttachableScrap>();
        if (!scrap) return;

        var volumeIncrement = 4f * _volumeGain;
        CoreVolume += volumeIncrement;
        _body.mass += volumeIncrement * _massGain;

        switch (_scaleMode)
        {
            case ScaleMode.All:
                transform.localScale = CoreRadius * 2f * Vector3.one;
                break;

            case ScaleMode.ColliderOnly:
                _collider.radius = CoreRadius;
                break;
        }

        ConformScrapToSphere();

        scrap.Scrapball = this;
        if (addToList)
        {
            _attachedObjects.Add(obj.transform);
        }
    }

    private void ConformScrapToSphere()
    {
        var origin = transform.position;
        var radiusForScrap = CoreRadius - _scrapInset;
        foreach(var x in _attachedObjects)
        {
            var pos = x.position - origin;
            x.position = origin + Vector3.Lerp(pos, pos.normalized * radiusForScrap, _roundness);
        }
    }
}
