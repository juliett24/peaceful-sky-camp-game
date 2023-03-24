using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroRespawner : MonoBehaviour
{
    [SerializeField] private Animation _fadeAnimation;
    [SerializeField] private string _fadeAnimationName = "HeroRespawn";
    [SerializeField] [Cinemachine.TagField] private string _checkpointTag;
    [SerializeField] [Cinemachine.TagField] private string _hazardTag;

    public Transform RespawnSpot;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag(_checkpointTag))
        {
            RespawnSpot = collider.transform;
        }
        if (collider.gameObject.CompareTag(_hazardTag))
        {
            _fadeAnimation.Play(_fadeAnimationName);
        }
    }

    public void RespawnHero()
    {
        if (!RespawnSpot) return;
        transform.position = RespawnSpot.position;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
