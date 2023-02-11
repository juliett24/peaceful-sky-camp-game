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
    // // Currently broken, might fix if needed
    // [TooltipAttribute("How closely Scrap objects conform to the sphere. 1 for full effect, 0 to prevent movement.")]
    // [SerializeField] [Range(0, 1)] private float _roundness = 1f;

    // [TooltipAttribute("How deep Scrap objects are inset into the sphere, if Roundness is 1.")]
    // [SerializeField] private float _scrapInset = 1f;

    [TooltipAttribute("The list of attached Scrap objects.")]
    [SerializeField] private List<GameObject> _attachedObjects = new List<GameObject>();

    public float CoreVolume {
        get => 4f / 3f * Mathf.PI * CoreRadius * CoreRadius * CoreRadius;
        private set => CoreRadius = Mathf.Pow(value / (4f / 3f * Mathf.PI), 0.33333f);
    }
    public float CoreRadius {
        get;
        private set;
    } = 1f;
    public float MaxRadius {
        get;
        private set;
    } = 1f;

    public void Awake()
    {
        var scrap = gameObject.AddComponent<AttachableScrap>();
        scrap.Scrapball = this;
        foreach (var x in _attachedObjects)
        {
            AttachObject(x, false);
        }
    }

    public void AttachObject(GameObject obj, bool addToList = true)
    {
        if (obj.GetComponent<AttachableScrap>()) return;

        // TODO: fix this. Only increments radius once.
        var volumeIncrement = 4f * _volumeGain;
        CoreVolume += volumeIncrement;
        _body.mass += volumeIncrement * _massGain;
        MaxRadius = Mathf.Max(MaxRadius, Vector3.Distance(obj.transform.position, transform.position));

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

        var scrap = obj.AddComponent<AttachableScrap>();
        scrap.Scrapball = this;
        if (addToList)
        {
            _attachedObjects.Add(obj);
        }
    }

    private void ConformScrapToSphere()
    {
        // // Currently broken, might fix if needed some day

        // var origin = transform.position;
        // var radiusForScrap = Radius - _scrapInset;
        // foreach(var x in _attachedObjects)
        // {
        //     x.isKinematic = true;
        //     var pos = x.transform.position - origin;
        //     x.MovePosition(origin + Vector3.Lerp(pos, pos.normalized * radiusForScrap, _roundness));
        //     x.isKinematic = false;
        // }
    }
}
