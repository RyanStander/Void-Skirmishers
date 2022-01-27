using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionRespawnManager : MonoBehaviour
{
    private List<GameObject> respawnPoints = new List<GameObject>();
    private Queue<ShipData> timerQueuedShips = new Queue<ShipData>();
    private Queue<ShipData> queuedShips = new Queue<ShipData>();

    [Header("Respawn Attributes")]
    [SerializeField] private Factions faction = Factions.FactionUndefined;
    [Range(0,10)]
    [SerializeField] private int respawnDelay = 3;

    private void OnEnable()
    {
        EventManager.Subscribe(EventType.RESPAWNSHIPEVENT, OnIncomingShipData);
    }
    private void OnDisable()
    {
        EventManager.Unsubscribe(EventType.RESPAWNSHIPEVENT, OnIncomingShipData);
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateRespawnPoints();
    }
    private void UpdateRespawnPoints()
    {
        //Updates valid respawn points
        var children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform point in children)
        {
            if (point.tag is "RespawnPoint")
            {
                respawnPoints.Add(point.gameObject);
            }
        }
        if(respawnPoints.Count==0)
        {
            throw new System.Exception("Respawn points are 0!, make sure that there are respawn points contained in "+this.gameObject+"'s hierarchy or that they are correctly tagged!");
        }
    }
    private void Update()
    {
        HandleQueue();
    }
    //Handles the queue
    private void HandleQueue()
    {
        //QueuedShips has ships to send out
        if (timerQueuedShips.Count != 0)
        {
            StartCoroutine(HandleRespawnTimed(timerQueuedShips.Dequeue()));
        }

        if (queuedShips.Count != 0)
        {
            HandleRespawn(queuedShips.Dequeue());
        }
    }
    //Courotinue that helps respawn and rebuild ships
    IEnumerator HandleRespawnTimed(ShipData givenData)
    {
        //Waits until respawn delay is over
        yield return new WaitForSeconds(respawnDelay);

        //Then calls the indended respawn
        HandleRespawn(givenData);
    }
    private void HandleRespawn(ShipData givenData)
    {
        //Waits until a respawn point is free
        GameObject potentialRespawnPoint = GetRespawnPoint();
        if(potentialRespawnPoint==null)
        {
            //Requeue the ship until a spawn point frees up
            queuedShips.Enqueue(givenData);
        }
        else
        {
            //Spawn the object with the correct data and faction
            potentialRespawnPoint.GetComponent<RespawnPoint>().SpawnShip(givenData);
        }
    }
    private GameObject GetRespawnPoint()
    {
        //If is currently available for respanwning
        foreach (GameObject point in respawnPoints)
        {
            //Return the point
            if (point.GetComponent<RespawnPoint>().isAvailable)
            {
                //Sets point to unavailable
                point.GetComponent<RespawnPoint>().isAvailable = false;

                //Returns the point as a valid spawn location
                return point;
            }
        }
        //Else return as null
        return null;
    }
    private void OnIncomingShipData(EventData eventData)
    {
        //Cast and error handling to make sure that the correct type of EventData is being recieved
        if (eventData is RespawnShipEventData buyEventData)
        {
            if (buyEventData.shipData.isRespawnable == true)
            {
                //If ship matches manager faction, then 
                if (IsFactionShip(buyEventData.shipData))
                {
                    timerQueuedShips.Enqueue(buyEventData.shipData);
                }
                //If no, then do nothing
            }
        }
        else
        {
            //Unity Player
            Debug.Log("Warning: Given EventData is not the same as the permitted RespawnShipEventData");
        }
    }
    private bool IsFactionShip(ShipData givenData)
    {
        //Checks if the recieved ship data corresponds to this faction
        if(givenData.faction == faction) { return true; }
        else { return false; }
    }
    public Factions CurrentFaction()
    {
        return faction;
    }

    //Queues up ship data
    public void EnqueueShipData(ShipData shipData)
    {
        queuedShips.Enqueue(shipData);
    }
}
