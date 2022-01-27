using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchSetupManager : MonoBehaviour
{
    [Header("Match Values")]
    [Range(10, 600)]
    [SerializeField] private float matchDuration = 300;
    [SerializeField] private Text timerText=null;

    [Range(1,10)]
    [SerializeField] private int playersPerTeam=2;
    [SerializeField] private List<FactionRespawnManager> factionManagers = new List<FactionRespawnManager>();//Limits at 4

    [Header("Faction Prefabs")]
    [SerializeField] private List<GameObject> PlayerCapitalShips = new List<GameObject>();//Minimum 1
    [SerializeField] private List<GameObject> AICapitalShips = new List<GameObject>(); //Minimum the amount of teams
    
    //Potential names for players
    private readonly List<string> userIDs= new List<string>(){ "Granny4theWIN", "SeekNDstroy","Xxx_Sl4y3r_xxX","GlitterGunner","SuperGurl3000",
        "CHUCKNORRIS1493","Youknowwhoiam","FluffySnarf","barackyomama","KFCServer","FantasticNinja","CobwebRobber","BillNyeehTheSkeletor","TheLegend27","Cube74","N00BMASTER69"};
    private List<string> takenIDs = new List<string>();
    private bool isPlayer = true;

    private List<ShipData> shipDatas = new List<ShipData>();

    private void Start()
    {
        InitializeMatch();
        ShipDataToRespawnManagers();
    }
    private void InitializeMatch()
    {
        //Compares number of teams against number of capital ships given
        //Exception error if not enough ships are specified
        if (factionManagers.Count <= AICapitalShips.Count && 1 <= PlayerCapitalShips.Count)
        {
            //For each team
            for (int i = 0; i < factionManagers.Count; i++)
            {
                //Selects the faction
                Factions capitalFaction = (Factions)i;

                //For each player in team
                for (int b = 0; b < playersPerTeam; b++)
                {
                    //Selects the userID
                    int randomIndex = Random.Range(1, userIDs.Count);
                    while (takenIDs.IndexOf(userIDs[randomIndex]) != -1)
                    {
                        randomIndex = Random.Range(1, userIDs.Count);
                    }
                    string capitalUserID = userIDs[randomIndex];
                    takenIDs.Add(capitalUserID);

                    //Creates temporary game object to make a ShipData component
                    GameObject temporaryObject = new GameObject();
                    ShipData shipData = temporaryObject.AddComponent<ShipData>();

                    if (isPlayer)
                    {
                        shipData.UpdateShipData(capitalUserID, isPlayer, PlayerCapitalShips[i], true, capitalFaction);
                        
                        //makes it so there is only one player
                        isPlayer = false;
                    }
                    else
                    {
                        shipData.UpdateShipData(capitalUserID, isPlayer, AICapitalShips[i], true, capitalFaction);
                    }

                    //Adds data to list
                    shipDatas.Add(shipData);

                    //Fires off data of new user
                    EventManager.AddEvent(new NewUserEventData(new UserMatchData(shipData.userID,shipData.faction.ToString(),0,0)));

                    //Destroys the temporary object
                    Destroy(temporaryObject);
                }
            }
        }
        else if (!(factionManagers.Count <= AICapitalShips.Count))
        {
            throw new System.Exception("There are not enough prefabs for the amount of teams,\nadd more prefabs to " + gameObject.name + " if you want larger amount of teams");
        }
        else
        {
            throw new System.Exception("You did not assign a prefab for player,\nadd the prefab to " + gameObject.name);
        }
    }
    private void ShipDataToRespawnManagers()
    {
        //Gives the ship data to the respawn managers based on the selected factions
        foreach (ShipData data in shipDatas)
        {
            //Filters based on which faction manager matches
            foreach (FactionRespawnManager manager in factionManagers)
            {
                //If matching faction
                if (manager.CurrentFaction() == data.faction)
                {
                    //Enqueues the data
                    manager.EnqueueShipData(data);
                }
            }
        }
    }

    private void Update()
    {
        MatchTimer();
    }

    private bool isMatchOver = false;
    private void MatchTimer()
    {
        //Checks if match duration is over and if so, fire off an event
        if (matchDuration<=0 && !isMatchOver)
        {
            //Fires off event that the match is over
            EventManager.AddEvent(new MatchOverEventData());
            isMatchOver = true;
        }
        else
        {
            //Lower time
            matchDuration -= Time.deltaTime;

            //Update timer
            if(timerText!=null && matchDuration > 0)
            {
                timerText.text = ((int)matchDuration).ToString();
            }
        }
    }
}
