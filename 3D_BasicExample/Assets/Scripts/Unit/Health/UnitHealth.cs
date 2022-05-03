using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UnitHealth : MonoBehaviour
{
    [SerializeField] float _maxHealth = 100;
    protected float _currentHealth;

    [SerializeField] bool _isDead = false;

    [Header("UI")]
    [SerializeField] Slider _healthUI;

    [Header("Debug")]
    [SerializeField] bool _useDebugInput = false;
    [SerializeField] bool _useDebugPrint = false;

    [Header("Events")]
    public  UnityEvent _onTakeDamage;
    public UnityEvent _onDead;

    protected virtual void Start()
    {
        _currentHealth = _maxHealth;
    }

    void Update()
    {
        if(_useDebugInput)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                _TakeDamage(20);
            }
        }
        
        _HealthUI_UpdateValue();
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

        if(_useDebugPrint) { Debug.Log(this.name + " | Current Health = " + _currentHealth); }
        

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

    public virtual void _HealthUI_UpdateValue()
    {
        if(_healthUI == null) { return; }
        _healthUI.value = _currentHealth / _maxHealth;
    }
}
