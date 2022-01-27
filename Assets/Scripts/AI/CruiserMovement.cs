using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CruiserMovement : MonoBehaviour
{
    [Header("Ship Attributes")]
    //Points at which the force of movement can be applied to
    public Transform forwardPropulsionPoint;

    //Power for the vehicle to move
    [Range(1, 25)]
    public int steeringPower = 25;
    [Range(25, 300)]
    public int forwardPower = 50, verticalPower=40;

    //Rate at which the velocities will be reduced over time
    [Range(0.8f, 0.99f)]
    public float angledSlowdownRate = 0.99f, generalSlowdownRate = 0.99f;

    [Range(0, 0.5f)]
    public float maximumAngularVelocity =0.25f;

    //Initialized and important for movement
    private Rigidbody rBody;

    [Header("Pathfinding")]
    private Transform currentTarget;
    private List<GameObject> targets= new List<GameObject>();

    /// <summary>
    /// The distance that a fighter looks for objects to avoid hitting
    /// </summary>
    [SerializeField] float detectionDistance = 20f;
    /// <summary>
    /// The offset on Y axis for the raycast used for how far up and down it looks for objects to avoid
    /// </summary>
    [SerializeField] float rayCastYOffset = 2.5f;
    /// <summary>
    /// The Offset on X axis for the raycast used for how far left and right it looks for objects to avoid
    /// </summary>
    [SerializeField] float rayCastXOffset = 2.5f;
    /// <summary>
    /// the origin of the raycast, move further forward if you dont want it to avoid things on its sides or behind it (it shouldnt look for things behind it)
    /// </summary>
    [SerializeField] float startPoint = 5f;

    [Header("Object Distances")]
    [Range(100,700)]
    [SerializeField] private int LookAtTargetDistance = 700;
    [Range(100, 500)]
    [SerializeField] private int FarCircleDistance = 400;
    [Range(100, 300)]
    [SerializeField] private int CloseCircleDistance = 200;

    [SerializeField] private Factions alliedFaction = Factions.FactionUndefined;
    public void Awake()
    {
        //boostTimeRemaining = boostTime;
        rBody = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        //playerMovement();
        TargetSwitching();
        Movement();
        Pathfinding();
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0);
    }


    private void TurnToTarget()
    {



        if (currentTarget != null)
        {
            if (Vector3.Distance(transform.position, currentTarget.transform.position) > LookAtTargetDistance)
            {
                Vector3 newPos = currentTarget.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(newPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.5f * Time.deltaTime);
            }
            else if (Vector3.Distance(transform.position, currentTarget.transform.position) > FarCircleDistance)
            {
                Vector3 newPos = currentTarget.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(newPos) * Quaternion.Euler(0, 60, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.5f * Time.deltaTime);
            }else if (Vector3.Distance(transform.position, currentTarget.transform.position) > CloseCircleDistance)
            {
                Vector3 newPos = currentTarget.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(newPos) * Quaternion.Euler(0, 90, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.5f * Time.deltaTime);
            }


                if (currentTarget.position.y < gameObject.transform.position.y)
            {
                rBody.AddForce(-Vector3.Scale(new Vector3(0, 1, 0), transform.up) * verticalPower);
            }
            else if(currentTarget.position.y > gameObject.transform.position.y)
            {
                rBody.AddForce(Vector3.Scale(new Vector3(0, 1, 0), transform.up) * verticalPower);
            }
            
        }
    }

    private void Movement()
    {
        var forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward);
        Vector3 horizontalForceToApply = forward * forwardPower;
        rBody.AddForceAtPosition(horizontalForceToApply, forwardPropulsionPoint.position);
    }

    private void Pathfinding()
    {
        //checks what is ahead of fighter, depending on where this objects blocks the view of the ai it will try to turn away
        RaycastHit hit;

        //Direction of steer force
        float steer = 0;

        var localVelocity = transform.InverseTransformDirection(rBody.velocity);

        //used for going up and down
        var verticalMovement = Vector3.Scale(new Vector3(0, 1, 0), transform.up);
        Vector3 verticalForceToApply = Vector3.zero;
        /*these are the points i am raycasting from to check where
        collisions are happening                                */
        Vector3 left = transform.position - transform.right * rayCastXOffset - transform.forward * startPoint;
        Vector3 right = transform.position + transform.right * rayCastXOffset - transform.forward * startPoint;
        Vector3 up = transform.position + transform.up * rayCastYOffset - transform.forward * startPoint;
        Vector3 down = transform.position - transform.up * rayCastYOffset - transform.forward * startPoint;

        //Shows the raycast lines of the ships in the scene view
        Debug.DrawRay(left, transform.forward * detectionDistance, Color.yellow);
        Debug.DrawRay(right, transform.forward * detectionDistance, Color.yellow);
        Debug.DrawRay(up, transform.forward * detectionDistance, Color.yellow);
        Debug.DrawRay(down, transform.forward * detectionDistance, Color.yellow);

        //Checks which direction the ship should turn to avoid hitting objects
        //checks if object is to the left
        if (Physics.Raycast(left, transform.forward, out hit, detectionDistance))
        {
            //turning right
            steer = 1;
        }
        //checks if object is to the right
        else if (Physics.Raycast(right, transform.forward, out hit, detectionDistance))
        {
            //turning left
            steer = -1;
        }

        //checks if object is above
        if (Physics.Raycast(up, transform.forward, out hit, detectionDistance))
        {
            verticalForceToApply = -verticalMovement * verticalPower;
        }
        //checks if object is below
        else if (Physics.Raycast(down, transform.forward, out hit, detectionDistance))
        {
            verticalForceToApply = verticalMovement * verticalPower;
        }


        //Apply forces to relevant points
        //rBody.AddForceAtPosition(steer * transform.right * steeringPower * localVelocity.z, propulsionPoint);
        transform.Rotate((steer * transform.up * steeringPower * localVelocity.z) / 200);
        rBody.AddForce(verticalForceToApply);


        //if there are no objects in the cruisers path, it will continue following its target
        if (steer==0||verticalForceToApply==Vector3.zero)
        {
            TurnToTarget();
        }

        //Slows down the vehicle and limits
        rBody.angularVelocity *= angledSlowdownRate;
        rBody.velocity *= generalSlowdownRate;
        rBody.maxAngularVelocity = maximumAngularVelocity;
        //Sets the X axi to 0 to prevent weird turning
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, eulerRotation.y, eulerRotation.z);
    }

    private void TargetSwitching()
    {
        //resets to closes target to first in the array
        targets = new List<GameObject>();
        //Gets faction types excluding the allided ones
        var query = Enum.GetValues(typeof(Factions)).Cast<Factions>().Except(new Factions[] { alliedFaction });
        foreach (Factions faction in query)
        {
            targets.AddRange(GameObject.FindGameObjectsWithTag(faction.ToString()));
        }
        GameObject closest;
        if (targets.Count>0)
        {
            closest = targets[0];
            currentTarget = closest.transform;

            //goes through the array of targets and tries to find the closest one, comparing based on distance
            foreach (GameObject target in targets)
            {
                if (Vector3.Distance(transform.position, target.transform.position) < Vector3.Distance(transform.position, closest.transform.position))
                {
                    closest = target;
                    currentTarget = target.transform;
                }
            }
        }
        else
        {
            Debug.Log("No enemies to attack");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
    }

    public Transform GetTarget()
    {
        return currentTarget;
    }
}
