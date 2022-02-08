using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsExample : MonoBehaviour
{
    public Rigidbody physicsSystem;


    // Start is called before the first frame update
    void Start()
    {
        //We can set this one to false when you want to do air attack.
        physicsSystem.useGravity = false;

        // Set it to True again after we finish Air attack.
        physicsSystem.useGravity = true;

        // AddForce a command to push something.
        // Like this example, We push this cube Right for 10 and Up for 100.
        // Force try to rise speed overtime until reach the max speed.
        physicsSystem.AddForce(new Vector3(200, 300, 0), ForceMode.Force);

        // Impulse -> Reach max speed immdiatly.
        physicsSystem.AddForce(new Vector3(200, 300, 0), ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        // This is the command to set Velocity to object directly.
        // For example, this -> Set Velocity (1.0f in Right, 0 in Up and 0 in Forward Direction.
        // Set it in Update to let it set all the time.
        physicsSystem.velocity = new Vector3(10, 0, 0);
    }
}
