using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactionKillsUI : MonoBehaviour
{
    //UI elements to be updated
    [SerializeField] private List<GameObject> FactionKillCounters = new List<GameObject>();
    //Maximum teams
    private int maximumTeams = 4;

    //The board where the leaderboard information is contained
    [SerializeField] private LeaderboardMatchData leaderboardMatchData = null;
    private void Start()
    {
        if (leaderboardMatchData == null)
        {
            leaderboardMatchData = this.gameObject.GetComponent<LeaderboardMatchData>();
        }

        //Sets counters activity to false until enabled/updated
        foreach(GameObject obj in FactionKillCounters)
        {
            obj.SetActive(false);
        }
    }

    // Update is called once per frame
    public void LateUpdate()
    {
        //Temporary data for gathering kill counts from
        leaderboardMatchData.UpdateTeamCounts();
        Dictionary<string, FactionMatchData> temporaryData = leaderboardMatchData.factionStats;

        //If more teams are used than supported on the counters
        if (temporaryData.Count> maximumTeams)
        {
            return;
        }

        int temporaryCounter = 0;
        //Update the relevant panels based on available stats
        foreach (KeyValuePair<string, FactionMatchData> dictionary in temporaryData)
        {
            //Set counter to active
            FactionKillCounters[temporaryCounter].SetActive(true);

            //Update the information
            FactionKillCounters[temporaryCounter].GetComponentInChildren<Text>().text = dictionary.Value.kills.ToString();

            //Add to temporary counter
            temporaryCounter++;
        }
    }
}
