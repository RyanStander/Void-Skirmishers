using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public bool isAvailable = false;
    public void SpawnShip(ShipData data)
    {
        //Instantiates a ship
        GameObject spawnedShip = Instantiate(data.prefabType, transform.position, transform.rotation);

        //Sets the correct starting values
        //Update the ships data
        ShipData spawnedShipData = spawnedShip.GetComponent<ShipData>();
        if (spawnedShipData == null)
        {
            //If no component is attached
            spawnedShipData = spawnedShip.AddComponent<ShipData>();
            spawnedShipData.UpdateShipData(data.userID, data.isPlayer, data.prefabType, data.isRespawnable,data.faction);
        }
        else
        {
            //Update current component
            spawnedShip.GetComponent<ShipData>().UpdateShipData(data.userID,data.isPlayer,data.prefabType,data.isRespawnable, data.faction);
        }
        //Update the tag
        spawnedShip.tag = data.faction.ToString();
    }
    private void OnTriggerStay(Collider other)
    {
        isAvailable = false;
    }
    private void OnTriggerExit(Collider other)
    {
        isAvailable = true;
    }
}