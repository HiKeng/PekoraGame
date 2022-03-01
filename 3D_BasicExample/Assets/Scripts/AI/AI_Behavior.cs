using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AI_Behavior : MonoBehaviour
{
    public float randomPatrolRadius = 5.0f;

    [HideInInspector]
    public NavMeshAgent navigationAgent;

    float delayAfterMove = 1.0f;
    float delayCounter = 0.0f;

    [HideInInspector]
    public bool onMoving = false;
    [HideInInspector]
    public bool onDelayAfterMove = false;

    [HideInInspector]
    public GameObject targetToFollow = null;

    // Start is called before the first frame update
    void Start()
    {
        navigationAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(targetToFollow == null)
        {
            Patrol_Behavior();
        }
        else
        {
            FollowPlayer_Behavior();
        }
    }

    void Patrol_Behavior()
    {
        Vector3 currentLocation = gameObject.transform.position;
        Vector3 navigationDestination = navigationAgent.destination;

        if (onMoving == true
            && Vector3.Distance(currentLocation, navigationDestination) <= navigationAgent.stoppingDistance)
        {
            onMoving = false;
            onDelayAfterMove = true;
        }

        if (onDelayAfterMove == true)
        {
            delayCounter += Time.deltaTime;

            if (delayCounter >= delayAfterMove)
            {
                delayCounter = 0.0f;
                onDelayAfterMove = false;
            }
        }

        if (onMoving == false && onDelayAfterMove == false)
        {
            AIMove_Patrol();
        }
    }

    void AIMove_Patrol()
    {
        onMoving = true;
        float randomTargetPosX = Random.Range(-randomPatrolRadius, randomPatrolRadius);
        float randomTargetPosZ = Random.Range(-randomPatrolRadius, randomPatrolRadius);
        Vector3 targetPatrolPos = gameObject.transform.position + new Vector3(randomTargetPosX, 0, randomTargetPosZ);

        navigationAgent.SetDestination(targetPatrolPos);
    }

    void FollowPlayer_Behavior()
    {
        onMoving = true;
        onDelayAfterMove = false;
        delayCounter = 0.0f;
        navigationAgent.SetDestination(targetToFollow.gameObject.transform.position);
    }
}
