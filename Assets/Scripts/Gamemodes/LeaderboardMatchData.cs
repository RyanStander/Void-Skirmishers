using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System.Linq;
using System;

public class LeaderboardMatchData : MonoBehaviour
{
    //The board where the leaderboard information is contained
    [SerializeField] GameObject toggledBoard = null;

    //Container that the data item will be created in
    [SerializeField] Transform targetContainer = null;

    //UserData display prefab
    [SerializeField] GameObject userDataItem = null;

    //List of available user match data
    private List<GameObject> userDataObjectList = new List<GameObject>();

    //Exemptions to the leaderboard when considering kills
    [SerializeField] private List<string> exemptedIdentifiers = new List<string>() {"bot"};

    public Dictionary<string, FactionMatchData> factionStats = new Dictionary<string, FactionMatchData>();

    private void OnEnable()
    {
        EventManager.Subscribe(EventType.STARTKILLEVENT, OnIncomingKillData);
        EventManager.Subscribe(EventType.NEWUSEREVENT, OnIncomingUser);
    }
    private void OnDisable()
    {
        EventManager.Unsubscribe(EventType.STARTKILLEVENT, OnIncomingKillData);
        EventManager.Unsubscribe(EventType.NEWUSEREVENT, OnIncomingUser);
    }

    private void OnIncomingKillData(EventData eventData)
    {
        //Cast and error handling to make sure that the correct type of EventData is being recieved
        if (eventData is StartKillEventData killEventData)
        {
            //Killer object
            //First determine if items already exist
            bool pairFound = false;
            //Filters out exempted userIDs for killer
            if (!exemptedIdentifiers.Contains(killEventData.killerUserID))
            {
                foreach (GameObject dataObject in userDataObjectList)
                {
                    //Temporary reference
                    UserMatchData userMatchData = dataObject.GetComponent<UpdateUserMatchData>().userData;

                    //If the tags matches, update the container
                    if (userMatchData.userID == killEventData.killerUserID)
                    {
                        //Determin if it was a team kill
                        if (killEventData.killedTeamTag == killEventData.killerTeamTag)
                        {
                            userMatchData.kills -= 1;
                        }
                        else
                        {
                            userMatchData.kills += 1;
                        }

                        dataObject.GetComponent<UpdateUserMatchData>().UpdateUserData(userMatchData);

                        //Inform that a pair was found
                        pairFound = true;
                    }
                }
                //If already existing, update otherwise
                if (!pairFound)
                {
                    int kills;
                    //Determin if it was a team kill
                    if (killEventData.killedTeamTag == killEventData.killerTeamTag)
                    {
                        kills = -1;
                    }
                    else
                    {
                        kills = 1;
                    }

                    //Add new item
                    AddDataItem(new UserMatchData(killEventData.killerUserID, killEventData.killerTeamTag, kills, 0));
                }
            }

            //Killed object
            //First determine if items already exist
            pairFound = false;
            //Filters out exempted userIDs for killed
            if (!exemptedIdentifiers.Contains(killEventData.killedUserID))
            {
                foreach (GameObject dataObject in userDataObjectList)
                {
                    //Temporary reference
                    UserMatchData userMatchData = dataObject.GetComponent<UpdateUserMatchData>().userData;

                    //If the tags matches, update the container
                    if (userMatchData.userID == killEventData.killedUserID)
                    {
                        //Add 1 death
                        userMatchData.deaths += 1;

                        //Inform that a pair was found
                        pairFound = true;

                        dataObject.GetComponent<UpdateUserMatchData>().UpdateUserData(userMatchData);
                    }
                }
                if (!pairFound)
                {
                    //Add new item
                    AddDataItem(new UserMatchData(killEventData.killerUserID, killEventData.killerTeamTag, 0, 1));
                }
            }
        }
        else
        {
            //Unity Player
            Debug.Log("Warning: Given EventData is not the same as the permitted StartKillEventData");
        }
    }
    private void OnIncomingUser(EventData eventData)
    {
        //Cast and error handling to make sure that the correct type of EventData is being recieved
        if (eventData is NewUserEventData userEventData)
        {
            AddDataItem(userEventData.userData);
            //If no, then do nothing
        }
        else
        {
            //Unity Player
            Debug.Log("Warning: Given EventData is not the same as the permitted NewUserEventData");
        }
    }
    public void AddDataItem(UserMatchData givenData)
    {
        //ResizeParent();
        GameObject newDataItem = Instantiate(userDataItem, targetContainer);
        newDataItem.transform.localScale = new Vector3(1, 1, 1);
        newDataItem.GetComponent<UpdateUserMatchData>().UpdateUserData(givenData);

        //Adds item to dictionary
        userDataObjectList.Add(newDataItem);
    }

    private void Update()
    {
        ToggleLeaderboard();
    }

    //Update leaderboard based on keys
    private void ToggleLeaderboard()
    {
        //If pressing enable
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            toggledBoard.SetActive(true);
        }
        //If lifting disable
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            toggledBoard.SetActive(false);
        }
    }
    private void LateUpdate()
    {
        SortByKills();
    }

    private void SortByKills()
    {
        //If list has items to organize
        if (userDataObjectList.Count != 0)
        {
            //Sort the data objects
            userDataObjectList = userDataObjectList.OrderBy(x => x.GetComponent<UpdateUserMatchData>().userData.kills).ToList();

            //Then update the location of the children
            foreach (GameObject child in userDataObjectList)
            {
                //Pushed each child to the start of the list, updating their position in the list
                child.transform.SetAsFirstSibling();
            }
        }
    }

    public void UpdateTeamCounts()
    {
        //Built list of participating factions
        Dictionary<string, FactionMatchData> participatingFactions = new Dictionary<string, FactionMatchData>();

        //Populates the list
        foreach (GameObject dataObject in userDataObjectList)
        {
            //Temporary
            FactionMatchData userMatchData = new FactionMatchData(dataObject.GetComponent<UpdateUserMatchData>().userData.faction,dataObject.GetComponent<UpdateUserMatchData>().userData.kills, dataObject.GetComponent<UpdateUserMatchData>().userData.deaths);

            //If not already existing
            if (!participatingFactions.ContainsKey(userMatchData.faction))
            {
                //Create key reference
                participatingFactions.Add(userMatchData.faction, new FactionMatchData(userMatchData.faction, 0, 0));
            }
            //Get the correct faction
            participatingFactions.TryGetValue(userMatchData.faction, out FactionMatchData dictionaryData);
            
            //Update dictionary data
            dictionaryData.kills += userMatchData.kills;
            dictionaryData.deaths += userMatchData.deaths;

            //Replace data in the dictionary
            participatingFactions[userMatchData.faction] = dictionaryData;
        }

        //Update the list
        factionStats = participatingFactions;
    }

    public FactionMatchData GetLeadingFaction()
    {
        //Updates the list
        UpdateTeamCounts();

        //Temporary leading faction
        FactionMatchData leadingFaction = factionStats.First().Value;

        //Find the leading faction by kills
        foreach (KeyValuePair<string, FactionMatchData> dictionary in factionStats)
        {
            //Checks if greater than previous leader
            if(dictionary.Value.kills > leadingFaction.kills)
            {
                leadingFaction = dictionary.Value;
            }
        }

        return leadingFaction;
    }
}
