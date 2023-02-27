using UnityEngine;

[ExecuteAlways]
public class ScrapSpawner : MonoBehaviour
{
    [SerializeField] private bool _spawnOnStart = true;
    [SerializeField] private int _count = 50;
    [SerializeField] private float _totalRadius = 20f;
    [SerializeField] private float _solidRadius = 5f;
    [SerializeField] private float _distExponent = 0.5f;
    [SerializeField] private GameObject[] _prefabs = new GameObject[0];
    [SerializeField] private int _debugCircleSegments = 90;

    private Vector3[] _borderHits = new Vector3[0];

    public void SpawnAll()
    {
        for (int i = 0; i < _count; i++)
        {
            SpawnRandomPrefab(Mathf.Lerp(_totalRadius, _solidRadius, i / (float)_count));
        }
    }

    private void Awake()
    {
        if (Application.IsPlaying(gameObject))
        {
            if (_spawnOnStart) SpawnAll();
            enabled = false;
        }
    }

    private void OnValidate()
    {
        if (_borderHits.Length != _debugCircleSegments)
        {
            _borderHits = new Vector3[_debugCircleSegments];
        }
        var quat = Quaternion.identity;
        var vec = Vector3.right * _totalRadius;
        for (var i = 0; i < _debugCircleSegments; i++)
        {
            var hit = Physics.Raycast(transform.position + transform.rotation * quat * vec, transform.rotation * Vector3.down, out var hitInfo, 999f);
            _borderHits[i] = hitInfo.point + hitInfo.normal * 0.5f;
            quat.eulerAngles = quat.eulerAngles + new Vector3(0, 360 / _debugCircleSegments, 0);
        }
    }

    private void SpawnRandomPrefab(float radius)
    {
        var originalPrefab = _prefabs[Random.Range(0, _prefabs.Length - 1)];
        var randomRotation = Quaternion.Euler(Random.Range(-15f, 15f), Random.Range(-180f, 180f), 0);
        var hit = GetRandomSpawnPosition(radius);
        var newPrefab = Instantiate(originalPrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal) * randomRotation, transform);
    }

    private void Update()
    {
        if (Application.IsPlaying(gameObject)) return;

        if (transform.hasChanged)
        {
            OnValidate();
            transform.hasChanged = false;
        }
        var quat = Quaternion.identity;
        var vec = Vector3.right * _totalRadius;
        var origin = transform.position;
        var rotation = transform.rotation;
        for (var i = 0; i < _debugCircleSegments; i++)
        {
            var start = rotation * quat * vec;
            quat.eulerAngles = quat.eulerAngles + new Vector3(0, 360 / _debugCircleSegments, 0);
            Debug.DrawLine(origin + start, origin + rotation * quat * vec, new Color(1f, 0.6f, 0f));

            if (i != 0)
            {
                Debug.DrawLine(_borderHits[i - 1], _borderHits[i], new Color(1f, 0.6f, 0f));
            }
        }
    }

    private RaycastHit GetRandomSpawnPosition(float radius)
    {
        var r = radius * Mathf.Pow(Random.Range(0f, 1f), _distExponent);
        var t = Random.Range(0f, 2f * Mathf.PI);
        var offset = r * new Vector3(Mathf.Cos(t), 0f, Mathf.Sin(t));
        var castFrom = transform.position + transform.rotation * offset;
        var hit = Physics.Raycast(castFrom, transform.rotation * Vector3.down, out var hitInfo, 999f);
        return hitInfo;
    }
}
