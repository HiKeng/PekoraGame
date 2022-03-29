using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Abilities : MonoBehaviour
{
    [Header("Generic Parameters")]
    public bool _hasLimitedLiftTime = true;
    public float _liftTime = 3f;
    [HideInInspector] public Transform _originPoint;

    [Header("Caster")]
    public GameObject _caster;

    [Header("Events")]
    public UnityEvent _onStart;
    public UnityEvent _onEndLifeTime;

    public virtual void Start()
    {
        _originPoint = transform;

        _onStart.Invoke();

        if(_hasLimitedLiftTime)
        {
            StartCoroutine(_StartLifeTimeCount(_liftTime));
        }
    }

    public virtual void _SetCaster(GameObject _targetCaster)
    {
        _caster = _targetCaster;
    }

    IEnumerator _StartLifeTimeCount(float WaitTime)
    {
        yield return new WaitForSeconds(WaitTime);
        _EndAbility();
    }

    public virtual void _EndAbility()
    {
        _onEndLifeTime.Invoke();
        Destroy(gameObject);
    }
}
