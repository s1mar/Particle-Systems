
using UnityEngine;

public class RhsToLhs{

	public static Vector3 ConvertPosToLhs(float x, float y, float z)
	{

			return new Vector3(-x,y,z);

	}
	
	public static Vector3 ConvertPosToLhs(Vector3 rhsPos){

		return new Vector3(-rhsPos.x,rhsPos.y,rhsPos.z);
	}

	public static Quaternion ConvertRhsEulerToQuaternion(float xa, float ya, float za, float angle, bool normalize=true){
		angle = 2*Mathf.Sin(angle);
		Quaternion quaternion = new Quaternion(xa,-ya,-za,angle);
		if(normalize){quaternion.Normalize();}
		return quaternion;
	}

	public static Quaternion ConvertRhsToLHSQuaternion(Quaternion quaternion, bool normalize = true){
		float angle = 2*Mathf.Sin(quaternion.w);
		Quaternion quaternionLHS = new Quaternion(quaternion.x,-quaternion.y,-quaternion.z,angle);
		if(normalize){quaternionLHS.Normalize();}
		return quaternionLHS;
	}
}
