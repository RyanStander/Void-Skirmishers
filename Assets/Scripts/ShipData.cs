using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipData : MonoBehaviour
{
    public string userID;
    public bool isPlayer;
    public GameObject prefabType;
    public bool isRespawnable;
    public Factions faction;

    public void UpdateShipData(string userID, bool isPlayer, GameObject prefabType, bool isRespawnable, Factions faction)
    {
        this.userID = userID;
        this.isPlayer = isPlayer;
        this.prefabType = prefabType;
        this.isRespawnable = isRespawnable;
        this.faction = faction;
    }
}
