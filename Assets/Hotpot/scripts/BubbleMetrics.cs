using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BubbleMetrics : MonoBehaviour
{

    public Material Alt;
    public float intense = 0;
    public void OnTriggerEnter(Collider other)
    {
        if (other != GameObject.Find("Guest(Clone)"))
        {
            
            MeshRenderer mr = GetComponent<MeshRenderer>();
            mr.material = Alt;
          
            DrawSpheres();
            intense++;
                //Debug.Log(intense);
            /**
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Debug.Log("sphere");

            var renderer = sphere.GetComponent<Renderer>();

            renderer.sharedMaterial = Alt;
            **/

        }


    }




    public void DrawSpheres()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = this.transform.position;
        sphere.transform.localScale = new Vector3(1f, 1f, 1f);
        
        
    }
}
