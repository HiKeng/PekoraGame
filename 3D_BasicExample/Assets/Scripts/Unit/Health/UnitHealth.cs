using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitHealth : MonoBehaviour
{
    [SerializeField] float _maxHealth = 100;
    [SerializeField] float _currentHealth;

    [SerializeField] bool _isDead = false;

    [Header("Events")]
    [SerializeField] UnityEvent _onTakeDamage;
    [SerializeField] UnityEvent _onDead;

    protected virtual void Start()
    {
        _currentHealth = _maxHealth;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            _TakeDamage(50);
        }
    }

    bool _isDeadCheck()
    {
        if(!_isDead && _currentHealth <= 0)
        {
            _isDead = true;
            _Dead();
            _onDead.Invoke();
        }

        return _isDead;
    }

    public virtual void _TakeDamage(float _damage)
    {
        _currentHealth -= _damage;
        _onTakeDamage.Invoke();

        Debug.Log(this.name + " | Current Health = " + _currentHealth);

        _isDeadCheck();
    }

    public virtual void _GetHit(SO_HurtBox _hurtbox)
    {
        _TakeDamage(_hurtbox._damage);
    }

    public virtual void _Dead()
    {
        gameObject.SetActive(false);
        _onDead.Invoke();
    }
}
