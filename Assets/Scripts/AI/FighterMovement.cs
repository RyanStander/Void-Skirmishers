using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class FighterMovement : MonoBehaviour
{
    private Transform currentTarget; //The target that the ship chooses
    private List<GameObject> targets= new List<GameObject>(); //The list of targets available
    /// <summary>
    /// The speed that the Fighter moves at
    /// </summary>
    [SerializeField]float movementSpeed = 10f;
    /// <summary>
    /// The rotational speed of a fighter
    /// </summary>
    [SerializeField] float rotationalDamp = 0.5f;

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

    [SerializeField] private Factions alliedFaction = Factions.FactionUndefined;
    void FixedUpdate()
    {
        TargetSwitching();
        Move();
        Pathfinding();
    }
    void Turn()
    {
        //turns to its current target with look rotation and smooth turning slerp
        if (currentTarget != null)
        {
            Vector3 newPos = currentTarget.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(newPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationalDamp * Time.deltaTime);
        }
    }
    void Move()
    {
        //using transform instead of force currently
        transform.position += transform.forward * movementSpeed * Time.deltaTime;
    }

    void Pathfinding()
    {
        //checks what is ahead of fighter, depending on where this objects blocks the view of the ai it will try to turn away
        RaycastHit hit;
        Vector3 raycastOffset = Vector3.zero;
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
        if (Physics.Raycast(left, transform.forward, out hit, detectionDistance))
        {
            raycastOffset += Vector3.up;
        }
        else if (Physics.Raycast(right, transform.forward, out hit, detectionDistance))
        {
            raycastOffset += Vector3.down;
        }

        if (Physics.Raycast(up, transform.forward, out hit, detectionDistance))
        {
            raycastOffset += Vector3.left;
        }
        else if (Physics.Raycast(down, transform.forward, out hit, detectionDistance))
        {
            raycastOffset -= Vector3.right;
        }

        if (raycastOffset != Vector3.zero)
        {
            //if something is blocking ther way it will try to turn away
            transform.Rotate(raycastOffset * 50f * Time.deltaTime);
        }
        else
        {
            //if nothing is blocking the way it will chase its target
            Turn();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
    }
    public Transform GetTarget()
    {
        return currentTarget;
    }
    //A function that checks for the closes target and swaps to it.
    private void TargetSwitching()
    {
        
        var query = Enum.GetValues(typeof(Factions)).Cast<Factions>().Except(new Factions[] { alliedFaction });
        //resets to closes target to first in the array
        targets = new List<GameObject>();
        foreach (Factions faction in query)
        {
            targets.AddRange(GameObject.FindGameObjectsWithTag(faction.ToString()));
        }
        
        GameObject closest;
        if (targets != null)
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


}
