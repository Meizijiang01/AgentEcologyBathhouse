using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atrium : MonoBehaviour
{
    public Material Main;
    public Material Alt;

    public void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Guest>()) return;
        Debug.Log(other.transform.parent);
        if (!other.transform.parent == this.transform) return;
        //Debug.Log(other.transform.parent);
        //Debug.Log(this);
        //GameObject light= transform.Find("bathtup_light");
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material = Alt;
       
        //guest.SetText("Inside Atrium");
        //guest.SetSlider(1);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<Guest>()) return;
       
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material = Main;

        Guest guest = other.GetComponent<Guest>();
        //guest.SetText("Outside Atrium");
        //guest.SetSlider(0);
    }
}