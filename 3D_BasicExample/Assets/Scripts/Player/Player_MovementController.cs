using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_MovementController : MonoBehaviour
{
    public GameObject targetCamera;
    public Rigidbody physicsSystem;
    public Animator animatorSystem;

    public float movementSpeed = 10.0f;
    public float turnSpeed = 10.0f;
    public float jumpPower = 10.0f;
    bool onAttacking = false;
    bool onJumping = false;

    float _delayedVelocityUpdate = 0;
    [SerializeField] float _velocityUpdateDelayTime = 0.5f;
    [SerializeField] float _animationLerpSpeed = 3;
    
    void Start()
    {
        
    }

    void Update()
    {
        if(!onAttacking) // Cannot move when this character still attacking.
        {
            Update_Movement();
            Update_Attacking();
        }

        Update_JumpMovement();
        Update_Animation();
    }

    void Update_Movement()
    {
        Vector3 resultVelocity = new Vector3(0, physicsSystem.velocity.y, 0);

        if (Input.GetKey(KeyCode.W))
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

            _UpdateVelocityLerp(true);
        }
        else
        {
            _UpdateVelocityLerp(false);
        }
    }

    private void _UpdateVelocityLerp(bool _isMove)
    {
        int _multiplier = _isMove ? 1 : -1;

        animatorSystem.SetFloat("VelocityLerp", animatorSystem.GetFloat("VelocityLerp") + Time.deltaTime * _animationLerpSpeed * _multiplier);

        if (animatorSystem.GetFloat("VelocityLerp") > 1)
        {
            animatorSystem.SetFloat("VelocityLerp", 1);
        }
        else if (animatorSystem.GetFloat("VelocityLerp") < 0)
        {
            animatorSystem.SetFloat("VelocityLerp", 0);
        }
    }

    void Update_JumpMovement()
    {
        if(Input.GetKeyDown(KeyCode.Space) == true)
        {
            physicsSystem.AddForce(new Vector3(0, jumpPower, 0));
        }
    }

    private void Update_Attacking()
    {
        if(Input.GetButtonDown("Fire1") == true) // Fire1 is Left mouse click.
        {
            if(onAttacking == false)
            {
                Debug.Log("Attack");
                Start_AttackingState();
                animatorSystem.SetTrigger("DoAttacking");
            }
        }
    }

    private void Update_Animation()
    {
        float currentSpeed = physicsSystem.velocity.magnitude;
        animatorSystem.SetFloat("CurrentVelocity", currentSpeed);

        StartCoroutine(DelayedUpdateVelocity(currentSpeed));
    }

    IEnumerator DelayedUpdateVelocity(float _currentVelocity)
    {
        yield return new WaitForSeconds(_velocityUpdateDelayTime);

        _delayedVelocityUpdate = _currentVelocity;
    }

    public void Start_AttackingState()
    {
        onAttacking = true; // Set Attacking state to true.
        physicsSystem.velocity = Vector3.zero;
    }

    public void End_AttackingState()
    {
        onAttacking = false; // Set Attacking state to false.
    }

    private void OnCollisionEnter(Collision hitWithObject)
    {
        Vector3 playerPosition = gameObject.transform.position + new Vector3(0, 1, 0);
        Vector3 contactPoint = Get_CenterOfContactPoint(hitWithObject);

        if(playerPosition.y >= contactPoint.y) // If contact point is below the player. It means it should contact ground.
        {
            onJumping = false;
        }
    }

    Vector3 Get_CenterOfContactPoint(Collision hitWithObject)
    {
        // Get Average Position of Contact Points. Sum them all and divide by how many of them.

        Vector3 centerOfContactPoint = new Vector3(0, 0, 0);
        ContactPoint[] contactPointList = new ContactPoint[hitWithObject.contactCount]; // Create an Array to keep all contact points.
        hitWithObject.GetContacts(contactPointList);    // Get All contact points and set it into contactPointList.

        for (int index = 0; index < contactPointList.Length; index += 1)
        {
            centerOfContactPoint += contactPointList[index].point; // Sum all contant points location together.
        }

        centerOfContactPoint = centerOfContactPoint / hitWithObject.contactCount; // Calculate Average of them to find center.

        return centerOfContactPoint;
    }

    public void _DisableControl()
    {
        physicsSystem.velocity = Vector3.zero;
        this.enabled = false;
    }
}
