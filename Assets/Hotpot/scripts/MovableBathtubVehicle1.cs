using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovableBathtubVehicle1 : MonoBehaviour
{
    public GameObject Tubs;
    public float TubSpeed = 2.0f;

    private Dictionary<GameObject, int> _tubs = new Dictionary<GameObject, int>();
    private List<Vector3> _positions = new List<Vector3>();

    private void SetTub()
    {
        for (int i = 0; i < Tubs.transform.childCount; i++)
        {
            _tubs.Add(Tubs.transform.GetChild(i).gameObject, i);
            _positions.Add(Tubs.transform.GetChild(i).transform.position);
            //Debug.Log(i);
        }
       
    }

    private void Start()
    {
        SetTub();
    }

    private void Update()
    {
        
        Vector3 position = Vector3.zero;
        if (ClickObject("MovingBath", ref position))
        {
            //Debug.Log("Click Bath");
            //if we are at obstacle limit, remove oldest obstacle
            TubSpeed = TubSpeed+2.0f;
            //Debug.Log(TubSpeed);
            if (TubSpeed >8.0f)
            {
                TubSpeed = 2.0f;
            }

            //instantiate obstacle

        }
        


        for (int i = 0; i < Tubs.transform.childCount; i++)
        {
            GameObject tub = Tubs.transform.GetChild(i).gameObject;

            //animate cars
            //when the car reaches the position, we increase the index to the next position
            //Debug.Log(tub.name); 
            //Debug.Log("_tubs[tub]: "+_tubs[tub]); 
            //Debug.Log("_positions.Count: "+_positions.Count);
            if (tub.transform.position == _positions[_tubs[tub]])
            {
                int p = _tubs[tub] + 1;
                if (p >= _positions.Count) { p = 0; }
                _tubs[tub] = p;
            }

            //move car
            //int j = _cars[car];
            Vector3 newPos = Vector3.MoveTowards(tub.transform.position,
                _positions[_tubs[tub]], //_positions[j]
                TubSpeed * Time.deltaTime);
            tub.transform.position = newPos;
        }
    }

    private bool ClickObject(string layer, ref Vector3 vec)
    {
        //guard statement if no moust button clicked
        if (!Input.GetMouseButtonDown(0)) { return false; }
        Vector3 screenPoint = Input.mousePosition; //mouse position on the screen
        Ray ray = Camera.main.ScreenPointToRay(screenPoint); //converting the mouse position to ray from mouse position
        RaycastHit hit;
        if (!Physics.Raycast(ray.origin, ray.direction, out hit)) return false; //was something hit?

        //Debug.Log(hit.transform.gameObject.layer+ "  "+LayerMask.NameToLayer(layer));
        if (hit.transform.gameObject.layer != 11) return false; //was hit on the layer?
      
        //if a layer was hit, set the camera follow and lookat
        vec = hit.point;
        return true;
    }


}