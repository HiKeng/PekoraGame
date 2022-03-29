using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityActivation : StateMachineBehaviour
{
    #region Variables

    [Header("Prefab")]
    [SerializeField] List<GameObject> _prefabOnStart;
    [SerializeField] GameObject _abilityPrefab;

    [Header("ID")]
    [SerializeField] int _ID = -1;

    [Header("Parameter")]
    [SerializeField] float _spawnDelay = 0.5f;

    [Header("Offset Spawn")]
    [SerializeField] Vector3 _offsetPosition;

    [System.Serializable]
    public enum _SetState
    {
        _none,
        _onEnter,
        _onExit,
        _onOneState
    }

    [Header("Freeze Position")]
    [SerializeField] bool _isFreezePosition = true;
    [SerializeField] _SetState _setFreezePosition;

    [Header("Cooldown Count")]
    [SerializeField] bool _isStartCountCooldown = false;
    [SerializeField] _SetState _setCooldownCount;

    [Header("Object Pooling")]
    [SerializeField] bool _isUseObjectPooling = false;


    #endregion

    #region Methods

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _ActivateOnState(animator, _SetState._onEnter);
        _ActivateOnOneState(animator, _SetState._onOneState, true);

        ///

        _spawnPrefabOnStart(animator);
        animator.GetComponent<MonoBehaviour>().StartCoroutine(_SpawnAbility(animator));

        ///

        // Spawn Attached VFX
        _spawnAttachedVFX(animator, true);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _ActivateOnState(animator, _SetState._onExit);
        _ActivateOnOneState(animator, _SetState._onOneState, false);

        _spawnAttachedVFX(animator, false);
    }


    void _ActivateOnState(Animator animator, _SetState _state)
    {
        if(_setFreezePosition == _state)
        {
            animator.SetBool("isFreezePosition", _isFreezePosition);
        }

        if(_setCooldownCount == _state)
        {
            animator.GetComponent<Enemy_AbilitiesManager>()._StartCountCooldown(_ID);
        }
    }

    void _ActivateOnOneState(Animator animator, _SetState _state, bool _isActivate)
    {
        if (_setFreezePosition == _state)
        {
            animator.SetBool("isFreezePosition", _isActivate);
        }
    }

    IEnumerator _SpawnAbility(Animator animator)
    {
        yield return new WaitForSeconds(_spawnDelay);

        Vector3 _finalPosition = animator.transform.position + _offsetPosition;

        _SetCaster(animator, Instantiate(_abilityPrefab, _finalPosition, animator.transform.rotation));
    }

    private void _SetCaster(Animator animator, GameObject _newObject)
    {
        animator.GetComponent<Enemy_AbilitiesManager>()._SetCasterToAbility(_newObject, _ID);
    }

    void _spawnPrefabOnStart(Animator animator)
    {
        for (int i = 0; i < _prefabOnStart.Count; i++)
        {
            GameObject _newObject = Instantiate(_prefabOnStart[i], animator.transform.position, Quaternion.identity);
        }
    }

    void _spawnAttachedVFX(Animator animator, bool _isOnEnter)
    {
        if(_ID < 0) { return; }

        if (animator.GetComponent<Enemy_AbilitiesManager>()._GetAbilityByID(_ID)._ActivateMethod != Enemy_AbilitiesManager.AbilityProperties.VFX_ActivateMethod.None
            && animator.GetComponent<Enemy_AbilitiesManager>()._GetAbilityByID(_ID)._AttachedVFX.Count > 0)
        {
            if(animator.GetComponent<Enemy_AbilitiesManager>()._GetAbilityByID(_ID)._ActivateMethod == 
               Enemy_AbilitiesManager.AbilityProperties.VFX_ActivateMethod.OneState)
            {
                _ListGameObjectSetActive(animator.GetComponent<Enemy_AbilitiesManager>()._GetAbilityByID(_ID)._AttachedVFX, _isOnEnter);
            }

            else if (animator.GetComponent<Enemy_AbilitiesManager>()._GetAbilityByID(_ID)._ActivateMethod == 
                     Enemy_AbilitiesManager.AbilityProperties.VFX_ActivateMethod.OverTime)
            {
                if(_isOnEnter)
                {
                    _ListGameObjectSetActive(animator.GetComponent<Enemy_AbilitiesManager>()._GetAbilityByID(_ID)._AttachedVFX, true);
                }
            }
        }
    }

    void _ListGameObjectSetActive(List<GameObject> _targetList, bool _isActive)
    {
        for (int i = 0; i < _targetList.Count; i++)
        {
            if(_targetList[i] != null)
            {
                _targetList[i].SetActive(_isActive);
            }
        }
    }

    #endregion
}
