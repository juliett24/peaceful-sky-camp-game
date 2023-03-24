using UnityEngine;

public class AnimEventPasser : MonoBehaviour
{
    [System.Serializable]
    private struct ObjectMethodPair
    {
        public GameObject obj;
        public string method;
    }

    [SerializeField] private ObjectMethodPair[] _calls;

    public void PassCall(int id)
    {
        _calls[id].obj.SendMessage(_calls[id].method);
    }
}
