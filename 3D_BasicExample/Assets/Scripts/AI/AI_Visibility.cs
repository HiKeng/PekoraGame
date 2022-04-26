using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Visibility : MonoBehaviour
{
    public AI_Behavior aiBehavior;

    [Header("Debug")]
    [SerializeField] bool _DebugMessage = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") == true)
        {
            aiBehavior.targetToFollow = other.gameObject;

            if(_DebugMessage) { print("Found Player"); }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") == true)
        {
            aiBehavior.targetToFollow = null;
            aiBehavior.onMoving = false;
            aiBehavior.navigationAgent.SetDestination(gameObject.transform.position); // End Moving

            if (_DebugMessage) { print("Lost Sight Player"); }
        }
    }
}
