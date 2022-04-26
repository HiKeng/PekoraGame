using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [Header("UI Index Order")]
    [SerializeField] int thisIndex; // Start from 0

    [Header("References")]
    [SerializeField] MenuButtonController menuButtonController;
    Animator animator;
    AnimatorFunctions animatorFunctions;

    private void Awake()
    {
        if(menuButtonController == null)
        {
            menuButtonController = transform.root.GetComponent<MenuButtonController>();
        }

        animatorFunctions = GetComponent<AnimatorFunctions>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(menuButtonController.index == thisIndex)
        {
            animator.SetBool("isSelected", true);

            if(Input.GetButtonDown("Submit"))
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

    // To change index when mouse on a button.
    public void _MouseOnThisButton()
    {
        menuButtonController._MouseOnChooseThisButton(this);
    }

    public int _GetIndex()
    {
        return thisIndex;
    }
}
