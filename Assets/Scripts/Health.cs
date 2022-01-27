using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Explosions))]
public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int maxShield = 10;

    [SerializeField] private float shieldRegenRate = 0.25f; //Rate at which shields regenerate

    [Range(0,5)]
    [SerializeField] private float shieldRegenStaggerTime = 0.5f; //When hit, shield takes time to begin regenerating
    private float staggerTimeRemaining=0;
    private bool staggeringRegen = false;

    //Current parameters, set in start
    private int curHealth;
    private float curShield;

    //Prevents ship from having its kill condition happen multiple times
    private bool isDead = false;

    private void Start()
    {
        //Sets the ships health to its defined maximum on creation
        curHealth = maxHealth;
        curShield = maxShield;
    }

    private void Update()
    {
        RegenerateShields();
    }

    private void RegenerateShields()
    {
        //If staggering, wait until stagger time is over
        if (staggeringRegen)
        {
            //Else reset values and set staggeringRegen to false
            if (staggerTimeRemaining<0)
            {
                staggerTimeRemaining = shieldRegenStaggerTime;
                staggeringRegen = false;
            }
            //If time remains, continue to lower time
            else
            {
                //Lower time
                staggerTimeRemaining -= Time.deltaTime;
            }
        }
        //If shield is not staggered, regen as expected
        else
        {
            //If below max
            if (curShield < maxShield) curShield += shieldRegenRate;

            //If at max
            if (curShield > maxShield) curShield = maxShield;
        }
    }

    //Whenever the ship is hit with a bullet or another ship, they take damage,
    //this looks for whether it is a player bullet and the damage taken.
    public void TakeDamage(string tagName = "", string userID = "", int damageAmount = 1)
    {
        //Set stagger to true
        staggeringRegen = true;
        staggerTimeRemaining = shieldRegenStaggerTime;

        //Shield takes damage
        curShield -= damageAmount;

        //If shields go negative, carry over remaining damage, else set damage to 0
        if (curShield<0)
        {
            //Set damage and reset shield count
            damageAmount = (int)Mathf.Abs(curShield);
            curShield = 0;
        }
        else
        {
            damageAmount = 0;
        }

        //ship taking damage
        curHealth -= damageAmount;

        //if the current health of a ship is 0 or less it is destroyed
        if (curHealth <= 0 && !isDead)
        {
            isDead = true;
            ShipData victimShipData = this.gameObject.GetComponent<ShipData>();

            //Sends out event that a kill has been made
            if (userID != "" && tagName != "" && victimShipData != null)
            {
                EventManager.AddEvent(new StartKillEventData(userID, tagName, victimShipData.userID, victimShipData.faction.ToString()));
            }
            else
            {
                if (userID != "")
                {
                    throw new System.NullReferenceException("userID was not assigned");
                }
                else if (tagName != "")
                {
                    throw new System.NullReferenceException("tagName was not assigned");
                }
                else if (victimShipData != null)
                {
                    throw new System.NullReferenceException("shipData of target was not found");
                }
                else if (victimShipData.userID != "")
                {
                    throw new System.NullReferenceException("shipData userID of target was not found");
                }
                else if (victimShipData.faction.ToString() != "")
                {
                    throw new System.NullReferenceException("shipData faction of target was not found");
                }
            }
            //Request to respawn this ship
            EventManager.AddEvent(new RespawnShipEventData(this.gameObject.GetComponent<ShipData>()));

            //The ship is destroyed so it will create an explosion
            Explosions temporaryExplosion = gameObject.GetComponent<Explosions>();
            temporaryExplosion.CreateShipExplosion();
        }
    }
    public float GetHealthPercentage()
    {
        return curHealth / (float)maxHealth;
    }
    public float GetShieldPercentage()
    {
        return curShield / maxShield;
    }
}
