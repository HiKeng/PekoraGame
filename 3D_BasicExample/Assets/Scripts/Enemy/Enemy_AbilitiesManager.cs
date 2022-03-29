using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AbilitiesManager : MonoBehaviour
{
    #region Variables

    [System.Serializable]
    public class AbilityProperties
    {
        [Header("Properties")]
        public string _name;
        public int _ID;
        public GameObject _prefab;
        public Vector3 _spawnOffset;

        [Header("Cooldown")]
        public float _cooldownDuration = 2f;
        public float _currentCooldownCount;

        [Header("Use Limit")]
        public bool _hasLimitedAmount = false;
        public int _limitAmount = 1;

        //

        public enum VFX_ActivateMethod
        {
            OneState,
            OverTime,
            None
        }

        [Header("Attached VFX")]
        public VFX_ActivateMethod _ActivateMethod;
        public List<GameObject> _AttachedVFX;
    }

    public List<AbilityProperties> _abilities;

    #endregion

    #region Private Methods

    void Update()
    {
        _CountCooldown();
    }

    void _CountCooldown()
    {
        for (int i = 0; i < _abilities.Count; i++)
        {
            if(_abilities[i]._currentCooldownCount > 0)
            {
                _abilities[i]._currentCooldownCount -= Time.deltaTime;
            }
        }
    }

    #endregion

    #region Public Methods

    public void _DecreaseLimitedAmount(int _abilityID)
    {
        if(_GetAbilityByID(_abilityID)._hasLimitedAmount)
        {
            _GetAbilityByID(_abilityID)._limitAmount--;
        }
    }

    public void _StartCountCooldown(int _abilityID)
    {
        _GetAbilityByID(_abilityID)._currentCooldownCount = _GetAbilityByID(_abilityID)._cooldownDuration;
    }

    public bool _IsAbilityAvailable(int _abilityID)
    {
        if(_AbilityHasLimitedAmount(_abilityID) && _IsRunningOutOfAbility(_abilityID))
        {
            return false;
        }

        return _GetAbilityByID(_abilityID)._currentCooldownCount <= 0;
    }

    public AbilityProperties _GetAbilityByID(int _abilityID)
    {
        for (int i = 0; i < _abilities.Count; i++)
        {
            if (_abilities[i]._ID == _abilityID)
            {
                return _abilities[i];
            }
        }

        return null;
    }

    public bool _IsRunningOutOfAbility(int _abilityID)
    {
        return _GetAbilityByID(_abilityID)._limitAmount <= 0;
    }

    public bool _AbilityHasLimitedAmount(int _abilityID)
    {
        return _GetAbilityByID(_abilityID)._hasLimitedAmount;
    }

    public void _SetCasterToAbility(GameObject _ability, int _abilityID)
    {
        if(_ability.GetComponent<Abilities>() == null) { return; }

        _ability.GetComponent<Abilities>()._SetCaster(this.gameObject);

        if(_AbilityHasLimitedAmount(_abilityID))
        {
            _DecreaseLimitedAmount(_abilityID);
        }
    }

    public void _GenerateAbilityByID(int _abilityID)
    {
        GameObject _newAbility = Instantiate(_GetAbilityByID(_abilityID)._prefab, 
                                             transform.position + _GetAbilityByID(_abilityID)._spawnOffset,
                                             Quaternion.identity);
    }

    #endregion
}