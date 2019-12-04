
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    // the particle will have a mass, that will be assigned to it by the particle manager
    float mass;

    //the net force acting on the particle in a frame update;
    Vector3 theNetForceActingOnTheParticleAtAGivenInstant;

    //Since we have a kinematic approach, the particle will have a velocity
    Vector3 velocity;

    //the time after which the particle will die 
    float timeToLive;

    //the current time after which the particle will die
    float currentTimeToLive;

    //TODO: Color colorOfTheParticle;

    //The forces that have accumulated on the particle under observation
    List<Vector3> cummulativeOfForces = new List<Vector3>(0);

    bool considerAffectsOfGravity = false;

    const float GRAVITY = 6.673e-11f;

    bool startEngine = false;


    public void activateParticle()
    {
        startEngine = true;    
    }

    void Update()
    {
        if (startEngine)
        {
            //check to see if the object is still alive
            bool isAlive = hasTimeToLive(Time.deltaTime);
            if (isAlive)
            {
                //TODO : decayIntensity();
                forceProcessing(Time.deltaTime);
                processAffectsOfGravity();
            }
            else
            {
                //Destroy this instance of the particle
                Destroy(gameObject);
            }
        }
    }



    void forceProcessing(float deltaTime) {
        theNetForceActingOnTheParticleAtAGivenInstant = Vector3.zero;

        foreach (Vector3 force in cummulativeOfForces) {
            theNetForceActingOnTheParticleAtAGivenInstant += force;
        }

        cummulativeOfForces.Clear();

        //calculate acceleration, the euler integeration approach
        Vector3 acceleration = theNetForceActingOnTheParticleAtAGivenInstant / mass;
        velocity += acceleration * deltaTime;

        //update the position using the updated velocity
        transform.position += velocity * deltaTime;
    }


    void processAffectsOfGravity() {
        Particle[] particlesForGravity;

        if (considerAffectsOfGravity)
        {
            particlesForGravity = GameObject.FindObjectsOfType<Particle>();
        }
        else {
            //early exit
            return;
        }

        foreach (Particle A in particlesForGravity)
        {
            foreach (Particle B in particlesForGravity)
            {
                if (A != B)
                {
                  
                    Vector3 differenceAB = A.transform.position - B.transform.position;
                    float rs = Mathf.Pow(differenceAB.magnitude, 2f);
                    float magnitudeOfGravity = GRAVITY * mass * mass / rs;
                    Vector3 gravityFeltOnA = magnitudeOfGravity * differenceAB.normalized;

                    A.AddForce(-gravityFeltOnA);
                }
            }
        }

    }

    public void AddForce(Vector3 force) {
        cummulativeOfForces.Add(force);
    }

    public void setScale(float scaleFactor) {
        transform.localScale = transform.localScale * scaleFactor;
    }

    bool hasTimeToLive(float delta)
    {
        currentTimeToLive -= delta;
        return currentTimeToLive>0;
    }

    public  void initializeParticleInstance(float mass, float ttl, bool enableGravity = false) {

        
        this.mass = mass;
        this.timeToLive = ttl;
        considerAffectsOfGravity = enableGravity;
        this.currentTimeToLive = timeToLive;
        
    }
    
    
}
