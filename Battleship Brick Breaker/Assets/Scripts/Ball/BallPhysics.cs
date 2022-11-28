using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  Controls movement of a ball in a pong-like game. allows for full control over velocity.
/// </summary>
public class BallPhysics : MonoBehaviour
{
    [Header("Local Components and Variables")]
    [SerializeField] float radius = 0.5f;
    [SerializeField] GameObject paddle;
    [SerializeField] float bounciness = 1.005f;
    [SerializeField] LayerMask collisionLayer;
    Rigidbody rb;
    bool isBoundToPaddle;
    PlayerTracker playerTracker; //Class which tracks the last player to hit ball and initial owner of ball

    [Header("Velocity Variables")]
    [SerializeField] Vector3 currentVelocityDirection;
    [SerializeField] float currentSpeed;
    [SerializeField] float maxBounceAngle = 60f;
    [SerializeField] Vector3 velocityCache;


    [Header("Sounds")]
    [SerializeField] Sound generalHit;


    //Encapsulated Fields
    public float Radius { get => radius; set => radius = value; }
    public bool IsBoundToPaddle { get => isBoundToPaddle; set => isBoundToPaddle = value; }
    public float CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }
    public Vector3 CurrentVelocityDirection { get => currentVelocityDirection; set => currentVelocityDirection = value; }
    public float MaxBounceAngle { get => maxBounceAngle; set => maxBounceAngle = value; }
    public float Bounciness { get => bounciness; set => bounciness = value; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        generalHit.src = GetComponent<AudioSource>();
        playerTracker = GetComponent<PlayerTracker>();
        velocityCache = Vector3.zero;
    }
    void Start()
    {
        rb.isKinematic = true;
        //justHitPaddleRoutine = null;

        Radius = transform.localScale.x / 2;
        BindToPaddle();

    }


    /// <summary>
    ///  Pauses ball by setting speed to 0 and caching it
    /// </summary>
    public void PauseBall()
    {
        velocityCache = currentSpeed * currentVelocityDirection.normalized;
        currentSpeed = 0;
    }
    /// <summary>
    ///  Resumes ball from paused state
    /// </summary>
    public void ResumeBall()
    {
        currentSpeed = velocityCache.magnitude;
        currentVelocityDirection = velocityCache.normalized;
    }

    /// <summary>
    ///  Get inward direction based on last paddle to hit ball.
    /// </summary>
    /// <returns>-1 if last player was left sided, 1 if last player was right sided</returns>
    public int GetInwardSign()
    {
        if (playerTracker.GetCurrentOwner() == 1)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
    /// <summary>
    ///  Checks if intitial owner of ball is player 1 or not
    /// </summary>
    /// <returns>true if main owner is player 1, false if player 2</returns>
    public bool IsPlayer1()
    {
        if (playerTracker.GetMainOwner() == 1)
        {
            return true;
        }
        else if (playerTracker.GetMainOwner() == 2)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private void FixedUpdate()
    {

        if (currentSpeed > 0)
        {
            currentVelocityDirection.y = 0;
            Debug.DrawLine(transform.position, transform.position + (currentVelocityDirection * 2), Color.red, Time.fixedDeltaTime);
            float distance = currentSpeed * Time.fixedDeltaTime;
            Vector3 newPosition = transform.position + (currentVelocityDirection.normalized * distance);//position to move ball based on current speed and direction
            rb.MovePosition(newPosition);
        }

    }
    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    ///  Increases radius of ball
    /// </summary>
    public void IncreaseSize(float percent)
    {
        float newRadius = (Radius * percent) * 2;
        transform.localScale = newRadius * Vector3.one;
    }
    /// <summary>
    ///  Resets radius of ball
    /// </summary>
    public void ResetSize()
    {
        float newRadius = (Radius) * 2;
        transform.localScale = newRadius * Vector3.one;
    }

    public Collider ClosestCollider(Collider[] colliders)
    {
        float closest = Vector3.Distance(transform.position, colliders[0].ClosestPoint(transform.position));
        Collider closestCollider = colliders[0];
        if (colliders.Length == 1)
        {
            return colliders[0];
        }
        else
        {
            for (int loop = 0; loop < colliders.Length; loop++)
            {
                float distance = Vector3.Distance(transform.position, colliders[loop].ClosestPoint(transform.position));
                if (distance < closest)
                {
                    closest = distance;
                    closestCollider = colliders[loop];
                }
            }
        }
        return closestCollider;
    }

    public Vector3[] CollidingNormals()
    {
        float radius = 0.5f * transform.localScale.x;

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        Vector3[] normals = new Vector3[colliders.Length];

        for (int loop = 0; loop < colliders.Length; loop++)
        {
            Vector3 collisionPoint = colliders[loop].ClosestPoint(transform.position);
            Vector3 direction = (collisionPoint - transform.position).normalized;

            Ray ray = new Ray(transform.position, direction);
            Debug.DrawRay(transform.position, direction * 10, Color.green, 10f);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, collisionLayer))
            {
                normals[loop] = hit.normal;
                //Debug.Log(hit.normal + " THIS NORMAL");
            }
            else
            {
                normals[loop] = Vector3.zero;
            }
        }
        //Debug.Log(ResultantNormal(normals) + " Resultant");
        return normals;
    }

    public Vector3 ResultantNormal(Vector3[] normals)
    {
        Vector3 result = Vector3.zero;
        for (int loop = 0; loop < normals.Length; loop++)
        {
            result += normals[loop];
        }
        return result.normalized;
    }
    private void OnCollisionEnter(Collision other)
    {
        bool rotate = true;
        float radius = 0.5f * transform.localScale.x;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, collisionLayer);
        Vector3 normal;
        if (colliders.Length > 1)
        {
            Debug.Log("2 colliders");

            if (other.collider == colliders[0])
            {
                Vector3[] normals = CollidingNormals();
                normal = ResultantNormal(normals);
                rotate = false;
                //Debug.Break();
            }
            else
            {
                return;
            }
        }
        else
        {
            normal = other.GetContact(0).normal;
        }
        //take normal of collision surface
        //Check what ball collided with based on collider tag
        switch (other.collider.tag)
        {
            case "Arena":
                Reflect(normal, rotate);
                break;
            case "Brick":
                Reflect(normal, rotate);
                break;
            case "Paddle":
                int sign = 1;

                PaddleController paddle = other.gameObject.GetComponentInParent<PaddleController>();
                if (paddle != null)
                {
                    if (!paddle.canHitBall)
                    {
                        return;
                    }
                    if (paddle.Player1)
                    {
                        sign = -1;
                    }
                    else
                    {
                        sign = 1;
                    }
                }

                //Rotates reflection vector based on where ball collides with paddle relative to paddle's centroid
                Vector3 paddlePos = other.transform.position;
                Vector3 contactPoint = other.GetContact(0).point;

                float offset = paddlePos.z - contactPoint.z;
                float width = other.collider.GetComponent<BoxCollider>().bounds.size.z / 2;

                Vector3 reflectedDirection = Vector3.Reflect(currentVelocityDirection, normal);

                float currentAngle = Vector3.SignedAngle(sign * Vector3.right, reflectedDirection, Vector3.up);
                float bounceAngle = (offset / width) * maxBounceAngle;
                float newAngle = Mathf.Clamp(currentAngle + bounceAngle, -this.maxBounceAngle, this.maxBounceAngle);

                Quaternion rotation = Quaternion.AngleAxis(newAngle, Vector3.up);
                currentVelocityDirection = rotation * (sign * Vector3.right);
                IncreaseVelocity(bounciness);
                break;
        }

    }

    public void PaddleReflect(Vector3 normal, Transform paddle, Vector3 contactPoint)
    {
        float offset = paddle.position.z - contactPoint.z;
        float width = paddle.GetComponent<BoxCollider>().bounds.size.z / 2;

        Vector3 reflectedDirection = Vector3.Reflect(currentVelocityDirection, normal);

    }

    /// <summary>
    ///  Reflects balls current direction vector based on input normal. Ensures no 90 degree reflections occur
    /// </summary>
    /// <param name="normal">Reflects velocity relative to normal vector</param>
    public void Reflect(Vector3 normal)
    {
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            rand = 1;
        }


        Vector3 reflectedVector = Vector3.Reflect(currentVelocityDirection, normal);
        //difference variables which will be small if reflected vector is very similar to initial velocity vector
        float diffZ = 1 - Mathf.Abs(reflectedVector.normalized.z);
        float diffX = 1 - Mathf.Abs(reflectedVector.normalized.x);
        
        if (normal == Vector3.right || normal == Vector3.left)
        {
            if (diffX <= 0.001f)
            {

                float angle = 10;
                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    reflectedVector = Quaternion.AngleAxis(-1 * angle, Vector3.up) * reflectedVector; // ensure reflected vector isn't parralel with initial vector
                    currentVelocityDirection = reflectedVector.normalized;
                }
                else
                {
                    reflectedVector = Quaternion.AngleAxis(angle, Vector3.up) * reflectedVector;
                    currentVelocityDirection = reflectedVector.normalized;
                }
            }
            else
            {
                currentVelocityDirection = reflectedVector.normalized;
            }
        }
        
        if (normal == Vector3.forward || normal == Vector3.back)
        {
            if (diffZ <= 0.001f) // if difference between initial vector and reflected vecotr Z component is very small
            {
                Debug.Log("Rotated Reflection");
                float angle = Random.Range(25, 36); // determine random angle to rotate reflection vector
                int rotateDirection = 1;
                if (currentVelocityDirection.x > 0) // determines rotation direction based on initial velocity vector
                {
                    rotateDirection = 1;
                }
                else
                {
                    rotateDirection = -1;
                }

                Debug.Log(reflectedVector.normalized + " initial");

                reflectedVector = Quaternion.AngleAxis(rotateDirection * angle, Vector3.up) * reflectedVector; // ensure reflected vector isn't parralel with initial vector
                currentVelocityDirection = reflectedVector.normalized;
            }
            else
            {
                currentVelocityDirection = reflectedVector.normalized;
            }
        }
        
        else
        {
            currentVelocityDirection = reflectedVector.normalized;
        }



    }
    public void Reflect(Vector3 normal, bool rotate)
    {
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            rand = 1;
        }


        Vector3 reflectedVector = Vector3.Reflect(currentVelocityDirection, normal);
        if (rotate)
        {
            //difference variables which will be small if reflected vector is very similar to initial velocity vector
            float diffZ = 1 - Mathf.Abs(reflectedVector.normalized.z);
            float diffX = 1 - Mathf.Abs(reflectedVector.normalized.x);
            if (normal == Vector3.right || normal == Vector3.left)
            {
                if (diffX <= 0.02f)
                {
                    float angle = Random.Range(25, 36);
                    int random = Random.Range(0, 2);
                    if (random == 0)
                    {
                        reflectedVector = Quaternion.AngleAxis(-1 * 30f, Vector3.up) * reflectedVector; // ensure reflected vector isn't parralel with initial vector
                        currentVelocityDirection = reflectedVector.normalized;
                    }
                    else
                    {
                        reflectedVector = Quaternion.AngleAxis(30, Vector3.up) * reflectedVector;
                        currentVelocityDirection = reflectedVector.normalized;
                    }
                }
                else
                {
                    currentVelocityDirection = reflectedVector.normalized;
                }
            }
            else if (normal == Vector3.forward || normal == Vector3.back)
            {
                if (diffZ <= 0.02f) // if difference between initial vector and reflected vecotr Z component is very small
                {
                    float angle = Random.Range(25, 36); // determine random angle to rotate reflection vector
                    int rotateDirection = 1;
                    if (currentVelocityDirection.x > 0) // determines rotation direction based on initial velocity vector
                    {
                        rotateDirection = 1;
                    }
                    else
                    {
                        rotateDirection = -1;
                    }

                    Debug.Log(reflectedVector.normalized + " initial");

                    reflectedVector = Quaternion.AngleAxis(rotateDirection * 10f, Vector3.up) * reflectedVector; // ensure reflected vector isn't parralel with initial vector
                    currentVelocityDirection = reflectedVector.normalized;
                }
                else
                {
                    currentVelocityDirection = reflectedVector.normalized;
                }
            }
            else
            {
                currentVelocityDirection = reflectedVector.normalized;
            }
        }
        else
        {
            currentVelocityDirection = reflectedVector.normalized;
        }




    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    /// <summary>
    /// Increase speed by given percentage
    /// </summary>
    /// <param name="percentage"> Increase speed by percentage x</param>
    public void IncreaseVelocity(float percentage)
    {
        float newMag = currentSpeed * percentage;
        if (newMag >= GameManager.instance.MaxBallVelocity)
        {
            newMag = GameManager.instance.MaxBallVelocity;
        }
        currentSpeed = newMag;
    }

    /// <summary>
    /// Sets parent to paddle's transform. Sets velocity to zero and position to specified ball position on paddle
    /// </summary>

    public void BindToPaddle()
    {
        currentSpeed = 0;
        currentVelocityDirection = Vector3.zero;
        isBoundToPaddle = true;
        if (paddle == null)
        {
            return;
        }
        transform.SetParent(paddle.transform);
        transform.position = paddle.GetComponent<PaddleController>().BallPosition.position;

    }

    /// <summary>
    /// Launches ball into arena
    /// </summary>
    /// <param name="point"> Initial Return position</param>
    /// <param name="direction"> Direction of launch</param>
    public void Launch(float power, Vector3 direction)
    {
        isBoundToPaddle = false;
        transform.parent = null;

        currentSpeed = power;
        currentVelocityDirection = direction;
        paddle.GetComponent<Artillery>().CanFire = true;

    }

    /// <summary>
    /// Method is called when ball goes out of arena as part of OnBallOut Event
    /// </summary>
    public void OnBallOut()
    {
        BindToPaddle();

    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "OOB":
                BindToPaddle();
                paddle.GetComponentInChildren<AimArrow>().StartOscillation();
                break;
        }
    }
    public void StartRandomReturn(float velocity, Transform point)
    {
        StartCoroutine(RandomReturn(5, velocity, point));
    }

    /// <summary>
    ///  Stops ball at specified position for x number of frames then launches it back at paddle at a randomly rotated direction
    /// </summary>
    /// <param name="frameStop"> number of frames ball freezes for</param>
    /// <param name="velocity"> Return Velocity</param>
    /// <param name="point"> Initial Return position</param>
    IEnumerator RandomReturn(int framesStop, float velocity, Transform point)
    {
        currentSpeed = 0;

        for (int s = 0; s < framesStop; s++)
        {
            yield return null;
        }
        transform.position = point.position;
        Vector3 direction = point.right;
        direction = Quaternion.AngleAxis(Random.Range(-30, 31), Vector3.up) * direction;
        direction = direction.normalized;


        currentSpeed = velocity;
        currentVelocityDirection = direction;

    }

}
