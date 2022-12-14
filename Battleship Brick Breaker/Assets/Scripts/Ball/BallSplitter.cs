using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSplitter : MonoBehaviour
{
    Rigidbody rb;
    PlayerTracker playerTracker;
    BallPhysics ballPhysics;
    // Start is called before the first frame update
    private void Awake() {
        ballPhysics = GetComponent<BallPhysics>();
        rb = GetComponent<Rigidbody>();
        playerTracker = GetComponent<PlayerTracker>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SplitBall(int times){
        for (int loop = 0; loop < times; loop++){
            ExtraBallManager.instance.SpawnExtraBall(transform.position, ballPhysics.IsPlayer1());
        }
    }
}
