using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] BoxCollider _hitBox;

    [Header("Debug")]
    [SerializeField] bool _drawGizmos;

    public void _SetHitBoxActive(bool _isActive)
    {
        _hitBox.enabled = _isActive;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = _hitBox.transform.localToWorldMatrix;

        if (!_drawGizmos) { return; }
        if (!_hitBox.enabled) { return; }

        Gizmos.DrawCube(Vector3.zero, _hitBox.size);
    }
}
