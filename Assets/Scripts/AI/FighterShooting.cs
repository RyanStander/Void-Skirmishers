using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[RequireComponent(typeof(ShipData))]
public class FighterShooting : MonoBehaviour
{
    private Transform target;
    /// <summary>
    /// the locations where bullets are fired from
    /// </summary>
    [SerializeField] Transform[] projectileSpawnPoints=null;
    /// <summary>
    /// The maximum distance that a target can be from the ship for firing
    /// </summary>
    [SerializeField] float maxTagetingDistance = 30;
    /// <summary>
    /// The projectile that is fired
    /// </summary>
    [SerializeField] GameObject projectileObject=null;
    /// <summary>
    /// The movement script that is used by the ship
    /// </summary>
    [SerializeField] FighterMovement fighterMovement=null;
    private float NextFire;//cooldown between fires
    /// <summary>
    /// fire rate of how fast projectiles are shot
    /// </summary>
    [SerializeField] float FireRate = 10;
    private AudioSource[] audioSources;
    [SerializeField] ShipData shipData = null;

    [SerializeField] private Factions alliedFaction = Factions.FactionUndefined;
    void Start()
    {
        if (shipData==null)
        {
            shipData = gameObject.GetComponent<ShipData>();
        }
        audioSources = transform.root.GetComponents<AudioSource>();
    }
    void LateUpdate()
    {
        FireAtTarget();
    }

    private void FireAtTarget()
    {
        //takes current target and shoots at them if they are within certain constraints (refer to InFront())
        target = fighterMovement.GetTarget();
        //checks if the target is in sight and then fires bullets
        if (InFront() && HaveLineOfSight())
            if (Time.time > NextFire)
            {
                //audioSources[0].Play();
                NextFire = Time.time + FireRate;

                foreach (Transform spawnPoint in projectileSpawnPoints)
                {
                    GameObject bulletClone = Instantiate(projectileObject, spawnPoint.position, spawnPoint.rotation);
                    if (shipData!=null)
                    {
                        if (bulletClone.GetComponent<LaserBeam>() != null)
                        {
                            bulletClone.GetComponent<LaserBeam>().SetShooterValues(shipData.userID, gameObject.tag);
                        }
                        else if (bulletClone.GetComponent<Shooting>() != null)
                        {
                            bulletClone.GetComponent<Shooting>().SetShooterValues(shipData.userID, shipData.faction.ToString());
                        }
                    }
                    else
                    {
                        throw new Exception("Could not find ship data");
                    }
                    
                }
            }
    }

    public bool InFront()
    {
        if (target == null)
        {
            return false;
        }
        else
        {
            Vector3 directionToTarget = transform.position - target.position;
            float angle = Vector3.Angle(transform.forward, directionToTarget);

            //Check if it is in range
            if (Mathf.Abs(angle) > 90 && Mathf.Abs(angle) < 270)
            {
                return true;
            }
            return false;
        }
    }

    public bool HaveLineOfSight()
    {
        RaycastHit hit;

        //draws a raycast from each point and looks if one of them can hit the target
        //will return false if none have line of sight
        foreach (Transform spawnPoint in projectileSpawnPoints)
        {
            Vector3 direction = target.position - spawnPoint.position;

            if (Physics.Raycast(spawnPoint.position, direction, out hit, maxTagetingDistance))
            {
                Debug.DrawRay(spawnPoint.position, direction, Color.green);
                var query = Enum.GetValues(typeof(Factions)).Cast<Factions>().Except(new Factions[] { alliedFaction });
                //resets to closes target to first in the array
                foreach (Factions faction in query)
                {
                    if (hit.collider.gameObject.tag == faction.ToString())
                    {
                        return true;
                    }
                }

            }


        }
        return false;
    }
}
