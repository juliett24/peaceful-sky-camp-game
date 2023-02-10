using UnityEngine;
using System.Collections.Generic;


public class ScrapballContents : MonoBehaviour
{
    [TooltipAttribute("How closely Scrap objects conform to the sphere. 1 for full effect, 0 to prevent movement.")]
    [SerializeField] [Range(0, 1)] private float _roundness = 1f;
    [TooltipAttribute("How deep Scrap objects are inset into the sphere, if Roundness is 1.")]
    [SerializeField] private float _scrapInset = 1f;
    [TooltipAttribute("The attached Rigidbody.")]
    [SerializeField] private Rigidbody _body;
    [TooltipAttribute("The list of attached Scrap objects.")]
    [SerializeField] private readonly List<Rigidbody> _attachedObjects = new List<Rigidbody>();

    public float Volume {
        get => 4f / 3f * Mathf.PI * Radius * Radius * Radius;
        private set => Radius = Mathf.Pow(Volume / (4f / 3f * Mathf.PI), 0.33333f);
    }
    public float Radius {
        get;
        private set;
    } = 1f;

    public void AttachObject(GameObject obj)
    {
        var body = obj.AddComponent<Rigidbody>();
        var joint = obj.AddComponent<FixedJoint>();
        joint.connectedBody = _body;

        // TODO: fix this. Only increments radius once.
        var volumeIncrement = 4f;
        Volume = Volume + volumeIncrement;
        transform.localScale = Radius * 2f * Vector3.one;

        var origin = transform.position;
        var radiusForScrap = Radius - _scrapInset;
        foreach(var x in _attachedObjects)
        {
            x.isKinematic = true;
            var pos = x.transform.position - origin;
            x.MovePosition(origin + Vector3.Lerp(pos, pos.normalized * radiusForScrap, _roundness));
            x.isKinematic = false;
        }
        _attachedObjects.Add(body);
    }

    public void OnCollisionEnter(Collision collision)
    {
        var obj = collision.gameObject;
        if (obj.tag == "ScrapAttachable")
        {
            AttachObject(obj);
        }
    }
}
