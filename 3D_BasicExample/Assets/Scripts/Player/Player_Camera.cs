using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    public GameObject targetCamera;
    public float cameraHorizontalSpeed = 1.0f;
    public float cameraVerticalSpeed = 1.0f;

    public float cameraMaximumVerticalAngle = 80.0f;
    public float cameraMinimumVertialAngle = -10.0f;

    float horizontalDistanceToCamera = 0.0f;
    float verticalDistanceToCamera = 0.0f;

    void Start()
    {
        // Horizontal

        Vector3 playerLocation = gameObject.transform.position;
        Vector3 playerLocationNoY = new Vector3(playerLocation.x, 0, playerLocation.z);

        Vector3 cameraLocation = targetCamera.transform.position;
        Vector3 cameraLocationNoY = new Vector3(cameraLocation.x, 0, cameraLocation.z);

        horizontalDistanceToCamera = Vector3.Distance(playerLocationNoY, cameraLocationNoY);

        // Vertical

        Vector3 playerLocationNoX = new Vector3(0, playerLocation.y, playerLocation.z);
        Vector3 cameraLoacationNoX = new Vector3(0, cameraLocation.y, cameraLocation.z);

        verticalDistanceToCamera = Vector3.Distance(playerLocationNoX, cameraLoacationNoX);
    }

    void Update()
    {
        Update_MouseHorizontalMovement();
        Update_MouseVerticalMovement();
    }

    void Update_MouseHorizontalMovement()
    {
        float mouseAxisUpdate_X = Input.GetAxis("Mouse X");
        mouseAxisUpdate_X = mouseAxisUpdate_X * cameraHorizontalSpeed; // Apply multiplier to make it faster or slower.
        Vector3 currentCameraAngle = targetCamera.transform.eulerAngles;
        float nextCameraAngle = currentCameraAngle.y + mouseAxisUpdate_X; // Update Camera Angle by mouseAxisUpdate_X.


        targetCamera.transform.eulerAngles = new Vector3(nextCameraAngle, currentCameraAngle.y, currentCameraAngle.z); // Set only Y.

        // Location Formula -> Location = new Vector(D * Sin(), CurrentY, D * Cos() )
        // Sin() and Cos() in Unity use Radian Unit not Degree Unit.
        // Formula to convert Degree to Radian is Degree * PI / 180.0f.
        float nextCameraRadian = (nextCameraAngle + 180.0f) * Mathf.PI / 180.0f;

        // If player is not at (0,0,0), we have to add current Player Location X to this formula.
        float nextCameraPosY = gameObject.transform.position.y - (verticalDistanceToCamera * Mathf.Sin(nextCameraRadian));
        float nextCameraPosZ = verticalDistanceToCamera * Mathf.Cos(nextCameraRadian);

        nextCameraPosZ += verticalDistanceToCamera;
        Vector3 currentCameraLocation = targetCamera.transform.position;
        Vector3 cameraForwardNoY = targetCamera.transform.forward;
        cameraForwardNoY = new Vector3(cameraForwardNoY.x, 0, cameraForwardNoY.z);
        cameraForwardNoY = cameraForwardNoY.normalized;
        currentCameraLocation += cameraForwardNoY * nextCameraPosZ;

        targetCamera.transform.position = new Vector3(currentCameraLocation.x, nextCameraPosY, currentCameraLocation.z);
     }

    private void Update_MouseVerticalMovement()
    {
        float mouseAxisUpdate_Y = Input.GetAxis("Mouse Y");
        mouseAxisUpdate_Y = mouseAxisUpdate_Y * cameraVerticalSpeed;

        Vector3 currentCameraAngle = targetCamera.transform.eulerAngles;
        float nextCameraAngle = currentCameraAngle.x - mouseAxisUpdate_Y;

        if(nextCameraAngle > 100.0f)
        {
            nextCameraAngle = nextCameraAngle - 360.0f; // Fix Unity calulation for Degree. Sometime Unity just change -10 -> 350.
        }

        if(nextCameraAngle < cameraMaximumVerticalAngle)
        {
            nextCameraAngle = cameraMaximumVerticalAngle;
        }

        if(nextCameraAngle < cameraMaximumVerticalAngle)
        {
            nextCameraAngle = cameraMinimumVertialAngle;
        }

        targetCamera.transform.eulerAngles = new Vector3(nextCameraAngle, currentCameraAngle.y, currentCameraAngle.z); // Set only Y

        float nextCameraRadian = (nextCameraAngle + 180.0f) * Mathf.PI / 180.0f;
        float nextCameraPosY = gameObject.transform.position.y + (verticalDistanceToCamera * Mathf.Sin(nextCameraRadian));
        float nextCameraPosZ = gameObject.transform.position.z + (verticalDistanceToCamera * Mathf.Cos(nextCameraRadian));

        Vector3 currentCameraPosition = targetCamera.gameObject.transform.position;
        float finalNextCameraPosZ = gameObject.transform.position.z - (currentCameraPosition.z - nextCameraPosZ);
        targetCamera.transform.position = new Vector3(targetCamera.transform.position.x, nextCameraPosY, nextCameraPosZ);
    }
}
