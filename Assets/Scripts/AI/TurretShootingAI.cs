using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TurretShootingAI : MonoBehaviour
{
    [Header("Turret Attributes")]
    [SerializeField] float turnSpeed = 10;
    [SerializeField] Transform turretBody = null;
    [SerializeField] Transform turretHead = null;
    [SerializeField] GameObject bullet = null;
    [SerializeField] GameObject conePoint = null;

    private List<Transform> turretFirePoints = new List<Transform>();
    private Transform activeFirePoint;

    private List<GameObject> targets = new List<GameObject>();

    [Range(100, 1000)]
    public int turretRange = 500;
    [Range(0,25)]
    [SerializeField] private int bulletStartDistance = 0;
    [Range(0, 90)]
    public int maximumVerticalAngle = 45, maximumHorizontalAngle = 45;

    private AudioSource audioSource;

    public bool targetSpotted = false;
    public bool isLeadingTurret;

    public Factions alliedFaction=Factions.FactionUndefined;

    void Start()
    {
        //Gets the audio source
        audioSource = transform.GetComponent<AudioSource>();

        //Allows for multiple firing points if multiple exist
        var firingPoints = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform point in firingPoints)
        {
            //Finds all firing points within the object
            if (point.tag is "FiringPoint")
            {
                turretFirePoints.Add(point.transform);
            }
        }
    }

    void FixedUpdate()
    {
        //Only run if the leading turret
        if(isLeadingTurret)
        {
            //Gets all valid and potential targets
            UpdateTargets();
            //Update firing points
            UpdateFiringPoints();
            //Raycasts to targets and checks for target
            RaycastingAI();
        }
    }

    private void UpdateTargets()
    {
        //Clears targets list
        targets.Clear();

        //Gets faction types excluding the allided ones
        var query = Enum.GetValues(typeof(Factions))
            .Cast<Factions>()
            .Except(new Factions[] {alliedFaction});

        //Adds all opposing factions as valid targets
        foreach (Factions faction in query)
        {
            //If faction is currently present
            if(GameObject.FindWithTag(faction.ToString())!=null)
            {
                //Then add all objects with that tag to the targets
                targets.AddRange(GameObject.FindGameObjectsWithTag(faction.ToString()));
            }
        }
    }
    public void RaycastingAI()
    {
        //Checks that targets is not empty, if so then return early
        if(targets.Count<1) { return; }

        //Declares temporary closest game object to be filtered
        GameObject closestTarget = targets[0];

        //Gets the closest target to the turret based on distance (and if it is visible)
        foreach (GameObject target in targets)
        {
            //If target is visible, within range and rotation
            if(IsTargetVisible(target) && isWithinRotation(target))
            {
                //If closer than the previous found closer target
                if (Vector3.Distance(activeFirePoint.position, target.transform.position) < Vector3.Distance(transform.position, closestTarget.transform.position))
                {
                    //Sets the new closest target
                    closestTarget = target;
                }
            }
        }

        //If closest target is still valid, then consider it spotted
        if (IsTargetVisible(closestTarget) && isWithinRotation(closestTarget))
        {
            targetSpotted = true;
            RotateTowards(closestTarget);
        }
        else
        {
            targetSpotted = false;
        }
    }

    public float CombineSingleEulerAxis(float firstDegree, float secondDegree)
    {
        //First gets the difference between both degrees
        float degree = (float)(firstDegree - secondDegree);
        //Then adds 360 if negative, as this will allow it to be on a 360 circle scale
        //if (degree < 0) degree += 360f;

        //However, as an adjusted and specific degree style is needed, the following is used instead
        if (degree > 0) degree -= 360f;

        return degree * -1;
    }


    private bool IsTargetVisible(GameObject target)
    {
        Vector3 origin = conePoint.transform.position; // The origin of the ray
        Vector3 direction = target.transform.position; //The target spot
        RaycastHit hitInfo;
        float distanceToTarget = Vector3.Distance(direction, origin);

        if (distanceToTarget <= turretRange) //If within range, then draw ray and check for hit
        {
            Ray rayToTargetObject = new Ray(origin, direction - origin); //Sets out ray
            if (Physics.Raycast(rayToTargetObject, out hitInfo))
            {
                //If ray hits valid target return as true, else can not see target
                if (hitInfo.collider.gameObject == target)
                {
                    Debug.DrawRay(origin, direction - origin, Color.green);
                    return true;
                }
                else
                {
                    Debug.DrawRay(origin, direction - origin, Color.red);
                    return false;
                }
            }
        }
        return false;
    }
    private void UpdateFiringPoints()
    {
        //If more than one fire point is present, it will search from the point inbetween them by averagin their positions
        if (turretFirePoints.Count > 1)
        {
            //Total XY so that
            Vector3 totalXYZ = new Vector3(0, 0);
            foreach (var point in turretFirePoints)
            {
                totalXYZ += new Vector3(point.transform.position.x, point.transform.position.y, point.transform.position.z);
            }
            activeFirePoint.position = totalXYZ / turretFirePoints.Count;
        }
        //Else it will use the singular position, although the previous if statement could be used this is less processing
        else
        {
            activeFirePoint = turretFirePoints[0];
        }
    }
    public void Fire(string givenUserID, string givenTeamTag)
    {
        foreach (Transform point in turretFirePoints)
        {
            GameObject newProjectile = Instantiate(bullet, point.position-transform.right* bulletStartDistance, point.rotation);

            //If projectile is normal
            if (newProjectile.GetComponent<Shooting>() != null)
            {
                newProjectile.GetComponent<Shooting>().SetShooterValues(givenUserID, givenTeamTag);
            }
            
            //If projectile is laser beam
            if (newProjectile.GetComponent<LaserBeam>()!=null)
            {
                newProjectile.GetComponent<LaserBeam>().SetShooterValues(givenUserID, givenTeamTag);
            }
        }
    }

    protected void RotateTowards(GameObject to)
    {
        //This function makes it so that the turret will rotate towards the designated target and start shooting
        //if there is no body it will designate all movement to the head of the turret
        if (turretBody != null)
        {
            //----------------------------
            //    Turret Body Movement
            //----------------------------
            Vector3 relativeLookPosition = to.transform.position - turretBody.transform.position;

            //Calculate target rotation
            Quaternion turretBodyTargetRotation = Quaternion.LookRotation(relativeLookPosition) * Quaternion.Euler(270, 0, 90);

            //Set target rotations
            turretBody.transform.rotation = Quaternion.Slerp(turretBody.transform.rotation, turretBodyTargetRotation, Time.deltaTime * turnSpeed);
            turretBody.transform.eulerAngles = new Vector3(270, turretBody.transform.eulerAngles.y, turretBody.transform.eulerAngles.z);

            //----------------------------
            //    Turret Head Movement
            //----------------------------
            Vector3 lookDirection = to.transform.position - turretHead.transform.position;

            //Calculate target rotation
            Quaternion turretTargetRotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(270, 0, 90);

            //Set target rotation
            turretHead.transform.rotation = Quaternion.Slerp(turretHead.transform.rotation, turretTargetRotation, Time.deltaTime * turnSpeed);
        }
        else
        {
            //----------------------------
            // Turret Head Movement Only
            //----------------------------
            Vector3 lookDirection = to.transform.position - turretHead.transform.position;
            Quaternion turretTargetRotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0, 270, -10);
            turretHead.transform.rotation = Quaternion.Slerp(turretHead.transform.rotation, turretTargetRotation, Time.deltaTime * turnSpeed);
        }
    }

    private bool isWithinRotation(GameObject potentialTarget)
    {
        //These transforms allows for saving previous information if the rotation is not viable
        Vector3 previousHeadRotationEuler = turretHead.transform.localEulerAngles;
        Vector3 previousBodyRotationEuler = turretBody.transform.localEulerAngles;

        //----------------------------
        //    Turret Head Estimation
        //----------------------------
        Vector3 lookDirection = potentialTarget.transform.position - turretHead.transform.position;

        //Calculate target rotation
        Quaternion turretTargetRotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(270, 0, 90);

        //Set target rotation
        turretHead.transform.rotation = Quaternion.Slerp(turretHead.transform.rotation, turretTargetRotation, Time.deltaTime * turnSpeed);

        if (turretBody != null)
        {
            //----------------------------
            //    Turret Body Estimation
            //----------------------------
            Vector3 relativeLookPosition = potentialTarget.transform.position - turretBody.transform.position;
            Quaternion turretBodyTargetRotation = Quaternion.LookRotation(relativeLookPosition) * Quaternion.Euler(270, 0, 90);

            //Set target rotations
            turretBody.transform.rotation = Quaternion.Slerp(turretBody.transform.rotation, turretBodyTargetRotation, Time.deltaTime * turnSpeed);
            turretBody.transform.eulerAngles = new Vector3(270, turretBody.transform.eulerAngles.y, turretBody.transform.eulerAngles.z);

            //Checks if body rotation is within limits
            if (turretBody.transform.localEulerAngles.y < maximumHorizontalAngle || turretBody.transform.localEulerAngles.y > 360 - maximumHorizontalAngle)
            {
                if (turretHead.transform.localEulerAngles.y < maximumVerticalAngle || turretHead.transform.localEulerAngles.y > 360 - maximumVerticalAngle)
                {
                    //Reset
                    UpdateLocalEulerAngles(previousBodyRotationEuler, previousHeadRotationEuler);
                    return true;
                }
            }
        }
        //If only using head
        else if (turretHead.transform.localEulerAngles.y < maximumVerticalAngle || turretHead.transform.localEulerAngles.y > 360 - maximumVerticalAngle)
        {
            //Reset if no transformation can/should be made
            UpdateLocalEulerAngles(previousBodyRotationEuler, previousHeadRotationEuler);
            return true;
        }
        //Reset if no transformation can/should be made
        UpdateLocalEulerAngles(previousBodyRotationEuler, previousHeadRotationEuler);

        return false;
    }

    //Updates the local euler angles of the turret body and head
    public void UpdateLocalEulerAngles(Vector3 targetBodyRotation, Vector3 targetHeadRotation)
    {
        turretBody.transform.localEulerAngles = targetBodyRotation;
        turretHead.transform.localEulerAngles = targetHeadRotation;
    }
    public Vector3 GetBodyEulerAngles()
    {
        return turretBody.transform.localEulerAngles;
    }
    public Vector3 GetHeadEulerAngles()
    {
        return turretHead.transform.localEulerAngles;
    }
}
