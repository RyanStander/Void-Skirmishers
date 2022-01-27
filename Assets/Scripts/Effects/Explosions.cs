using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Explosions : MonoBehaviour
{
    /// <summary>
    /// Duration of the animation for bullet explosion
    /// </summary>
    [SerializeField] float animationDurationBullet=2f;
    /// <summary>
    /// bullet explosion object
    /// </summary>
    [SerializeField] GameObject bulletExplosion=null;

    /// <summary>
    /// Duration of the animation for ship explosion
    /// </summary>
    [SerializeField] float animationDurationExplosion=4f;
    /// <summary>
    /// Ship explosion object
    /// </summary>
    [SerializeField] GameObject shipExplosion=null;
    
    //Creates an explosion for bullets hiting objects
    public void CreateBulletExplosion(Vector3 Pos)
    {
        //instantiates the object
        GameObject boom = Instantiate(bulletExplosion, Pos, Quaternion.identity, transform) as GameObject;
        //destroys the explosion after a set time
        Destroy(boom, animationDurationBullet);
    }
    public void CreateShipExplosion()
    {
        //Debug.Log("Big stupid crashed!");
        GameObject boom = Instantiate(shipExplosion, transform.position, Quaternion.identity) as GameObject;
        //destroys the explosion after a set time
        Destroy(boom, animationDurationExplosion);
        //destroys the object that created the explosion
        Destroy(gameObject);
    }
}
