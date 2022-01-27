using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUserMatchData : MonoBehaviour
{
    public UserMatchData userData;
    [SerializeField] private Text userNameText;
    [SerializeField] private Text factionNameText;
    [SerializeField] private Text killsText;
    [SerializeField] private Text deathsText;

    public void UpdateUserData(UserMatchData currentData)
    {
        //Updates the current available data
        userData = currentData;

        //Updates the text
        UpdateText();
    }
    public void UpdateText()
    {
        //Updates the text
        userNameText.text = userData.userID;
        factionNameText.text = userData.faction;
        killsText.text = userData.kills.ToString();
        deathsText.text = userData.deaths.ToString();
    }
}
