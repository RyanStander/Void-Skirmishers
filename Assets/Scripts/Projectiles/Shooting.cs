using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Shooting : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 10;

    private Rigidbody rb;

    [SerializeField] float lifeSpan = 10;

    private string userIDOfShooter;
    private string tagOfShooter;
    void Start()
    {
        lifeSpan += Time.time;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //rb.AddForce(transform.forward * bulletSpeed* Time.deltaTime);
        rb.transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
        if (Time.time>lifeSpan)
        {
            Destroy(transform.root.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        var query = Enum.GetValues(typeof(Factions)).Cast<Factions>();
        foreach (Factions faction in query)
        {
            //If faction is currently present
            if (other.tag==faction.ToString())
            {              
                Health healthOfTargetHit = other.transform.root.GetComponent<Health>();
                if (healthOfTargetHit != null)
                    healthOfTargetHit.TakeDamage(tagOfShooter, userIDOfShooter);
            }
        }
        Explosions temp = other.transform.root.GetComponent<Explosions>();
        if (temp != null)
            temp.CreateBulletExplosion(transform.position);
        Destroy(transform.root.gameObject);
    }
    public float GetLifeSpan()
    {
        return lifeSpan;
    }

    public void SetShooterValues(string userID,string creatorTag)
    {
        userIDOfShooter = userID;
        tagOfShooter = creatorTag;
    }
}
