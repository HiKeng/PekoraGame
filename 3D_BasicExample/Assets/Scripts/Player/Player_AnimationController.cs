using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AnimationController : MonoBehaviour
{
    public Player_MovementController playerController;

    public void Notify_StartAttacking()
    {
        playerController.Start_AttackingState();
    }

    public void Notify_EndAttacking()
    {
        playerController.End_AttackingState();
    }
}
