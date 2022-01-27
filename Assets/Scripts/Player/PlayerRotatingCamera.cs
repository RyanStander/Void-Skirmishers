using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotatingCamera : MonoBehaviour
{
    public float cameraSpeed = 75; //Rotation speed for camera
    public Transform targetObject=null, mainCamera = null;

    //Direction that the player is pointing the camera
    public TurretOrientation orientation = TurretOrientation.UNDEFINED;

    private void OnEnable()
    {
        EventManager.Subscribe(EventType.PLAYERSPAWNEVENT,OnPlayerSpawn);
    }
    private void OnDisable()
    {
        EventManager.Unsubscribe(EventType.PLAYERSPAWNEVENT, OnPlayerSpawn);
    }

    private void Start()
    {
        //Mouse Locking and visibility toggling
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //Update is called once per frame
    void Update()
    {
        //Makes the rotating camera holder stick to the target object and avoids errors
        if (targetObject)
        {
            this.transform.position = targetObject.position;

            //Updates the orientation
            UpdateOrientationToTarget();
        }
    }

    void LateUpdate()
    {
        CameraShift();
    }
    
    //Rotates the camera to constantly look at where the boat is
    private void CameraShift()
    {
            //Rotates the camera holder based on the (Horizontal) mouse position/movement
            transform.Rotate(Vector3.up * Time.deltaTime * cameraSpeed * Input.GetAxis("Mouse X"));
    }

    private void UpdateOrientationToTarget()
    {
        //Allows for a degree where either one is considered an origin and rotation of another may be treated as local location
        int adjustedDegree = (int)CombineSingleEulerAxis(mainCamera.eulerAngles.y, targetObject.eulerAngles.y);

        //If viewing from the backside, aiming forward
        if (adjustedDegree <= 30 || adjustedDegree >= 330)
        {
            orientation = TurretOrientation.FRONT;
        }
        //If viewing from the right, aiming leftward
        if (adjustedDegree < 150 && adjustedDegree > 30)
        {
            orientation = TurretOrientation.LEFT;
        }
        //If viewing from the front, aiming backwards
        if (adjustedDegree <= 210 && adjustedDegree >= 150)
        {
            orientation = TurretOrientation.BACK;
        }
        //If viewing from the left, aiming rightward
        if (adjustedDegree < 330 && adjustedDegree > 210)
        {
            orientation = TurretOrientation.RIGHT;
        }
      
        //Debug.Log("Degree: " + adjustedDegree);
        //Debug.Log("Orientation: " + orientation);
    }
    public float CombineSingleEulerAxis(float firstDegree, float secondDegree)
    {
        //First gets the difference between both degrees
        float degree = (float)(firstDegree - secondDegree);
        //Then adds 360 if negative, as this will allow it to be on a 360 circle scale
        //if (degree < 0) degree += 360f;

        //However, as an adjusted and specific degree style is needed, the following is used instead
        if (degree > 0) degree -= 360f;

        return degree * -1;
    }

    //Update target object to follow
    private void OnPlayerSpawn(EventData eventData)
    {
        //Cast and error handling to make sure that the correct type of EventData is being recieved
        if (eventData is PlayerSpawnEventData userEventData)
        {
            targetObject = userEventData.followPoint;
            //If no, then do nothing
        }
        else
        {
            //Unity Player
            Debug.Log("Warning: Given EventData is not the same as the permitted PlayerSpawnEventData");
        }
    }

}
