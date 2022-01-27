using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameoverManager : MonoBehaviour
{
    //The board where the leaderboard information is contained
    [SerializeField] private LeaderboardMatchData leaderboardMatchData = null;
    [SerializeField] private GameObject gameOverPanel = null;
    [SerializeField] private Text winningFaction = null, killText = null, deathText = null;

    private void OnEnable()
    {
        EventManager.Subscribe(EventType.MATCHOVEREVENT, MatchOverUpdate);
    }
    private void OnDisable()
    {
        EventManager.Unsubscribe(EventType.MATCHOVEREVENT, MatchOverUpdate);
    }

    private void Start()
    {
        if(leaderboardMatchData==null)
        {
            leaderboardMatchData=this.gameObject.GetComponent<LeaderboardMatchData>();
        }
    }

    private void MatchOverUpdate(EventData eventData)
    {
        //Updates relevant text and displays end screen
        gameOverPanel.SetActive(true);

        //Temporary data fetch
        FactionMatchData temporaryData = leaderboardMatchData.GetLeadingFaction();

        //Update winner
        winningFaction.text = "Faction "+temporaryData.faction +" Wins!";

        //Update kills
        killText.text = "Total Kills: " + temporaryData.kills;

        //Update deaths
        deathText.text = "Total Deaths: " + temporaryData.deaths;
    }
}
