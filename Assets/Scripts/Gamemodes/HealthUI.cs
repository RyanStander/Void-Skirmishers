using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    //Bar being transformed
    [SerializeField] protected private RectTransform barRectTransform;

    //Health script being followed
    [SerializeField] protected private GameObject statsObject=null;

    //Maximum height of item being modified
    protected private float maximumHeight=0;

    protected void Awake()
    {
        maximumHeight = barRectTransform.rect.height;
    }
    protected void OnEnable()
    {
        EventManager.Subscribe(EventType.PLAYERSPAWNEVENT, UpdateFollowedStats);
    }
    protected void OnDisable()
    {
        EventManager.Unsubscribe(EventType.PLAYERSPAWNEVENT, UpdateFollowedStats);
    }
    protected private void UpdateFollowedStats(EventData eventData)
    {
        //Cast and error handling to make sure that the correct type of EventData is being recieved
        if (eventData is PlayerSpawnEventData userEventData)
        {
            statsObject = userEventData.statsObject;
            //If no, then do nothing
        }
        else
        {
            //Unity Player
            Debug.Log("Warning: Given EventData is not the same as the permitted PlayerSpawnEventData");
        }
    }
    private void FixedUpdate()
    {
        if (statsObject)
        {
            UpdateBarDisplay(statsObject.GetComponent<Health>().GetHealthPercentage());
        }
        else
        {
            //If no object, then 0 health
            UpdateBarDisplay(0);
        }
    }
    protected private void UpdateBarDisplay(float percentage)
    {
        //Updates the bar
        barRectTransform.sizeDelta = new Vector2(barRectTransform.sizeDelta.x, maximumHeight * percentage);
    }
}
