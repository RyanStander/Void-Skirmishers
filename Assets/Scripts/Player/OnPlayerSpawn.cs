using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerSpawn : MonoBehaviour
{
    [SerializeField] private Transform followPoint=null;
    private void Start()
    {
        if(followPoint is null)
        {
            Debug.Log("Follow point not set!");
            return;
        }

        EventManager.AddEvent(new PlayerSpawnEventData(followPoint, this.gameObject));
    }
}
