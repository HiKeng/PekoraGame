using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player_Attack : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] BoxCollider _hitBox;

    [Header("Debug")]
    [SerializeField] bool _drawGizmos;

    [Header("Events")]
    public UnityEvent _onStartAttack1;
    public UnityEvent _onStartAttack2;
    public UnityEvent _onHit;

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
