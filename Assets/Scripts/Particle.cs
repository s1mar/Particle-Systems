
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
                //processAffectsOfGravity();
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

    public  void initializeParticleInstance(float mass, float ttl) {

        
        this.mass = mass;
        this.timeToLive = ttl;
        this.currentTimeToLive = timeToLive;
        
    }
    

    //Code below for collision detection
     public float radius;
     Renderer rend;   

       void Start(){
        rend = GetComponent<Renderer>();
        radius = rend.bounds.extents.magnitude;
    }

     public Vector3 getCenterPosition(){
        return transform.position;
    }

    public float getRadius(){
       return radius;
    }

    public Vector3 getCenter(){
        return rend.bounds.center;
    }
    
    public float getMass(){
        return mass;
    }
}
