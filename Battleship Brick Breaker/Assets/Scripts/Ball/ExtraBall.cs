using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraBall : MonoBehaviour
{

    [SerializeField] GameObject actives;
    [SerializeField] GameObject inactives;

    BallPhysics ballPhysics;
    MeshRenderer meshRenderer;
    Collider ballCollider;
    PlayerTracker playerTracker;

    bool inPlay;

    Rigidbody rb;
    // Start is called before the first frame update
    private void Awake() {
        ballPhysics = GetComponent<BallPhysics>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        ballCollider = GetComponent<Collider>();
        playerTracker = GetComponent<PlayerTracker>();
        DisableBall();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() {
        
    }

    public void EnableBall(bool player1, Vector3 position){
        GetComponent<MeshRenderer>().enabled = true;
        ballCollider.enabled = true;
        transform.SetParent(null);
        playerTracker.SetCurrentOwner(player1);
        transform.position = position;
        Vector3 direction = Vector3.right;
        direction = Quaternion.AngleAxis(Random.Range(-360, 361), Vector3.up) * direction;
        ballPhysics.CurrentSpeed = GameManager.instance.InitialVelocity;
        ballPhysics.CurrentVelocityDirection = direction;
        inPlay = true;
    }
    public void DisableBall(){
        transform.SetParent(inactives.transform);
        transform.localPosition = Vector3.zero;
        
        ballCollider.enabled = false;
        
        ballPhysics.CurrentSpeed = 0;
        ballPhysics.CurrentVelocityDirection = Vector3.zero;
        inPlay = false;
    }

    public void OnBallOut(){
        Debug.Log("should disable here");
        DisableBall();
    }
}
