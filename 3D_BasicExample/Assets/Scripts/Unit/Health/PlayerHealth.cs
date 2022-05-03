using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : UnitHealth
{
    [SerializeField] UnityEvent _onCompleted;

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

    public void _StageCompleted()
    {
        _onCompleted.Invoke();
    }
}
