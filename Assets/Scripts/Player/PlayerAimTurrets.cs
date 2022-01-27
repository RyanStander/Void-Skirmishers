using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerAimTurrets : MonoBehaviour
{
    [SerializeField] private TurretContainer TurretContainer = null;

    [Header("Aiming")]
    [SerializeField] private GameObject leftRangeFinder = null;
    [SerializeField] private GameObject rightRangeFinder = null;
    [SerializeField] private GameObject frontRangeFinder = null;

    [Header("Sail Aim Settings")]
    [SerializeField] private List<MeshRenderer> sailMesh = null;
    [SerializeField] private Material sailNormalMat = null;
    [SerializeField] private Material sailTransMat = null;

    [Range(1, 25)]
    [SerializeField] private float verticalSpeed=10;

    //values of range finder
    [Range(1,500)]
    [SerializeField] private float sideRangeFinderLength = 12, sideRangeFinderWidth = 5,frontRangeFinderLength=20,frontRangeFinderWidth=1;

    //values for aiming constraints
    [Range(0,180)]
    [SerializeField] float sideTurretMaxYAngle=90,sideTurretMaxZAngle=30,frontTurretMaxYAngle=15,frontTurretMaxZAngle=5;
    // Update is called once per frame
    private void Update()
    {
        AimWithCamera();
        DisableAllRangeFinders();
        AimHideSails();
    }
    //----------------------------------------------
    //            Rotating with camera
    //----------------------------------------------
    private void RotateToPoint(List<TurretData> turrets,float MaxYAngle)
    {
        //Relative positions for camera and harpoon position
        Vector3 relativePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        foreach (TurretData turret in turrets)
        {
            //Rotation to check
            Quaternion possibleRotation = Quaternion.Euler(0, Quaternion.LookRotation(relativePosition).eulerAngles.y - 90, turret.turretBody.rotation.z);

            //Save last valid rotation
            Quaternion lastValidRotation = turret.turretBody.transform.rotation;

            //Rotate base to target point
            turret.turretBody.transform.rotation = possibleRotation;

            //Ensures the expected plunger rotation does not go over its limits
            if (turret.turretBody.localEulerAngles.y > MaxYAngle && turret.turretBody.localEulerAngles.y < 360 - MaxYAngle)
            {
                //Corrects to previous valid rotation
                turret.turretBody.transform.rotation = lastValidRotation;
            }
        }
        
    }

    //----------------------------------------------
    //            Rotating with mouse
    //----------------------------------------------
    private void VerticalRotateByMouse(List<TurretData> turrets,float MaxZAngle)
    {
        float mouseMovement = Input.GetAxis("Mouse Y")* verticalSpeed;
        foreach (TurretData turret in turrets)
        {
            Quaternion lastValidRotation = turret.turretHead.transform.rotation;

            turret.turretHead.Rotate(0, 0, -mouseMovement);

            if (turret.turretHead.localEulerAngles.z > MaxZAngle && turret.turretHead.localEulerAngles.z < 360 - MaxZAngle)
            {
                turret.turretHead.transform.rotation = lastValidRotation;
            }
        }
    }

    //----------------------------------------------
    //            Camera Orientation
    //----------------------------------------------
    private void AimWithCamera()
    {
        //checks the orientation of the camera to see what turrets to aim with
        TurretOrientation orientation = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerRotatingCamera>().orientation;
        switch (orientation)
        {
            case TurretOrientation.FRONT:
                FrontTurretAim();
                break;
            case TurretOrientation.LEFT:
                LeftTurretAim();
                break;
            case TurretOrientation.RIGHT:
                RightTurretAim();
                break;
        }
    }
    //----------------------------------------------
    //                    Aiming
    //----------------------------------------------
    //below scripts used to aim based on orientation
    private void LeftTurretAim()
    {
        List<TurretData> turrets = TurretContainer.GetLeftTurrets();
        RotateToPoint(turrets, sideTurretMaxYAngle);
        VerticalRotateByMouse(turrets,sideTurretMaxZAngle);
        SideRangeFinder(leftRangeFinder);        
    }

    private void RightTurretAim()
    {
        List<TurretData> turrets = TurretContainer.GetRightTurrets();
        RotateToPoint(turrets, sideTurretMaxYAngle);
        VerticalRotateByMouse(turrets, sideTurretMaxZAngle);
        SideRangeFinder(rightRangeFinder);
    }

    private void FrontTurretAim()
    {
        List<TurretData> turrets = TurretContainer.GetFrontTurrets();
        RotateToPoint(turrets, frontTurretMaxYAngle);
        VerticalRotateByMouse(turrets,frontTurretMaxZAngle);
        FrontRangeFinder(frontRangeFinder);
    }
    //----------------------------------------------
    //                 RangeFinder
    //----------------------------------------------
    //shows the trajectory of bullets
    private void SideRangeFinder(GameObject rangeFinderObject)
    {
        if (Input.GetMouseButtonDown(1))
        {
            rangeFinderObject.SetActive(true);
            rangeFinderObject.GetComponent<RangeFinderSize>().UpdateSize(sideRangeFinderLength, sideRangeFinderWidth);
        }
    }
    private void FrontRangeFinder(GameObject rangeFinderObject)
    {
        if (Input.GetMouseButtonDown(1))
        {
            rangeFinderObject.SetActive(true);
            rangeFinderObject.GetComponent<RangeFinderSize>().UpdateSize(frontRangeFinderLength, frontRangeFinderWidth);
        }
    }
    //disables all the range finders
    private void DisableAllRangeFinders()
    {
        if (Input.GetMouseButtonUp(1))
        {
            rightRangeFinder.SetActive(false);
            leftRangeFinder.SetActive(false);
            frontRangeFinder.SetActive(false);
        }
    }

    private void AimHideSails()
    {
        if (Input.GetMouseButtonUp(1))
        {
            foreach (MeshRenderer mesh in sailMesh)
            {
                mesh.material = sailNormalMat;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            foreach (MeshRenderer mesh in sailMesh)
            {
                mesh.material = sailTransMat;
            }
        }
    }
}