using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GuestMovingBath : Guest
{

    private List<Destination> _visitedBaths = new List<Destination>();
    private float _timer = 0;

    private float _wanderTimer = 2;

    private float _bathTime = 0; //how long the agent has been in the bath

    /// <summary>
    /// Called only once right after hitting Play
    /// </summary>
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        //Status = Action.RANDOM;
        //Vector3 newPos = RandomNavSphere(transform.position, 100, -1);
        //UpdateDestination(newPos);

        Status = Action.WALKING;
        UpdateDestination();
        FindPath(ref _currentConveyance, ref _destinations);
    }

    // Update is called once per frame
    public override void GuestUpdate()
    {
        if (Status == Action.RANDOM)
        {
            _timer += Time.deltaTime;
            if (_timer >= _wanderTimer)
            {
                //*/
                List<Destination> baths = GuestManager.Instance.DestinationList();
                foreach (Destination bath in baths)
                {
                    float distance = Vector3.Distance(bath.transform.position, transform.position);
                    Debug.Log(distance);
                    if (distance > 15) continue;
                    GuestWalkDestination();
                    return;
                }
                //*/

                Vector3 newPos = RandomNavSphere(transform.position, 100, -1);
                UpdateDestination(newPos);
                _timer = 0;
                _wanderTimer = Random.Range(WanderTimer.x, WanderTimer.y);
                //Debug.Log("Wander Timer: " + _wanderTimer);
            }
            return;
        }
        if (Status == Action.RIDING)
        {
            _currentConveyance.ConveyanceUpdate(this);
        }
        if (Status == Action.BATHING)
        {
            _bathTime += Time.deltaTime; //_bathTime = _bathTime + Time.deltaTime
            if (_bathTime > BathTime)
            {
                _tempDestination = Destination;
                Destination = null;

                if (Baths == 0) //if guest is done with baths
                {
                    Destination = GuestManager.Instance.RandomEntrance(this);
                }
                else //if guest needs new bath assigned
                {
                    GuestManager.Instance.AssignOpenBath(this, _visitedBaths); //Destination is assigned inside metho
                }
                if (Destination == null) return;

                SetText("Walking");
                //_tempDestination.RemoveGuest(this); //remove guest from current bath
                _destinations[0].RemoveGuest(this); //remove guest from current bath
                _destinations.RemoveAt(0); //remove current bath from destination list
                _bathTime = 0; //reseting bath time
                Status = Action.WALKING;  //start walking
                UpdateDestination(); //update new destination
                FindPath(ref _currentConveyance, ref _destinations); //finding best path
            }

            return; //so it doesn't run any code below
        }
        if (Status == Action.BATHRIDING)
        {
            //Debug.Log("BathRide");
            _bathTime += Time.deltaTime; //_bathTime = _bathTime + Time.deltaTime
            if (_bathTime > BathTime+3f)
            {
                _tempDestination = Destination;
                Destination = null;

                if (Baths == 0) //if guest is done with baths
                {
                    Destination = GuestManager.Instance.RandomEntrance(this);
                }
                else //if guest needs new bath assigned
                {
                    GuestManager.Instance.AssignOpenBath(this, _visitedBaths); //Destination is assigned inside metho
                }
                if (Destination == null) return;

                SetText("Walking");
                //_tempDestination.RemoveGuest(this); //remove guest from current bath
                _destinations[0].RemoveGuest(this); //remove guest from current bath
                _destinations.RemoveAt(0); //remove current bath from destination list
                _bathTime = 0; //reseting bath time
                Status = Action.WALKING;  //start walking
                UpdateDestination(); //update new destination
                FindPath(ref _currentConveyance, ref _destinations); //finding best path
            }

            return; //so it doesn't run any code below
        }

        //guard statement
        if (Destination == null) return; //return stops the update here until next frame

        //orient gameobject direction
        if (_agent.enabled && _agent.velocity != Vector3.zero)
        {
            Vector3 forward = _agent.velocity;
            forward.y = 0;
            transform.forward = forward;
        }
        DestinationDistance(); //++++
    }

    public override void GuestWalkDestination()
    {
        Status = Action.WALKING;
        UpdateDestination();
        FindPath(ref _currentConveyance, ref _destinations);
    }

    private void DestinationDistance()
    {
        //test agent distance from destination
        UpdateDestination();
        if (Vector3.Distance(transform.position, Destination.transform.position) < 2.1f)
        {
            if (Destination.GetComponentInParent<Conveyance>())
            {
                Status = Action.RIDING;
                _agent.enabled = false;
                _currentConveyance = Destination.GetComponentInParent<Conveyance>();
                return;
            }
            else if (Destination.tag == "Bath")
            {
                if (Destination.GetType()== typeof (Destination))
                {
                    StartBath();
                    //Debug.Log("Bath");
                    return;
                }

                //Debug.Log(Destination.name);
                if (Destination.GetType() == typeof(DestinationMovingBath))
                {
                    UpdateDestination();
                    GameObject go = Destination.transform.gameObject;
                    _agent.transform.position = go.transform.position;
                    _agent.transform.parent = go.transform;
                    //Debug.Log(go);
                    StartMovingBath(go);
                    return;
                }
                
            }
            else if (Destination.tag == "Entrance")
            {
                Destination.gameObject.GetComponent<GuestManager>().GuestExit(this);
                //GuestManager manager = Destination.gameObject.GetComponent<GuestManager>();
                //manager.GuestExit(this);
                return;
            }
            
        }
    }

    /// <summary>
    /// Update the agents destination and make sure the agent isn't stopped
    /// </summary>
    private void UpdateDestination()
    {
        _agent.SetDestination(Destination.transform.position);
        _agent.isStopped = false;
    }

    private void UpdateDestination(Vector3 position)
    {
        Debug.Log("Yes");
        _agent.SetDestination(position);
        _agent.isStopped = false;
    }

    public override void NextDestination(Destination destination = null)
    {
        _agent.enabled = true;
        _destinations.RemoveAt(0);
        Destination = _destinations[0];
        Status = Action.WALKING;
        FindPath(ref _currentConveyance, ref _destinations); //this allows multiple conveyances
    }

   
   
    /// <summary>
    /// Start bath by changing agent status and stopping the agent
    /// </summary>
    private void StartBath()
    {
        Baths--;
        _visitedBaths.Add(Destination);
        Status = Action.BATHING;
        _agent.isStopped = true;
        SetText("Bathing");
    }

    private void StartMovingBath(GameObject go)
    {
        Baths--;
        _visitedBaths.Add(Destination);
        Status = Action.BATHRIDING;
        _agent.enabled = true;
        _agent.transform.position = go.transform.position;
        //_agent.transform.parent = go.transform;
        _agent.isStopped = true;
        _agent.transform.SetParent(go.transform, true);
        //Debug.Log(_agent.transform.parent);
        SetText("BathRiding");
    }

    public override Destination GetUltimateDestination()
    {
        if (_destinations.Count == 0) return null;
        return _destinations[_destinations.Count - 1];
    }

    public override void SetText(string text)
    {
        if (Text == null) return;
        Text.text = text;
    }

    public override void SetSlider(float i)
    {
        if (Slider == null) return;
        Slider.value = i;
    }

    public override string GetText()
    {
        if (Slider == null) return string.Empty;
        return Text.text;
    }

    public override float GetSliderValue()
    {
        if (Slider == null) return Mathf.Infinity;
        return Slider.value;
    }

    public override List<Destination> VisitedBaths()
    {
        return _visitedBaths;
    }

   
}