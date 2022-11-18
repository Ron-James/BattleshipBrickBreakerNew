using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseParticle : MonoBehaviour
{
    ParticleSystem particles;
    public float pauseTime = 0.1f;
    // Start is called before the first frame update
    private void Awake() {
        particles = GetComponent<ParticleSystem>();
        particles.Simulate(pauseTime);
    }
    void Start()
    {
        particles.Simulate(pauseTime);
        //particleSystem.Pause();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    


}
