using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TardisControllerV4 : MonoBehaviour {


	[SerializeField]int framesBetweenEachKeyFrame = 56; // this will help with the smoothning while lerping
	[SerializeField] float tension = 1;
	[SerializeField] GameObject TardisObject;
	[SerializeField] GameObject ReloadBtn;
	List<AnimationAttr> animations = new List<AnimationAttr>(0);
	Queue<Vector3> lerpQueue;
	Queue<Quaternion> slerpQueue;

	bool startProcessing;
	bool endOfRotationSequence;
	bool endOfTranslationSequence;	

	//For Slerp rotation
	Quaternion fromQuat;
	Quaternion toQuat;	

	float timer;
	private void Awake() {
		if(TardisObject!=null){TardisObject.SetActive(false);}
	}

	// Use this for initialization
	void Start () {

		animations.Clear();

		//get the animation per second data from the text asset
		animations.AddRange(getKeyFramesFromTextAsset());


		//Check to see if the first key entry is at time 0, for if it is, update the scene accordingly, move and rotate the object to the partiular position and delete the first entry from the queues
		
		
		 AnimationAttr animationAttr = animations[0];
			if(animationAttr.time==0){
					transform.position = animationAttr.position;
					transform.rotation = animationAttr.quaternion;
					animations.RemoveAt(0);
			}
	

		//By now we would have the tardis at the appropriate frame for zero
		if(TardisObject!=null){TardisObject.SetActive(true);}
		
		//Now that we have the valid set of animations, set the lerp and slerp queues
		List<Vector3> rawPosList = animations.Select(item=> RhsToLhs.ConvertPosToLhs(item.position)).ToList();
		
		
		lerpQueue = new Queue<Vector3>(createCatmullRomSpline(rawPosList,framesBetweenEachKeyFrame,tension));
		slerpQueue = new Queue<Quaternion>(animations.Select(item=>RhsToLhs.ConvertRhsToLHSQuaternion(item.quaternion,false)));
	
		//Now that we have the queues set up, let;s process them
		
		//Rotation points initialiation
		if(slerpQueue.Count>0){
		fromQuat = transform.rotation;
		toQuat  = slerpQueue.Dequeue();
		Debug.Log("To Rotation: "+toQuat);
		//fromQuat.Normalize();
		//toQuat.Normalize();
		}
		else{
			endOfRotationSequence = true;
		}	
		
		startProcessing = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(startProcessing){

						if(endOfRotationSequence && endOfTranslationSequence){
							startProcessing = false;
							Debug.Log("Sequence Finished in: "+Time.time);
							GetComponent<AudioSource>().Stop();
							if(ReloadBtn!=null){ ReloadBtn.SetActive(true);}
							return;
						}

						//Debug.Log("Time elasped: "+(int)Time.time);
						timer+=Time.deltaTime;						
						//Slerp
							if(!endOfRotationSequence){
							
								//Debug.LogError("Slerp step:"+timer);
								
								if(timer>=1){
									//The last iteration for the current rotation
									timer = 1.0f;
									rotate();
									timer = 0.0f; //Prepping the slerp delta for the next animation
									updateRotationPoints(); // this will update the from and to quaternion rotation points
									
								}else{
									//A rotation is in progress
									rotate();
								}
							
						}
						
						//Lerp
						if(!endOfTranslationSequence){
							translate();
						}
						
						
			}
	}
private void updateRotationPoints(){
	if(slerpQueue.Count>0){
			fromQuat = toQuat;
			toQuat= slerpQueue.Dequeue();
			Debug.Log("To Rotation: "+toQuat);
			//fromQuat.Normalize();
			//toQuat.Normalize();
	}else{
		//all sequences processed
		endOfRotationSequence = true;
		Debug.Log("Rotation ended at:"+Time.time);
	}
}


private void rotate(){
	//Debug.LogError("Slerping by val: "+timer);
	transform.rotation  = Quaternion.Slerp(fromQuat,toQuat,calculateSmoothStep(timer));
}
 


int translationCount = 0;
private void translate(){
	if(lerpQueue.Count>0){
		//Debug.Log("Translation iteration"+(++translationCount));
			//transform.position= lerpQueue.Dequeue();
		Vector3 position = lerpQueue.Dequeue();
		//transform.position = Vector3.Lerp(transform.position,position,Time.deltaTime);	
		transform.position = position;
	}else{
		endOfTranslationSequence = true;
		Debug.Log("Translation ended at:"+Time.time);
	}

}

	
private List<AnimationAttr> getKeyFramesFromTextAsset(){

	List<AnimationAttr> list_AnimationAttrs = new List<AnimationAttr>(0);
	TextAsset keyframesText = Resources.Load("keyframes") as TextAsset;
	string[] keyFramesByLines = keyframesText.text.Split("\r\n".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);
	foreach (string key in keyFramesByLines)
	{
			List<float> elementdetails =new List<float>(); //t,x,y,z,xa,ya,za,angle;
			string[] sub_entries = key.Split(new char[]{',',' '},System.StringSplitOptions.RemoveEmptyEntries);
			foreach (string item in sub_entries)
			{
				elementdetails.Add(float.Parse(item.Trim()));
			}

			AnimationAttr animationAttr = new AnimationAttr(elementdetails[0],elementdetails[1],elementdetails[2],elementdetails[3],elementdetails[4],elementdetails[5],elementdetails[6],elementdetails[7]);
			//Debug.Log(animationAttr);
			list_AnimationAttrs.Add(animationAttr);
	}

	return list_AnimationAttrs;
}
public static List<Vector3> createCatmullRomSpline(List<Vector3> coords, int stepsPerTransition = 60, float tension = 1)
    {
		//Debug.Log("Pre interpolation points: "+coords.Count);
		//Debug.Log("Steps per curve: "+stepsPerTransition);
	
		List<Vector3> interpolatedPoints = new List<Vector3>(0);
		
		int pointerIndex = 0;
		while(pointerIndex<coords.Count-1){
			Vector3 prevCoord = pointerIndex==0?coords[pointerIndex]:coords[pointerIndex-1];
			Vector3 startCoord = coords[pointerIndex];
			Vector3 endCoord = coords[pointerIndex+1];
			Vector3 nextCoord = pointerIndex==coords.Count-2?coords[pointerIndex+1]:coords[pointerIndex+2];

			for(int step =0; step<=stepsPerTransition;++step){
				float deltaStep =  (float)step/stepsPerTransition;
				float deltaStepSquare = Mathf.Pow(deltaStep,2);
				float deltaStepCube = Mathf.Pow(deltaStep,3);
				
				//Now we have our degree 3 equation plugs ready, let's calculate the interpolate
				Vector3 interpolate = (-0.5f * tension * deltaStepCube 
				+ tension * deltaStepSquare - 0.5f * tension * deltaStep) * prevCoord +
                    (1 + 0.5f * deltaStepSquare * (tension - 6) 
					+ 0.5f * deltaStepCube * (4 - tension)) * startCoord +
                    (0.5f * deltaStepCube * (tension - 4) + 0.5f * tension * deltaStep 
					- (tension - 3) * deltaStepSquare) * endCoord +
                    (-0.5f * tension * deltaStepSquare + .5f * tension * deltaStepCube) * nextCoord;

				//Keep on repeating until we have all interpolate points
				interpolatedPoints.Add(interpolate);
			}
			pointerIndex++;
		}
	   return interpolatedPoints;
	}


private static float calculateSmoothStep(float currentStep){
			return currentStep*currentStep*(3.0f - 2.0f*currentStep);

	}


	public void onClick_Reload(){
		if(ReloadBtn!=null){ReloadBtn.SetActive(false);}
		endOfRotationSequence = false;
		endOfTranslationSequence = false;
		Start();
		GetComponent<AudioSource>().Play();
	}
}
