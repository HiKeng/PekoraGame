using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Behavior : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FollowPlayer_Behavior()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            
        }
    }

}
