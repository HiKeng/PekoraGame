using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvent : StateMachineBehaviour
{
    [SerializeField] float _triggerTime = 0f;
    bool _isTriggered = false;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _isTriggered = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Set attacking state back to false after reached the desired time.
        if(stateInfo.normalizedTime >= _triggerTime && !_isTriggered)
        {
            animator.transform.root.GetComponent<Player_MovementController>().End_AttackingState();
            _isTriggered = true;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
