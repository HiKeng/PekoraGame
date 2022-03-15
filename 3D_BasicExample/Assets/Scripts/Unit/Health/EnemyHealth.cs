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

        _onDead.Invoke();
    }
}
