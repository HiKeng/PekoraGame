using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsExample_02 : MonoBehaviour
{
    public Rigidbody physicsSystem;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        physicsSystem.AddForce(new Vector3(10000, 0, 0), ForceMode.Force);
    }
}
