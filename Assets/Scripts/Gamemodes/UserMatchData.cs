using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UserMatchData
{
    public string userID;
    public string faction;
    public int kills;
    public int deaths;

    public UserMatchData(string userID, string faction, int kills, int deaths)
    {
        this.userID=userID;
        this.faction = faction;
        this.kills = kills;
        this.deaths = deaths;
    }
}
