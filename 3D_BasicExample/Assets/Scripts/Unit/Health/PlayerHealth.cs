using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : UnitHealth
{
    public override void _Dead()
    {
        if (GetComponent<Animator>().GetBool("isTriggeredDead")) { return; }

        _HealthUI_UpdateValue();

        GetComponent<Animator>().SetTrigger("Dead");
        GetComponent<Animator>().SetBool("isDead", true);

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<CapsuleCollider>().isTrigger = true;
        _onDead.Invoke();
    }

    public void _SetDead()
    {
        _currentHealth = 0;
    }
}
