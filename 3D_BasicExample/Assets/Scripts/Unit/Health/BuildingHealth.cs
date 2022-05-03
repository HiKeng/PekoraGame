using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHealth : UnitHealth
{
    public override void _Dead()
    {
        _RemoveFromCurrentActiveList();
        base._Dead();
    }

    // When build was destoryed, it will be removed from current active list.
    public void _RemoveFromCurrentActiveList()
    {
        BuildingCounter.Instance._RemoveEnemyFromCurrentActiveList(this);
    }
}
