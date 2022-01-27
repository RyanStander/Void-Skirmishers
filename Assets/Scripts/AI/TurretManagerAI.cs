using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManagerAI : MonoBehaviour
{
    [Range(0.1f, 5)]
    public float FireRate = 0.5f;
    private float nextFiringTime;

    private List<GameObject> turrets = new List<GameObject>();
    [SerializeField] private GameObject leadingTurret=null;

    private string parentFaction=null;
    private string parentID = null;

    private void Start()
    {
        //Gets the parent faction and ID
        parentFaction = transform.parent.tag.ToString();
        parentID = transform.root.gameObject.GetComponent<ShipData>().userID;

        //Gets turrets
        var firingPoints = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform turret in firingPoints)
        {
            //Adds all turrets
            if (turret.tag is "Turret")
            {
                turrets.Add(turret.gameObject);

                //If the leading turret
                if(turret.GetComponent<TurretShootingAI>().isLeadingTurret)
                {
                    leadingTurret = turret.gameObject;
                }

                //Update targets allied faction
                if (System.Enum.TryParse(parentFaction, out Factions parsedFaction))
                {
                    turret.GetComponent<TurretShootingAI>().alliedFaction = parsedFaction;
                }
                else
                {
                    turret.GetComponent<TurretShootingAI>().alliedFaction = Factions.FactionUndefined;
                }
            }
        }

        //If no leading turret was found, use a random one
        if(leadingTurret==null)
        {
            leadingTurret = turrets[Random.Range(0,turrets.Count)];
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AimFollowingTurrets();
        FiringTimer();
    }
    private void FiringTimer()
    {
        //if the turret can see a target without obstruction it will start shooting
        if (leadingTurret.GetComponent<TurretShootingAI>().targetSpotted && Time.time > nextFiringTime)
        {
            //Fire from all points
            foreach(GameObject turret in turrets)
            {
                turret.GetComponent<TurretShootingAI>().Fire(parentID, parentFaction);
            }
            nextFiringTime = Time.time + FireRate;
        }
    }
    private void AimFollowingTurrets()
    {
        TurretShootingAI turretAI = leadingTurret.GetComponent<TurretShootingAI>();
        //Updates rotation of each turret based on leading turret
        foreach (GameObject turret in turrets)
        {            
            turret.GetComponent<TurretShootingAI>().UpdateLocalEulerAngles(turretAI.GetBodyEulerAngles(), turretAI.GetHeadEulerAngles());
        }
    }
}
