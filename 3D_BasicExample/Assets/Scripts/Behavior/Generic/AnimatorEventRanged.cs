using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventRanged : StateMachineBehaviour
{
    [SerializeField] Vector2 _triggerTimeBetween;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= _triggerTimeBetween.x && stateInfo.normalizedTime < _triggerTimeBetween.y)
        {
            animator.transform.root.GetComponent<Player_Attack>()._SetHitBoxActive(true);
        }
        else
        {
            animator.transform.root.GetComponent<Player_Attack>()._SetHitBoxActive(false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
