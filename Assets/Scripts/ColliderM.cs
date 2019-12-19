using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ColliderM : MonoBehaviour
{

    [SerializeField]
    public ParticleManager particleManager;
    
    [Range (0,10)]    
    public float correctionThreshold = 0;

    [Range(1,1000)]
    public int impulseMultiplier = 1;

    private Cushion cushion; // the cushion component attached to this collider

    [Range(2,9)]
    public int cushionMassMultiplier =2; //the cushion's mass would be relative to the mass of the ball but one thing is for certain, it's always gonna be more than the ball


    [Range(0,1)] 
    [SerializeField] public float eRestituion;

    [Range(0,1)] 
    [SerializeField] public float uFriction;

    public float offsetImpulseCorrection = 5000;    

    void Start(){
        cushion = GetComponent<Cushion>();
    }

    
    //Check for collision of each alive particle against the collider in every fixed update
    void FixedUpdate() {
       // Queue<Particle> particlesScheduledForDeletion = new Queue<Particle>();

         if(particleManager.particles.Count>0){

                //remove all "null" or dead particle references from the list before running the check
                particleManager.particles.RemoveAll(item => item == null);

                //check for collisions
                foreach(Particle p in particleManager.particles)
                {       
                     if(p!=null){

                        if(CollisionResolutionWithParticle(p)){

                            //Collision happened
                            Vector3 impulseForce = CalculateImpulseVectorParticleCushion(p,cushion);
                            p.AddForce(impulseForce * impulseMultiplier);

                        }

                     }
                               
                }

         }

    }

bool CollisionResolutionWithParticle(Particle particle){
           //Closest point to sphere center by clamping
           float x_closest =  Mathf.Max(cushion.MinX, Mathf.Min(particle.getCenter().x,cushion.MaxX));
           float y_closest =  Mathf.Max(cushion.MinY, Mathf.Min(particle.getCenter().y,cushion.MaxY));
           float z_closest =  Mathf.Max(cushion.MinZ, Mathf.Min(particle.getCenter().z,cushion.MaxZ));

           float distance = Mathf.Sqrt((x_closest - particle.getCenter().x)*(x_closest - particle.getCenter().x) +
                                       (y_closest - particle.getCenter().y)*(y_closest - particle.getCenter().y) +
                                       (z_closest - particle.getCenter().z)*(z_closest - particle.getCenter().z));

           return distance<particle.getRadius();                            
    }

     public Vector3 CalculateImpulseVectorParticleCushion(Particle A, Cushion B){
        float mass = A.getMass();
        Vector3 AB = B.center - A.getCenter();
        Vector3 n = AB.normalized;
        float Impulse = (-Vector3.Dot((1+eRestituion)*AB,n))/Vector3.Dot(n,(1/mass + 1/(mass*cushionMassMultiplier))*n); 
        
        Vector3 impulse = (offsetImpulseCorrection/3F)*Impulse*n;

        //Removing the z component
        impulse.z = 0;
        return impulse;
    }
}
