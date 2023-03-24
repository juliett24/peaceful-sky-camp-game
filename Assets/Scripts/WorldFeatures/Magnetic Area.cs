
using System.Collections.Generic;
using UnityEngine;

public class MagneticArea : MonoBehaviour
{
    [SerializeField] private Vector3 ForceVector;
    
    private List<Rigidbody> affectedObjects = new List<Rigidbody>();
    
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody affectedRigidbody))
        {
            affectedObjects.Add(affectedRigidbody);
        } 
        
    }
 
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody affectedRigidbody))
        {
            affectedObjects.Remove(affectedRigidbody);
        } 
        
    }
 
    void FixedUpdate()
    {
        foreach (Rigidbody affectedObject in affectedObjects)
        {
            Vector3 vel = affectedObject.velocity;
            affectedObject.velocity = new Vector3(vel.x, 0f, vel.z);
            
            affectedObject.AddForce(ForceVector);
            
            
        }

    }
}
