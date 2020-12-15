using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atrium : MonoBehaviour
{
    public Material Main;
    public Material Alt;
    public Material Alt1;

    public float timerCountDown = 5.0f;

    private bool isPlayerColliding = false;
    private bool isPlayerCollidingenough = false;

    public float bathing=0.0f;

   

    void Update()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "Player" && bathing==0.0f)
            {
                //Debug.Log("collide");
                MeshRenderer mr = GetComponent<MeshRenderer>();
                mr.material = Alt;
                bathing = 1.0f;
                return;
            }


        }

        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "Player" && bathing>0.0f)
            {
                //Debug.Log("collide");
                bathing = 0.0f;
            }


        }

        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "Player" && bathing>0.0f)
            {
                Debug.Log("collide1");
                MeshRenderer mr = GetComponent<MeshRenderer>();
                mr.material = Alt1;
                
                return;
            }

        }



        //Debug.Log(isPlayerColliding);
    }
    
}