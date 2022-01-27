using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnShipEventData : EventData
{
    public ShipData shipData;
    public RespawnShipEventData(ShipData shipData) : base(EventType.RESPAWNSHIPEVENT) 
    {
        this.shipData = shipData;
    }
}
public class MatchOverEventData : EventData
{
    public MatchOverEventData() : base(EventType.MATCHOVEREVENT){}
}
public class PlayerSpawnEventData : EventData
{
    public Transform followPoint;
    public GameObject statsObject;
    public PlayerSpawnEventData(Transform followPoint, GameObject statsObject) : base(EventType.PLAYERSPAWNEVENT) 
    {
        this.followPoint = followPoint;
        this.statsObject = statsObject;
    }
}

public class NewUserEventData : EventData
{
    public UserMatchData userData;
    public NewUserEventData(UserMatchData userData) : base(EventType.NEWUSEREVENT)
    {
        this.userData = userData;
    }
}

public class StartKillEventData : EventData
{
    //Killing user
    public string killerUserID;
    public string killerTeamTag;

    //Killed user
    public string killedUserID;
    public string killedTeamTag;
    public StartKillEventData(string killerUserID, string killerTeamTag, string killedUserID, string killedTeamTag) : base(EventType.STARTKILLEVENT)
    {
        //Killer
        this.killerUserID = killerUserID;
        this.killerTeamTag = killerTeamTag;

        //Killed
        this.killedUserID = killedUserID;
        this.killedTeamTag = killedTeamTag;
    }
}