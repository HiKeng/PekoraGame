using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] MenuButtonController menuButtonController;
    [SerializeField] Animator animator;
    [SerializeField] AnimatorFunctions animatorFunctions;
    [SerializeField] int thisIndex;

    private void Awake()
    {
        animatorFunctions = GetComponent<AnimatorFunctions>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(menuButtonController.index == thisIndex)
        {
            animator.SetBool("isSelected", true);

            if(Input.GetAxis("Submit") == 1)
            {
                animator.SetBool("isPressed", true);
                _playButtonEvent();
            }
            else if (animator.GetBool("isPressed"))
            {
                animator.SetBool("isPressed", false);
                animatorFunctions.disableOnce = true;
            }
        }
        else
        {
            animator.SetBool("isSelected", false);
        }
    }

    void _playButtonEvent()
    {
        GetComponent<Button>().onClick.Invoke();
    }
}
