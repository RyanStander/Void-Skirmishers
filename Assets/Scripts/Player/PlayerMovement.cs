using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ship Attributes")]
    //Points at which the force of movement can be applied to
    [SerializeField]
    private Transform forwardPropulsionPoint=null, backwardPropulsionPoint=null;

    //Power for the vehicle to move
    

    [Range(0, 15)]
    [SerializeField] private int steeringPower = 15;
    [Range(250, 1000)]
    [SerializeField] private int forwardPower = 500, backwardsPower = 500, verticalPower=400;

    //Rate at which the velocities will be reduced over time
    [SerializeField]
    [Range(0.8f, 0.99f)]
    private float angledSlowdownRate = 0.99f, generalSlowdownRate = 0.99f;

    [SerializeField]
    [Range(0, 0.5f)]
    private float maximumAngularVelocity =0.25f;

    //Initialized and important for movement
    private Rigidbody rBody;

    public void Awake()
    {
        //boostTimeRemaining = boostTime;
        rBody = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        playerMovement();
    }

    private void playerMovement()
    {
        //Direction of steer force
        float steer = 0;

        //Forward vector
        var forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward);
        var upward = Vector3.Scale(new Vector3(0, 1, 0), transform.up);

        //Force that gets applied at the end to the vehicle 
        Vector3 horizontalForceToApply = new Vector3(0, 0, 0);
        Vector3 verticalForceToApply = new Vector3(0, 0, 0);

        //The point at which these forces are applied
        Vector3 propulsionPoint = forwardPropulsionPoint.position;

        var localVelocity = transform.InverseTransformDirection(rBody.velocity);

        //Upwards and downwards 
        if (Input.GetKey(KeyCode.Q))
        {
            verticalForceToApply = upward * verticalPower;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            verticalForceToApply = -upward * verticalPower;
        }

        //Forwards
        if (Input.GetAxis("Vertical") > 0)
        {
            //Update force to apply
            horizontalForceToApply = forward * forwardPower;
        }
        //Backwards
        if (Input.GetAxis("Vertical") < 0)
        {
            //Update force to apply
            horizontalForceToApply = -forward * backwardsPower;

            //Change propulsion point being used
            propulsionPoint = backwardPropulsionPoint.position;
        }

        //Turning direction
        if (Input.GetAxis("Horizontal") < 0) steer = -1;
        else if (Input.GetAxis("Horizontal") > 0) steer = 1;

        //Apply forces to relevant points
        //rBody.AddForceAtPosition(steer * transform.right * steeringPower * localVelocity.z, propulsionPoint);
        transform.Rotate((steer * transform.up * steeringPower * localVelocity.z)/200);
        rBody.AddForceAtPosition(horizontalForceToApply, propulsionPoint);
        rBody.AddForce(verticalForceToApply);

        //Slows down the vehicle and limits
        rBody.angularVelocity *= angledSlowdownRate;
        rBody.velocity *= generalSlowdownRate;
        rBody.maxAngularVelocity = maximumAngularVelocity;
    }
}
