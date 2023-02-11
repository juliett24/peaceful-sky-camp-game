using UnityEngine;


public class AttachableScrap : MonoBehaviour
{
    public ScrapballContents Scrapball {
        get => _scrapball;
        set
        {
            _scrapball = value;
            if (gameObject != value.gameObject)
            {
                var joint = gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = value.GetComponent<Rigidbody>();
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
