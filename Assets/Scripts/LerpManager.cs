
using System.Collections.Generic;
using UnityEngine;

public class LerpManager : MonoBehaviour {

	Queue<Entity> lerpEntityQueue = new Queue<Entity>(0);
	public static bool flagComplete = false;
	float timer;

	bool startEngine = false;
	
	float timeElasped;

	bool notifiedEngineShutDown = false;
	Entity current;

	float lastPercHit = 0.0f;
	public void StartAnimating() {
			timeElasped = 0.0f;
			startEngine = true;
	}

		private void resetState(){
			lerpEntityQueue.Clear();
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
					Entity entity = new Entity(A.position,B.position,timerDuration);
					lerpEntityQueue.Enqueue(entity);	
			}

	}


	private void FixedUpdate() {

			if(startEngine){
			timeElasped+=Time.deltaTime;		
			if(current==null){
				if(lerpEntityQueue.Count>0){
					current = lerpEntityQueue.Dequeue();
				}else{
					//shutdown the engine
					startEngine = false;
					return;
				}
			}

			
			timer+=Time.deltaTime;
			float perc = timer/current.timeDuration;
			lastPercHit = perc;
			transform.position = Vector3.Lerp(current.from,current.to,perc);

			if(timer>=current.timeDuration){
				current =null;
				timer =0.0f;
			}

			}else{
				if(!notifiedEngineShutDown)
					{
						Debug.Log("Operation ended in: "+timeElasped+" last perc hit: "+lastPercHit);
						Debug.Log("With transform position at: "+transform.position);
						flagComplete = true;	
					}
					notifiedEngineShutDown = true;
			}

	}

	private static float calculateSmoothStep(float currentStep){
			return currentStep*currentStep*(3.0f - 2.0f*currentStep);
	}

	

	class Entity{

		public Vector3 from{get;set;}
		public Vector3 to{get;set;}
		public float timeDuration{get;set;}

		public Entity(Vector3 from, Vector3 to, float timerDuration){

					this.from = from;
					this.to = to;
					this.timeDuration = timerDuration;
		}


	}

}
