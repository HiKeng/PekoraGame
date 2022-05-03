using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : UnitHealth
{
    public override void _Dead()
    {
        if(GetComponent<Animator>().GetBool("isTriggeredDead")) { return; }

        _HealthUI_UpdateValue();

        GetComponent<Animator>().SetTrigger("Dead");
        GetComponent<Animator>().SetBool("isDead", true);

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<CapsuleCollider>().isTrigger = true;

        _RemoveFromCurrentActiveList();
        _onDead.Invoke();
    }

    public void _GoDead()
    {
        _TakeDamage(_currentHealth);
    }

    // When enemy is dead, it will be removed from current active list.
    public void _RemoveFromCurrentActiveList()
    {
        EnemyWaveSpawner.Instance._RemoveEnemyFromCurrentActiveList(this);
    }
}
