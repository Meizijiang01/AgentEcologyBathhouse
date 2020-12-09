using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doubleheight : MonoBehaviour
{

    public Material Alt;
    public float guestnumber = 0;
    // public Action Status;
    // public enum Action { BATHING, WALKING, FOLLOWING, RIDING, RANDOM }
    

    public void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Guest>()) return;
    


        Guest guest = other.GetComponent<Guest>();
        if (guest.Status == Guest.Action.BATHING) { other.GetComponent<Guest>().GuestWalkDestination(); }
        guestnumber++;
    
        MeshRenderer mr = other.GetComponent<MeshRenderer>();
        mr.material = Alt;
        guest.SetText("VIP BATHING!");
   
        guest.SetSlider(guestnumber);

    }

    

    public void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<Guest>()) return;

    }
}
