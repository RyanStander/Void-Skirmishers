using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    private LineRenderer lineRenderer = null;
    public int maximumRange = 25;
    public float duration = 2;
    public int projectileDamage = 5;
    private List<GameObject> damagedObjects = new List<GameObject>();

    private string teamTag=null, userID = "PlaceholderID";
    //Factions.FactionUndefined.ToString()

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        RaycastingPositions();
        Destroy(gameObject, duration);
    }
    void LateUpdate()
    {
        RaycastingPositions();
    }
    private void RaycastingPositions()
    {
        //Sets position of the laser at the current transform
        lineRenderer.SetPosition(0, transform.position);

        //Raycasy to see what is hittable
        RaycastHit hitObject;
        if (Physics.Raycast(transform.position, transform.forward, out hitObject))
        {
            //If hitting a valid collider
            if (hitObject.collider)
            {
                lineRenderer.SetPosition(1, hitObject.point);

                //Deals damage if not already in the hit objects list
                if (!damagedObjects.Contains(hitObject.collider.gameObject))
                {
                    //Gets the health script if applicable
                    if (hitObject.collider.gameObject.GetComponent<Health>() != null)
                    {
                        if (teamTag != null)
                        {
                            //Deals damage
                            hitObject.collider.gameObject.GetComponent<Health>().TakeDamage(teamTag, userID, projectileDamage);
                        }
                        else
                        {
                            throw new System.Exception("Team tag was null");
                        }
                    }
                }

                //Add to already damaged objects (if applicable)
                damagedObjects.Add(hitObject.collider.gameObject);
            }
            else
            {
                //Draw to maximum range
                lineRenderer.SetPosition(1, transform.position + (transform.forward * maximumRange));
            }
        }
        else
        {
            //Draw to maximum range
            lineRenderer.SetPosition(1, transform.position + (transform.forward * maximumRange));
        }
    }
    public void SetShooterValues(string userID, string creatorTag)
    {
        this.userID = userID;
        teamTag = creatorTag;
    }
}