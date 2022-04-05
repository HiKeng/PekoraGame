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
    public float cameraMinimumVerticalAngle = -10.0f;

    float horizontalDistanceToCamera = 0.0f;
    float verticalDistanceToCamera = 0.0f;

    bool onFirstCalculate = true;

    Vector3 cameraOriginalPosition = new Vector3(0, 0, 0);
    Vector3 cameraOriginalRotation = new Vector3(0, 0, 0);
    Vector3 cameraLocationOffset = new Vector3(0, 0, 0);
    Vector3 cameraRotationOffset = new Vector3(0, 0, 0);

    [SerializeField] bool _isFreeze = false;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 playerLocation = gameObject.transform.position;
        Vector3 playerLocationNoY = new Vector3(playerLocation.x, 0, playerLocation.z);

        Vector3 cameraLocation = targetCamera.transform.position;
        Vector3 cameraLocationNoY = new Vector3(cameraLocation.x, 0, cameraLocation.z);

        horizontalDistanceToCamera = Vector3.Distance(playerLocationNoY, cameraLocationNoY);

        Vector3 playerLocationNoX = new Vector3(0, playerLocation.y, playerLocation.z);
        Vector3 cameraLocationNoX = new Vector3(0, cameraLocation.y, cameraLocation.z);

        verticalDistanceToCamera = Vector3.Distance(playerLocationNoX, cameraLocationNoX);

        cameraOriginalPosition = targetCamera.transform.position;
        cameraOriginalRotation = targetCamera.transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        _mouseDisable();

        //

        Update_MouseHorizontalMovement();
        Update_MouseVerticalMovement();

        Update_CalculateCameraOffset();
        Update_CameraCollision();
    }

    void _mouseDisable()
    {
        Cursor.visible = _isFreeze;
        Screen.lockCursor = _isFreeze;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isFreeze = !_isFreeze;
        }
    }

    void Update_MouseHorizontalMovement()
    {
        float mouseAxisUpdate_X = Input.GetAxis("Mouse X");
        mouseAxisUpdate_X = mouseAxisUpdate_X * cameraHorizontalSpeed;          // Apply multiplier to make it faster or slower.
        Vector3 currentCameraAngle = targetCamera.transform.eulerAngles;
        float nextCameraAngle = currentCameraAngle.y + mouseAxisUpdate_X;       // Update Camera Angle by mouseAxisUpdate_X.

        targetCamera.transform.eulerAngles = new Vector3(currentCameraAngle.x, nextCameraAngle, currentCameraAngle.z);  // Set only Y.

        // Location Formula -> Location = new Vector( D * Sin() , CurrentY , D * Cos() )
        // Sin() and Cos() in Unity use Radian Unit not Degree Unit.
        // Formula to convert Degree to Radian is Degree * PI / 180.0f.
        float nextCameraRadian = (nextCameraAngle + 180.0f) * Mathf.PI / 180.0f;
        // If player is not at (0,0,0), we have to add current Player Location X to this formula.
        float nextCameraPosX = gameObject.transform.position.x + (horizontalDistanceToCamera * Mathf.Sin(nextCameraRadian));
        float nextCameraPosZ = gameObject.transform.position.z + (horizontalDistanceToCamera * Mathf.Cos(nextCameraRadian));

        targetCamera.transform.position = new Vector3(nextCameraPosX, targetCamera.transform.position.y, nextCameraPosZ);
    }

    void Update_MouseVerticalMovement()
    {
        float mouseAxisUpdate_Y = Input.GetAxis("Mouse Y");
        mouseAxisUpdate_Y = mouseAxisUpdate_Y * cameraVerticalSpeed;

        Vector3 currentCameraAngle = targetCamera.transform.eulerAngles;
        float nextCameraAngle = currentCameraAngle.x - mouseAxisUpdate_Y;

        if (nextCameraAngle > 90.0f)
        {
            nextCameraAngle = nextCameraAngle - 360.0f;     // Fix Unity calculation for Degree. Sometime Unity jyst change -10 -> 350
        }

        if (nextCameraAngle > cameraMaximumVerticalAngle)
        {
            nextCameraAngle = cameraMaximumVerticalAngle;
        }

        if (nextCameraAngle < cameraMinimumVerticalAngle)
        {
            nextCameraAngle = cameraMinimumVerticalAngle;
        }

        targetCamera.transform.eulerAngles = new Vector3(nextCameraAngle, currentCameraAngle.y, currentCameraAngle.z);  // Set only Y.

        float nextCameraRadian = (nextCameraAngle + 180.0f) * Mathf.PI / 180.0f;
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

    void Update_CalculateCameraOffset()
    {
        if (onFirstCalculate == true)
        {
            onFirstCalculate = false;
            cameraLocationOffset = cameraOriginalPosition - targetCamera.transform.position;
            cameraRotationOffset = cameraOriginalRotation - targetCamera.transform.eulerAngles;
        }

        Vector3 rightDirection = new Vector3(targetCamera.transform.right.x, 0, targetCamera.transform.right.z);
        Vector3 upDirection = new Vector3(0, targetCamera.transform.up.y, 0);
        Vector3 forwardDirection = new Vector3(targetCamera.transform.forward.x, 0, targetCamera.transform.forward.z);

        Vector3 offsetToRight = rightDirection * cameraLocationOffset.x;
        Vector3 offsetToUp = upDirection * cameraLocationOffset.y;
        Vector3 offsetToForward = forwardDirection * cameraLocationOffset.z;

        targetCamera.transform.position += offsetToRight + offsetToUp + offsetToForward;
        targetCamera.transform.eulerAngles += cameraRotationOffset;
    }

    void Update_CameraCollision()
    {
        RaycastHit hitInfo; // This one will get Collision Information the RayCase will get.

        // PlayerLocation with some offset to point ray from top of the head of player character.
        Vector3 playerLocation = gameObject.transform.position + new Vector3(0, 1.5f, 0f);
        Vector3 cameraLocation = targetCamera.transform.position;
        Vector3 directionPlayerToCamera = (cameraLocation - playerLocation).normalized;
        float distancePlayerToCamera = Vector3.Distance(playerLocation, cameraLocation);

        // Debug.DrawRay( StartLocation, Direction * RayDistance, Color);
        Debug.DrawRay(playerLocation, directionPlayerToCamera * distancePlayerToCamera, Color.red);

        // Physics.Raycast(StartLocation, Direction, out, distance);
        if (Physics.Raycast(playerLocation, directionPlayerToCamera, out hitInfo, distancePlayerToCamera))
        {
            // Wall Collision Check
            Vector3 hitLocation_Wall = hitInfo.point;
            Vector3 hitLocation_WallNoY = new Vector3(hitLocation_Wall.x, 0, hitLocation_Wall.z);
            Vector3 playerLocationNoY = new Vector3(playerLocation.x, 0, playerLocation.z);
            Vector3 cameraLocationNoY = new Vector3(cameraLocation.x, 0, cameraLocation.z);

            float distancePlayerToHitPointNoY = Vector3.Distance(playerLocationNoY, hitLocation_WallNoY);
            float distancePlayerToCameraNoY = Vector3.Distance(playerLocationNoY, cameraLocationNoY);
            float distanceDifferntNoY = distancePlayerToCameraNoY - distancePlayerToHitPointNoY;
            // Right here we now know how far we should move the camera to not block by the wall.

            Vector3 cameraForwardNoY = targetCamera.transform.position;
            cameraForwardNoY = new Vector3(cameraForwardNoY.x, 0, cameraForwardNoY.z);
            cameraForwardNoY = cameraForwardNoY.normalized; // Get only Direction. Remove size and length.

            // Move it forward to the front of the Wall.
            targetCamera.transform.position += cameraForwardNoY * distanceDifferntNoY * 1.1f;
        }
    }

    public void UnfreezeMouse()
    {
        _isFreeze = false;
    }
}
