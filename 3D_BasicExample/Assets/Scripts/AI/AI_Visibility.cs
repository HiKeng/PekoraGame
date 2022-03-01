using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Visibility : MonoBehaviour
{
    public AI_Behavior aiBehavior;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") == true)
        {
            aiBehavior.targetToFollow = other.gameObject;
            print("Found Player");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") == true)
        {
            aiBehavior.targetToFollow = null;
            aiBehavior.onMoving = false;
            aiBehavior.navigationAgent.SetDestination(gameObject.transform.position); // End Moving
            print("Lost Sight Player");
        }
    }
}
