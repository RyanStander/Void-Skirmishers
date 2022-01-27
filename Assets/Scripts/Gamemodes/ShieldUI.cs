using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Uses same functionality as HealthUI except for the fixedUpdate
public class ShieldUI : HealthUI
{
    private void FixedUpdate()
    {
        if (statsObject)
        {
            //Update bar based on the percentage of the shield
            UpdateBarDisplay(statsObject.GetComponent<Health>().GetShieldPercentage());
        }
        else
        {
            //If no object, then 0 shields
            UpdateBarDisplay(0);
        }
    }
}
