using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player_Attack : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] BoxCollider _hitBox;
    [SerializeField] SO_HurtBox _hurtBox;

    [Header("Debug")]
    [SerializeField] bool _drawGizmos;
    [SerializeField] bool _useCheat = false;
    [SerializeField] SO_HurtBox _cheatHurtBox;

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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            if(!_useCheat)
            {
                _hitBox.GetComponent<HurtBox>()._hurtBoxSO = _cheatHurtBox; // Start cheat
            }
            else
            {
                _hitBox.GetComponent<HurtBox>()._hurtBoxSO = _hurtBox; // Stop cheat
            }

            _useCheat = !_useCheat;
        }
    }
}
