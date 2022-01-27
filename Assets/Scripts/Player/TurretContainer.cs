using System.Collections.Generic;
using UnityEngine;

public class TurretContainer : MonoBehaviour
{
    [SerializeField] private List<TurretData> leftTurrets = new List<TurretData>();
    [SerializeField] private List<TurretData> rightTurrets = new List<TurretData>();
    [SerializeField] private List<TurretData> frontTurrets = new List<TurretData>();

    public List<TurretData> GetLeftTurrets()
    {
        return leftTurrets;
    }

    public TurretData GetLeftTurretByIndex(int index)
    {
        return leftTurrets[index];
    }

    public int GetLeftTurretsCount()
    {
        return leftTurrets.Count;
    }

    public List<TurretData> GetRightTurrets()
    {
        return rightTurrets;
    }

    public TurretData GetRightTurretByIndex(int index)
    {
        return rightTurrets[index];
    }

    public int GetRightTurretsCount()
    {
        return rightTurrets.Count;
    }

    public List<TurretData> GetFrontTurrets()
    {
        return frontTurrets;
    }

    public TurretData GetFrontTurretByIndex(int index)
    {
        return frontTurrets[index];
    }

    public int GetFrontTurretsCount()
    {
        return frontTurrets.Count;
    }
}
