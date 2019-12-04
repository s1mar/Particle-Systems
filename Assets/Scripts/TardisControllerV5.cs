using UnityEngine;
using System.Collections.Generic;

public class TardisControllerV5 : MonoBehaviour {

	[SerializeField] GameObject TardisObject;
	[SerializeField] GameObject ReloadBtn;

private void Awake() {
		ReloadBtn.SetActive(false);
		if(TardisObject!=null){TardisObject.SetActive(false);}
	}
	// Use this for initialization
	void Start () {
		if(TardisObject!=null){
			TardisObject.SetActive(true);
		}
		List<AnimationAttr> listAnimations = AnimationAttr.getAnimationListsFromAssets();
		
		LerpManager lerpManager = null; SlerpManager slerpManager = null;
		if((lerpManager = gameObject.GetComponent<LerpManager>())==null)
			lerpManager = gameObject.AddComponent<LerpManager>();
		

		if((slerpManager= gameObject.GetComponent<SlerpManager>())==null)
			slerpManager = gameObject.AddComponent<SlerpManager>();

		lerpManager.Intitalize(listAnimations);
		slerpManager.Intitalize(listAnimations);

		slerpManager.StartAnimating();
		lerpManager.StartAnimating();
	
	}

	private void Update() {
		if(SlerpManager.flagComplete && LerpManager.flagComplete){
			ReloadBtn.SetActive(true);
			GetComponent<AudioSource>().Stop();
		}else{
			ReloadBtn.SetActive(false);
		}
	}
	public void onClick_Reload(){
		if(ReloadBtn!=null){ReloadBtn.SetActive(false);}
		Start();
		GetComponent<AudioSource>().Play();
	}
	
}
