using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PaternosterRoller : Conveyance
{
    public GameObject Cars;
    public int MaxLoad;

    private Destination[] _destinations;
    private Dictionary<GameObject, int> _cars = new Dictionary<GameObject, int>();
    private List<Vector3> _positions = new List<Vector3>();
    private Dictionary<Guest, Vector3> _guests = new Dictionary<Guest, Vector3>();
    private Dictionary<GameObject, List<Guest>> _carRiders = new Dictionary<GameObject, List<Guest>>(); //keeps track of which cars have riders
    private List<Guest> _riders = new List<Guest>();
    private Dictionary<GameObject, float> _waitTime = new Dictionary<GameObject, float>();

    //car states
    public enum State { MOVING, WAITING };
    public State CurrentState = State.WAITING;
    private float _maxWait = 1.0f;
    //private float _waitTime = 0.0f;

    private void Start()
    {
        SetDestination();

        //initialize waittime
        for (int i = 0; i < Cars.transform.childCount; i++)
        {

            _waitTime.Add(Cars.transform.GetChild(i).gameObject, i);
        }

        List<GameObject> Keys = _waitTime.Keys.ToList();
        foreach (GameObject car in Keys)
        {
            _waitTime[car] = _maxWait;
        }
    }


    public override void SetDestination()
    {

        _destinations = GetComponentsInChildren<Destination>();


        //create the positions dictionary
        for (int i = 0; i < Cars.transform.childCount; i++)
        {
            _cars.Add(Cars.transform.GetChild(i).gameObject, i);
            _positions.Add(Cars.transform.GetChild(i).transform.position);
            _carRiders.Add(Cars.transform.GetChild(i).gameObject, new List<Guest>());
        }

        foreach (GameObject car in _cars.Keys)
        {
            _carRiders[car] = new List<Guest>();
        }

        //set the occupnacy limit for each waiting lobby
        foreach (Destination destination in _destinations)
        {
            destination.OccupancyLimit = 0;
        }
    }

    private bool SameSign(float x, float y)
    {
        return (x >= 0) ^ (y < 0);
    }

    private void Update()
    {
        if (_guests.Count == 0) return;
        //loading or unloading guests
        for (int i = 0; i < Cars.transform.childCount; i++)
        {
            
            GameObject car = Cars.transform.GetChild(i).gameObject;
           
            //timer until car starts moving
            if (_waitTime[car] <= 0)
            {
                CurrentState = State.MOVING;
            }
            else
            {
                _waitTime[car] -= Time.deltaTime;
                return;
            }

            //if car is not full
            if (_carRiders[car].Count< MaxLoad)
            {
                float carDirection = _positions[_cars[car]].y - car.transform.position.y;//telling if the car is going up or down
                foreach (KeyValuePair<Guest, Vector3> kvp in _guests)
                {
                    Guest guest = kvp.Key;
              
                    //guard statements
                    if (_riders.Contains(guest)) continue; //make sure guest doesn't move between cars
               
                    if (Mathf.Abs(car.transform.position.y - guest.transform.position.y-car.transform.localScale.y) > 1.2f) continue;

                    //test guest direction
                    float guestDirection = kvp.Value.y - guest.transform.position.y;
                    if (!SameSign(carDirection, guestDirection)) continue; //continue to next guest

                    //load guest
                    _riders.Add(guest);
                    _carRiders[car].Add(guest);//_carRiders[car] = guest;_need to be changed into guest list##############################################
                    IEnumerator coroutine = LoadPassenger(car, guest);
                    StartCoroutine(coroutine);
                    if (_carRiders[car].Count >= MaxLoad)
                    {
                        break;//if exceed maximum load, don't check any more guests
                    }

                    else { continue; }//not exceeding maximun load, continue loading guests
                     
                }
            }
            
            //check if guest has arrived at level
            else
            {
                //Debug.Log(_carRiders[car].Count);
                if (_carRiders[car] == null) { continue; }
                for (int j = 0; j < _carRiders[car].Count; j++)
                {
                    if (j > _guests.Count) { continue; }
                    
                    Guest guest = _carRiders[car][j];//get the item from the list!!!!!!!!!!!!!#################################################
                    Vector3 UnloadPosition = _guests[guest];
                    if (Mathf.Abs(UnloadPosition.y - guest.transform.position.y - car.transform.localScale.y) < 0.2f)
                    {
                        //unload guest
                        //Debug.Log("unloadGuest");
                        _carRiders[car].Remove(guest);
                        IEnumerator coroutine = UnloadPassenger(car, guest);
                        StartCoroutine(coroutine);
                    }

                }
               
            }

            //move car
            if (CurrentState == State.MOVING)
            {
                int q = _cars[car] + 1;
                if (q >= _positions.Count) { q = 0; }
                float CarSpeed = Mathf.Abs(Vector3.Distance(_positions[_cars[car]], _positions[q])) / 6f;

                //when the car reaches the position, we increase the index to the next position
                if (Mathf.Abs(Vector3.Distance(car.transform.position , _positions[_cars[car]]))<=0.02f)
                {
                    int p = _cars[car] + 1;
                    if (p >= _positions.Count) { p = 0; }
                    _cars[car] = p;
                    //Debug.Log(p);
                }

               
                Vector3 newPos = Vector3.MoveTowards(car.transform.position,
                _positions[_cars[car]], 
                CarSpeed * Time.deltaTime);
                car.transform.position = newPos;
                Debug.DrawLine(car.transform.position, _positions[_cars[car]]);

            }
            if (Mathf.Abs(Vector3.Distance(car.transform.position, _positions[_cars[car]])) <= 0.02f)
            {
                _waitTime[car] = _maxWait;
                CurrentState = State.WAITING;
            }
            //return;

        }
        
       
    }

    private IEnumerator LoadPassenger(GameObject car, Guest guest)
    {
        bool loading = true;
        while (loading)
        {
            guest.transform.position = Vector3.MoveTowards(guest.transform.position,
                car.transform.position,
                Time.deltaTime * Speed * 8);

            if (Vector3.Distance(guest.transform.position, car.transform.position) < 0.01f) { loading = false; }
            yield return new WaitForEndOfFrame();
        }

        guest.transform.parent = car.transform;
        yield break;
    }

    private IEnumerator UnloadPassenger(GameObject car, Guest guest)
    {
        bool unloading = true;
        while (unloading)
        {
            guest.transform.position = Vector3.MoveTowards(guest.transform.position,
                _guests[guest],
                Time.deltaTime * Speed * 8);

            if (Vector3.Distance(guest.transform.position, _guests[guest]) < 0.01f) { unloading = false; }
            yield return new WaitForEndOfFrame();
        }

        _riders.Remove(guest);
        _guests.Remove(guest);
        guest.transform.parent = null;
        guest.NextDestination();
        //Debug.Log("unloadGuest");
        //Debug.Log(_carRiders[car].Count);

        yield break;
    }

    public override void ConveyanceUpdate(Guest guest)
    {
        //guard statement if guest is already added
        if (_guests.ContainsKey(guest)) return;

        Destination destination = guest.GetUltimateDestination();//bath
        destination = GetDestination(destination.transform.position);//floor
        _guests.Add(guest, destination.transform.position);//add floor as destination
    }

    public override Destination GetDestination(Vector3 vec)
    {
        Destination[] tempDestinations = _destinations;
        tempDestinations = tempDestinations.OrderBy(go => Mathf.Abs(go.transform.position.y - vec.y)).ToArray();
        //tempDestinations = tempDestinations.OrderBy(x => x.name).ToArray();
        //tempDestinations = tempDestinations.OrderBy(x => Vector3.Distance(x.transform.position, Vector3.zero)).ToArray();
        return tempDestinations[0];
    }

    public override Vector3 StartPosition(Vector3 vec)
    {
        if (_destinations.Length == 0) { return Vector3.zero; }
        Destination destination = GetDestination(vec);
        return destination.transform.position;
    }

    public override Vector3 EndPosition(Vector3 vec)
    {
        if (_destinations.Length == 0) { return Vector3.zero; }
        Destination destination = GetDestination(vec);
        return destination.transform.position;
    }

    public override float WeightedTravelDistance(Vector3 start, Vector3 end)
    {
        float distance = 0;
        //guard statement
        if (_destinations.Length < 2) return distance;

        //get the total path distance
        Destination go1 = GetDestination(start);
        Destination go2 = GetDestination(end);
        distance = Vector3.Distance(go1.transform.position, go2.transform.position);

        //we scale the distance by the weight factor
        distance /= Weight;
        return distance;
    }
}