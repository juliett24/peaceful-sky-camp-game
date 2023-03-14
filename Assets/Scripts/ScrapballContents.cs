using UnityEngine;
using System.Collections.Generic;


public class ScrapballContents : MonoBehaviour
{
    private const float VOLUME_RADIUS_RATIO = 2f; // Accurate formula: 3f

    [TooltipAttribute("The first attached AttachableScrap that should collect scrap. If empty, creates one on self.")]
    [SerializeField] AttachableScrap _core;
    [TooltipAttribute("The collider to grow when scrap gets added.")]
    [SerializeField] CapsuleCollider _collectingCollider;

    [TooltipAttribute("The attached Rigidbody.")]
    [SerializeField] private Rigidbody _body;

    [TooltipAttribute("The Rigidbody's SphereCollider, because this is a ball.")]
    [SerializeField] private SphereCollider _bodyCollider;
    [TooltipAttribute("The object to attach scrap to, so they move with that object.")]
    [SerializeField] public Transform AttachToTransform;

    [Header("Size")]
    [TooltipAttribute("How much mass the core Rigidbody gains from Scrap (Multiplier).")]
    [SerializeField] private float _massGain = 1f;
    [TooltipAttribute("The scale of the ball's Rigidbody collider. Grows from attaching Scrap.")]
    [SerializeField] private float _bodyScale = 1f;
    [TooltipAttribute("The scale of the ball's core, both its mesh and collider. Grows from attaching Scrap.")]
    [SerializeField] private float _coreScale = 1f;
    [TooltipAttribute("The ball's collecting trigger grows by this much. Setting to negative prevents big objects from sticking out.")]
    [SerializeField] private float _coreAddedRadius = -1f;

    [Header("Scrap")]
    [TooltipAttribute("How closely Scrap objects conform to the sphere. 1 for full effect, 0 to prevent movement.")]
    [SerializeField] [Range(0, 1)] private float _roundness = 1f;

    [TooltipAttribute("How deep Scrap objects are inset into the sphere, if Roundness is 1.")]
    [SerializeField] private float _scrapInset = 1f;

    [TooltipAttribute("The list of attached Scrap objects.")]
    [SerializeField] private List<Transform> _attachedObjects = new List<Transform>();
    [SerializeField] private UnityEngine.Events.UnityEvent<float> _radiusChanged; 
    [SerializeField] private float _selfDestructForce = 20f;

    public float ScrapVolume {
        get => 4f / 3f * Mathf.PI * Mathf.Pow(_scrapRadius, VOLUME_RADIUS_RATIO);
        private set => ScrapRadius = Mathf.Pow(value / (4f / 3f * Mathf.PI), 1 / VOLUME_RADIUS_RATIO);
    }
    public float ScrapRadius {
        get => _scrapRadius;
        private set
        {
            _scrapRadius = value;
            _bodyCollider.radius = ScrapRadius * _bodyScale;
            _collectingCollider.radius = ScrapRadius * _coreScale - _coreAddedRadius;
            _radiusChanged.Invoke(value);
        }
    }
    private float _scrapRadius = 1f;

    public void Awake()
    {
        if (!_core) 
        {
            _core = gameObject.AddComponent<AttachableScrap>();
            _core.Scrapball = this;
        }
        _body.mass = _core.BallAddedMass;
        ScrapVolume = _core.BallAddedVolume;
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
            ScrapRadius = 1f;
            _attachedObjects.Clear();
        }
    }

    public void AttachObject(GameObject obj, bool addToList = true)
    {
        var scrap = obj.GetComponent<AttachableScrap>();
        if (!scrap) return;

        ScrapVolume += scrap.BallAddedVolume;
        _body.mass += scrap.BallAddedMass * _massGain;
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
        var radiusForScrap = ScrapRadius - _scrapInset;
        foreach(var x in _attachedObjects)
        {
            var pos = x.position - origin;
            if (pos.sqrMagnitude < radiusForScrap)
            {
                x.position = origin + Vector3.Lerp(pos, pos.normalized * radiusForScrap, _roundness);
            }
        }
    }
}
