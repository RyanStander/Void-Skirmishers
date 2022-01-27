using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Debugger : MonoBehaviour
{
    [SerializeField] private bool isDebuggerActive = false;
    [SerializeField] private GameObject allyFighter;
    [SerializeField] private GameObject enemyFighter;
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DebuggerEnableDisable();
        if (isDebuggerActive)
        {
            SpawnAllyFighter();
            SpawnEnemyFighter();
            DealDamageToAllShips();
        }
    }

    private void DebuggerEnableDisable()
    {
        if (Input.GetKey(KeyCode.RightShift)&&Input.GetKey(KeyCode.O) &&Input.GetKeyDown(KeyCode.P))
        {
            isDebuggerActive =!isDebuggerActive;
            Debug.Log("Debugger Intiated = " + isDebuggerActive.ToString());
        }
    }

    private void SpawnAllyFighter()
    {
        //creates an ally fighter
        if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Spawned Ally Fighter");
            GameObject clone = Instantiate(allyFighter);
        }
    }

    private void SpawnEnemyFighter()
    {
        //creates an enemy fighter
        if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Spawned Enemy Fighter");
            GameObject clone = Instantiate(enemyFighter);
        }
    }

    private void DealDamageToAllShips()
    {
        //Damages all ships
        if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Dealing Damage to all Ships");
            var shipHealth = FindObjectsOfType<Health>();
            foreach(Health health in shipHealth)
            {
                health.TakeDamage("FactionUndefined", "DebuggerAttacker", 5);
                //health.TakeDamage("FactionUndefined", "bot", 5);
            }
        }
    }
}
