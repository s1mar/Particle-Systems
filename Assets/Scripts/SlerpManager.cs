using System.Collections.Generic;
using UnityEngine;

/* 
	The Slerp Manager will be attached to the transform that it will be manipulating
 */
public class SlerpManager : MonoBehaviour {

	public static bool flagComplete = false;
	Queue<Entity> slerpEntityQueue = new Queue<Entity>(0);

	float timer;

	bool startEngine = false;
	
	float timeElasped = 0.0f;

	bool notifiedEngineShutDown = false;
	Entity current;

	float lastPercHit = 0.0f;
	public void StartAnimating() {
			timeElasped = 0.0f;
			startEngine = true;
	}
	private void resetState(){
			slerpEntityQueue.Clear();
			flagComplete = false;
			timer = 0;
			startEngine = false;
			timeElasped = 0.0f;
			notifiedEngineShutDown = false;
			current = null;
			lastPercHit = 0.0f;
		}
	public void Intitalize(List<AnimationAttr> animations){
			resetState();
			
			int indexNextElement = 0;
			while(!(++indexNextElement>=animations.Count)){
					AnimationAttr A = animations[--indexNextElement];
					AnimationAttr B = animations[++indexNextElement];

					float timerDuration = B.time - A.time;
					timerDuration = timerDuration<0?0:timerDuration;
					//timerDuration = 1;
					Entity entity = new Entity(A.quaternion,B.quaternion,timerDuration);
					slerpEntityQueue.Enqueue(entity);	
			}

	}


	private void FixedUpdate() {

			if(startEngine){
			timeElasped+=Time.deltaTime;
			if(current==null){
				if(slerpEntityQueue.Count>0){
					current = slerpEntityQueue.Dequeue();
				}else{
					//shutdown the engine
					startEngine = false;
					return;
				}
			}

			
			timer+=Time.deltaTime;
			float perc = timer/current.timeDuration;
			lastPercHit = perc;
			transform.rotation = Quaternion.Slerp(current.from,current.to,perc);

			if(timer>=current.timeDuration){
				current =null;
				timer =0.0f;
			}

			}else{
				if(!notifiedEngineShutDown)
					{
						Debug.Log("Operation ended in: "+timeElasped+" last perc hit: "+lastPercHit);
						Debug.Log("With Quaternion rotation at: "+transform.rotation);
						flagComplete = true;
					}
					notifiedEngineShutDown = true;
			}

	}

private static float calculateSmoothStep(float currentStep){
			return currentStep*currentStep*(3.0f - 2.0f*currentStep);
}

	

	class Entity{

		public Quaternion from{get;set;}
		public Quaternion to{get;set;}
		public float timeDuration{get;set;}

		public Entity(Quaternion from, Quaternion to, float timerDuration){

					this.from = from;
					this.to = to;
					this.timeDuration = timerDuration;
		}


	}

}
