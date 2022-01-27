using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFireTurrets : MonoBehaviour
{
    [SerializeField] private TurretContainer turretContainer = null;

    [Header("Gun Data")]
    [Range(0.1f,3)]
    [SerializeField] private float sideFireRate = 2;
    [Range(0.5f,10)]
    [SerializeField] private float frontFireRate = 5;
    private float nextLeftFire;
    private float nextRightFire;
    private float nextFrontFire;
    [SerializeField] private GameObject SideTurretsBullet=null;
    [SerializeField] private GameObject FrontTurretsBullet=null;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSourceComponent = null;
    [SerializeField] private AudioClip turretFireSound = null;
    [SerializeField] private AudioClip laserFireSound = null;

    private string factionTag=null;
    private string userID = null;

    private void Start()
    {
        factionTag = gameObject.tag.ToString();
        userID = gameObject.GetComponent<ShipData>().userID;
    }
    private void Update()
    {
        Shooting();
    }

    private void Shooting()
    {
        //On left mouse button, fire bullet
        if (Input.GetButton("Fire1"))
        {
            TurretOrientation orientation = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerRotatingCamera>().orientation;
            switch (orientation)
            {
                case TurretOrientation.FRONT:
                    FireFrontTurrets(turretContainer.GetFrontTurrets());
                    break;
                case TurretOrientation.LEFT:
                    FireLeftTurrets(turretContainer.GetLeftTurrets());
                    break;
                case TurretOrientation.RIGHT:
                    FireRightTurrets(turretContainer.GetRightTurrets());
                    break;
            }
            //rate of fire
            
        }
    }
    //instantiation of bullet
    private void createBullet(List<TurretData> turrets, GameObject bulletType,AudioClip sound)
    {
        if (audioSourceComponent != null)
        {
            if (turretFireSound != null)
            {
                audioSourceComponent.PlayOneShot(sound);
            }
            else
            {
                throw new System.NullReferenceException("Null Reference Exception: There is no AudioClip assigned to PlayerFireTurret Component");
            }
        }
        else
        {
            throw new System.NullReferenceException("Null Reference Exception: There is no AudioSource assigned to PlayerFireTurrets Component");
        }
        //Gets the orientation of the camera and then fires the respective turret
        foreach (TurretData turret in turrets)
        {
            GameObject bulletClone = Instantiate(bulletType, turret.turretFirePoint.position, turret.turretFirePoint.rotation);

            //If projectile is normal
            if (bulletClone.GetComponent<Shooting>() != null)
            {
                bulletClone.GetComponent<Shooting>().SetShooterValues(userID, factionTag);
            }

            //If projectile is laser beam
            if (bulletClone.GetComponent<LaserBeam>() != null)
            {
                bulletClone.GetComponent<LaserBeam>().SetShooterValues(userID, factionTag);
            }
        }
    }


    //following 3 functions run through respective turrets and creates a bullet from their firepoint
    private void FireLeftTurrets(List<TurretData> turrets)
    {
        if (Time.time > nextLeftFire)
        {
            nextLeftFire = Time.time + sideFireRate;
            createBullet(turrets, SideTurretsBullet,turretFireSound);
        }
    }
    private void FireRightTurrets(List<TurretData> turrets)
    {
        if (Time.time > nextRightFire)
        {   
            nextRightFire = Time.time + sideFireRate;
            createBullet(turrets, SideTurretsBullet, turretFireSound);
        }
    }
    private void FireFrontTurrets(List<TurretData> turrets)
    {
        if (Time.time > nextFrontFire)
        {
            nextFrontFire = Time.time + frontFireRate;
            createBullet(turrets, FrontTurretsBullet,laserFireSound);
        }
    }
}
