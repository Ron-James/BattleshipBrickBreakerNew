using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTester : MonoBehaviour
{
    BallPhysics ballPhysics;
    public Vector3 direction = Vector3.right;
    // Start is called before the first frame update
    void Start()
    {
        ballPhysics = GetComponent<BallPhysics>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            ballPhysics.Launch(5f, direction);
        }
    }

    private void OnDrawGizmos() {
        Debug.DrawRay(transform.position, direction.normalized * 100, Color.red);
    }
}
