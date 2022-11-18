using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionVelocityControl : MonoBehaviour
{
    
    Vector3 lastVelocity;
    Vector3 oldVelocity;
    Vector3 lastCollVelocity;
    public float largestMagnitude;
    Rigidbody rb;

    BallPhysics ballPhysics;


    public float LargestMagnitude { get => largestMagnitude; set => largestMagnitude = value; }
    public Vector3 LastVelocity { get => lastVelocity; set => lastVelocity = value; }

    // Start is called before the first frame update
    void Start()
    {
        largestMagnitude = 0;
        lastVelocity = Vector3.zero;
        rb = GetComponent<Rigidbody>();
        ballPhysics = GetComponent<BallPhysics>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        
        


    }
    private void OnCollisionExit(Collision other)
    {

    }

}
