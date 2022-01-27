using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterSpawnManager : MonoBehaviour
{
    private List<GameObject> spawnPoints = new List<GameObject>();
    [SerializeReference] private int spawnDelayPerWave = 1;
    [SerializeReference] private int spawnAmountPerWave = 1;

    // Start is called before the first frame update
    void Start()
    {
        UpdateRespawnPoints();
        InvokeRepeating("HandleSpawn", spawnDelayPerWave, spawnDelayPerWave);
    }
    private void UpdateRespawnPoints()
    {
        //Updates valid respawn points
        var children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform point in children)
        {
            if (point.tag is "RespawnPoint")
            {
                spawnPoints.Add(point.gameObject);
            }
        }
        if (spawnPoints.Count == 0)
        {
            throw new System.Exception("Respawn points are 0!, make sure that there are respawn points contained in " + this.gameObject + "'s hierarchy or that they are correctly tagged!");
        }
    }

    private void HandleSpawn()
    {
        for(int i = 0; i < spawnAmountPerWave; i++)
        {
            //Spawn the object with the correct data and faction
            GetRandomSpawnPoint().GetComponent<RespawnPoint>().SpawnShip(this.GetComponent<ShipData>());
        }
    }

    private GameObject GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }
}
