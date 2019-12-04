using UnityEngine;
using System.Collections.Generic;


public class AnimationAttr{

			public float time{get ; set;}
			public Vector3 position{get;set;}
			public Vector3 axis{get;set;}
			public float angle{get;set;}		
			public Quaternion quaternion{get;set;}

			public AnimationAttr(float time,Vector3 position,Vector3 axis,float angle){
						this.time = time;
						this.position = position;
						this.axis = axis;
						quaternion = new Quaternion(axis.x,axis.y,axis.z,angle);
			}

		    public AnimationAttr(float time, float pos_x,float pos_y,float pos_z, float xa,float ya, float za, float angle){
				this.time = time;
				//this.position = new Vector3(pos_x,pos_y,pos_z);
				this.axis = new Vector3(xa,ya,za);
				this.angle = angle;
				//this.quaternion = new Quaternion(xa,ya,za,angle);
				this.quaternion = RhsToLhs.ConvertRhsEulerToQuaternion(xa,ya,za,angle,true);
				this.position = RhsToLhs.ConvertPosToLhs(pos_x,pos_y,pos_z);
			}

			public static List<AnimationAttr> getAnimationListsFromAssets(){
					List<AnimationAttr> list_AnimationAttrs = new List<AnimationAttr>(0);
					TextAsset keyframesText = Resources.Load("keyframes_2") as TextAsset;
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

	}