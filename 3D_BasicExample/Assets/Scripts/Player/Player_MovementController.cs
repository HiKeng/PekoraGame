using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_MovementController : MonoBehaviour
{
    public GameObject targetCamera;
    public Rigidbody physicsSystem;

    public float movementSpeed = 10.0f;
    public float turnSpeed = 10.0f;
    public float jumpPower = 10.0f;

    void Start()
    {
        
    }

    void Update()
    {
        Update_Movement();
        Update_JumpMovement();
    }

    void Update_Movement()
    {
        Vector3 resultVelocity = new Vector3(0, physicsSystem.velocity.y, 0);

        if(Input.GetKey(KeyCode.W) == true)
        {
            resultVelocity += targetCamera.transform.forward * movementSpeed;
        }

        if (Input.GetKey(KeyCode.S) == true)
        {
            resultVelocity -= targetCamera.transform.forward * movementSpeed;
        }

        if (Input.GetKey(KeyCode.D) == true)
        {
            resultVelocity += targetCamera.transform.right * movementSpeed;
        }

        if (Input.GetKey(KeyCode.A) == true)
        {
            resultVelocity -= targetCamera.transform.right * movementSpeed;
        }

        resultVelocity = new Vector3(resultVelocity.x, physicsSystem.velocity.y, resultVelocity.z);
        physicsSystem.velocity = resultVelocity;

        Vector3 velocityNoY = new Vector3(physicsSystem.velocity.x, 0, physicsSystem.velocity.z);

        if(velocityNoY.magnitude != 0) // Get Result Speed for All Direction as a float
        {
            // Point to the next position that Velocity will move this object to.
            Vector3 directionToPointTo = gameObject.transform.position + velocityNoY; // Current Position + Velocity = Next Location.
            Quaternion fromQuaterion = gameObject.transform.rotation; // Rotation before Look At
            gameObject.transform.LookAt(directionToPointTo);

            Quaternion toQuaternion = gameObject.transform.rotation; // Rotation after Look At

            gameObject.transform.rotation = Quaternion.Lerp(fromQuaterion, toQuaternion, turnSpeed * Time.deltaTime);
        }
    }

    void Update_JumpMovement()
    {
        if(Input.GetKeyDown(KeyCode.Space) == true)
        {
            physicsSystem.AddForce(new Vector3(0, jumpPower, 0));
        }
    }
}
