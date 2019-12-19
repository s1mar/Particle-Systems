using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cushion : MonoBehaviour
{
   Vector3 minExtents;
   Vector3 maxExtents;
   public float MinX{get; private set;} 
   public float MaxX{get; private set;}
   public float MinY{get; private set;} 
   public float MaxY{get; private set;}
   public float MaxZ{get; private set;}
   public float MinZ{get; private set;}
   public Vector3 center{get; private set;}

    void Start(){
        Renderer rend = GetComponent<Renderer>();
        minExtents = rend.bounds.min;
        maxExtents = rend.bounds.max;
        MinX = minExtents.x;
        MaxX = maxExtents.x;
        MinY = minExtents.y;
        MaxY = maxExtents.y;
        MinZ = minExtents.z;
        MaxZ = maxExtents.z;
        center = rend.bounds.center;
    }


}
