using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : UnitHealth
{
    public override void _Dead()
    {
        if(GetComponent<Animator>().GetBool("isTriggeredDead")) { return; }

        GetComponent<Animator>().SetTrigger("Dead");
        GetComponent<Animator>().SetBool("isDead", true);

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<CapsuleCollider>().isTrigger = true;
        _onDead.Invoke();
    }
}
