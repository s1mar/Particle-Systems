using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public PARTICLE_ORIGIN origin = PARTICLE_ORIGIN.BACKWARD;

    [SerializeField]
    float particleScale = 1.0f;

    [SerializeField]
    float maxAngleDeviation = 75.0f;

    [SerializeField]
    GameObject prefabParticle;

    [SerializeField]
    private float mass = 1000000;

    [SerializeField]
    private GameObject emitterSource; // the source object from which the particles would be emitted

    [SerializeField]
    private float ttl; // the time to live in seconds

    [SerializeField]
    private float maxImpulse = 10000f; //10000 N

    [SerializeField]
    private float minImpulse = 1000f; //1000 N


    [SerializeField]
    private bool enableGravity;

    
    [SerializeField]
    private int maxNumOfParticlesCreatedInAnInstant = 100;
    
    
    [SerializeField]
    [Range(10, 100)]
    private int minNumOfParticlesCreatedInAnInstant = 10;

  

    [SerializeField]
    [Range(1, 100)]
    private int particleCreationFactor = 1;

    

    void Awake(){

        if (emitterSource == null) {
            throw new System.Exception("Please specify the transform whose center would be used for emitting particles");
        }

        if (prefabParticle == null)
        {
            throw new System.Exception("Please specify the particle prefab");

        }  
    }

    void FixedUpdate() {
        //update the position of the particle emitter with the moving source, it should be at the center of it always
        transform.position = emitterSource.transform.position;
        generateParticles();
    }

    Vector3 multiplyComponents(Vector3 A, Vector3 B)
    {
        return new Vector3(A.x * B.x, A.y * B.y, A.z * B.z);
    }
    void generateParticles() {

        //determine the direction vector in which the particles will be emitted
        Vector3 emissionDirection = determineEmissionDirection();

        //determine the number of particles to be generated this frame
        int numOfParticlesToGenerate = Random.Range(minNumOfParticlesCreatedInAnInstant, maxNumOfParticlesCreatedInAnInstant);
        numOfParticlesToGenerate *= particleCreationFactor;

        for (int i = 1; i <= numOfParticlesToGenerate; i++) {

            //generate a particle and fire it in a direction dependent on a random selection from a range of ejection angles


            float impulse = Random.Range(minImpulse, maxImpulse);

            
            //determine and instantiate the particle at the emitter source
            GameObject particleObject = createParticle(emitterSource.transform.position);
            
            Particle particleBehaviour = particleObject.AddComponent<Particle>();
            particleBehaviour.initializeParticleInstance(mass, ttl, enableGravity);

            Vector3 thrust = emissionDirection * impulse;

            //Apply random deviation; ejection angle
            applyRandomAngleDeviation(ref thrust);

            particleBehaviour.AddForce(thrust);

            //set the scale before activiating the particle
            particleBehaviour.setScale(particleScale);

            particleBehaviour.activateParticle();
        }    

    }

    GameObject createParticle(Vector3 birthPos) {
        return Instantiate(prefabParticle, birthPos, Quaternion.identity);
    }
    
    void applyRandomAngleDeviation(ref Vector3 vectorToRotate) {

        float randAngle_Up = Random.Range(-maxAngleDeviation, maxAngleDeviation);
        float randAngle_Right = Random.Range(-maxAngleDeviation, maxAngleDeviation);

        vectorToRotate = Quaternion.AngleAxis(randAngle_Up, emitterSource.transform.up) *vectorToRotate;
        vectorToRotate = Quaternion.AngleAxis(randAngle_Right, emitterSource.transform.right) * vectorToRotate;

    }


    Vector3 determineEmissionDirection()
    {
        switch (origin) {

            case PARTICLE_ORIGIN.BACKWARD:
                return -1 * emitterSource.transform.right;
            case PARTICLE_ORIGIN.FORWARD:
                return emitterSource.transform.right;
            case PARTICLE_ORIGIN.UP:
                return emitterSource.transform.up;
            case PARTICLE_ORIGIN.DOWN:
                return -1 * emitterSource.transform.up;
            default:
                return emitterSource.transform.position;
        }

    }

   public enum PARTICLE_ORIGIN { 
        BACKWARD,
        FORWARD,
        UP,
        DOWN
    }
}
