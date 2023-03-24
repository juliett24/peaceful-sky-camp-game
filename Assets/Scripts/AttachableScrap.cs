using UnityEngine;


public class AttachableScrap : MonoBehaviour
{
    public const string SCRAP_TAG = "ScrapAttachable";

    public float BallAddedMass = 1f;
    public float BallAddedVolume = 1f;

    public ScrapballContents Scrapball {
        get => _scrapball;
        set
        {
            _scrapball = value;
            if (gameObject != value.gameObject)
            {
                Destroy(GetComponent<Rigidbody>());
                foreach(var x in GetComponents<Collider>())
                {
                    x.enabled = false;
                }
                transform.parent = value.AttachToTransform;
            }
        }
    }

    [SerializeField] private ScrapballContents _scrapball;

    private void Awake()
    {
        gameObject.tag = SCRAP_TAG;
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (!_scrapball) return;

        var obj = collider.gameObject;
        // if (obj.tag == SCRAP_TAG)
        {
            _scrapball.AttachObject(obj);
        }
    }
}
