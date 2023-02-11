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

    [SerializeField] private ScaleMode _scaleMode = ScaleMode.None;

    [Header("Scrap")]
    // // Currently broken, might fix if needed
    // [TooltipAttribute("How closely Scrap objects conform to the sphere. 1 for full effect, 0 to prevent movement.")]
    // [SerializeField] [Range(0, 1)] private float _roundness = 1f;

    // [TooltipAttribute("How deep Scrap objects are inset into the sphere, if Roundness is 1.")]
    // [SerializeField] private float _scrapInset = 1f;

    [TooltipAttribute("The list of attached Scrap objects.")]
    [SerializeField] private List<GameObject> _attachedObjects = new List<GameObject>();

    public float Volume {
        get => 4f / 3f * Mathf.PI * Radius * Radius * Radius;
        private set => Radius = Mathf.Pow(value / (4f / 3f * Mathf.PI), 0.33333f);
    }
    public float Radius {
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
        Volume = Volume + volumeIncrement;
        switch (_scaleMode)
        {
            case ScaleMode.All:
                transform.localScale = Radius * 2f * Vector3.one;
                break;

            case ScaleMode.ColliderOnly:
                _collider.radius = Radius;
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
