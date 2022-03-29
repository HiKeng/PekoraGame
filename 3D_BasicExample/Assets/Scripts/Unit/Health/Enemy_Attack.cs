using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Enemy_Attack : MonoBehaviour
{
    #region Variables

    [Header("Reference")]
    [SerializeField] BoxCollider _hitBox;

    [Header("Properties")]
    [SerializeField] float _detectionRadius = 5f;
    [SerializeField] LayerMask _targetMask = new LayerMask();
    [SerializeField] float _attackInterval = 2f;
    [SerializeField] bool _isAttackOnCoolDown = false;

    [SerializeField] float _offsetAttackRange = 3f;

    public GameObject _lockOnTarget;

    Rigidbody _rigidbody;

    [Header("Events")]
    [SerializeField] UnityEvent _onAttack;

    [Header("Debug")]
    [SerializeField] bool _drawHurtBoxGizmos = true;

    #endregion

    #region Awake
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    #endregion

    #region Methods
    public void _SetHitBoxActive(bool _isActive)
    {
        _hitBox.enabled = _isActive;
    }

    private void Update()
    {
        if (_lockOnTarget == null) { return; }

        _EnemyAttack(_lockOnTarget);
        _checkTargetDistance();
    }

    void _detectTarget(GameObject _target)
    {
        _lockOnTarget = _target;
    }

    public bool _IsTargetLayerMask(GameObject obj)
    {
        return ((_targetMask.value & (1 << obj.layer)) > 0);
    }

    void _checkTargetDistance()
    {
        Vector3 _distance = _lockOnTarget.transform.position - transform.position;

        if (_distance.magnitude >= _GetOffsetAttackRange())
        {
            _lockOnTarget = null;
        }
    }

    public void _EnemyAttack(GameObject _target)
    {
        if (_isAttackOnCoolDown) { return; }

        //for (int i = 0; i < GetComponent<EnemyCore>()._weaponPartAttached.Count; i++)
        //{
        //    if (GetComponent<EnemyCore>()._weaponPartAttached[i]._weaponAttached != null)
        //    {
        //        GetComponent<EnemyCore>()._weaponPartAttached[i]._weaponAttached._Attack(_target);
        //    }
        //}

        //////////////// Attack here
        ///
        Debug.Log("Attack");

        _rigidbody.velocity = Vector3.zero;

        GetComponent<LookAtPlayer>()._LookAtPlayer();

        GetComponent<Animator>().SetTrigger("Attack");

        StartCoroutine(_AttackCoolDownCount(_attackInterval));

        _onAttack.Invoke();
    }

    IEnumerator _AttackCoolDownCount(float IntervalTime)
    {
        _isAttackOnCoolDown = true;
        yield return new WaitForSeconds(IntervalTime);
        _isAttackOnCoolDown = false;
    }

    public float _GetOffsetAttackRange()
    {
        return GetComponent<CapsuleCollider>().radius + _offsetAttackRange;
    }

    #endregion

    #region Collision Method

    private void OnTriggerEnter(Collider other)
    {
        if (_CheckOtherIsTargetToFight(other.gameObject))
        {
            _detectTarget(other.gameObject);

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_CheckOtherIsTargetToFight(other.gameObject))
        {
            if (_lockOnTarget == null)
            {
                _detectTarget(other.gameObject);
            }
        }
    }

    bool _CheckOtherIsTargetToFight(GameObject _target)
    {
        return _target.GetComponent<Player_MovementController>() != null || _target.GetComponent<BuildingHealth>() != null;
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _GetOffsetAttackRange());


        Gizmos.color = Color.red;
        Gizmos.matrix = _hitBox.transform.localToWorldMatrix;

        if (!_drawHurtBoxGizmos) { return; }
        if (!_hitBox.enabled) { return; }

        Gizmos.DrawCube(Vector3.zero, _hitBox.size);
    }

    private void OnDrawGizmos()
    {
        
    }

    #endregion
}
