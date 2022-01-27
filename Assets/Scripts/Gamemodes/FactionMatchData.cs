using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FactionMatchData
{
    public string faction;
    public int kills;
    public int deaths;

    public FactionMatchData(string faction, int kills, int deaths)
    {
        this.faction = faction;
        this.kills = kills;
        this.deaths = deaths;
    }
}
