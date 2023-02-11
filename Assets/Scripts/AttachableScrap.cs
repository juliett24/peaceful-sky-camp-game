using UnityEngine;


public class AttachableScrap : MonoBehaviour
{
    public Joint Joint { get; private set; }
    public ScrapballContents Scrapball {
        get => _scrapball;
        set
        {
            _scrapball = value;
            if (gameObject != value.gameObject)
            {
                Joint = gameObject.AddComponent<FixedJoint>();
                Joint.connectedBody = value.GetComponent<Rigidbody>();
            }
        }
    }

    private ScrapballContents _scrapball;

    public void OnCollisionEnter(Collision collision)
    {
        if (!_scrapball) return;

        var obj = collision.gameObject;
        if (obj.tag == "ScrapAttachable")
        {
            _scrapball.AttachObject(obj);
        }
    }
}
