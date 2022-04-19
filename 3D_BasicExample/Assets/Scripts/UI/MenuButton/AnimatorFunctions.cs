using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorFunctions : MonoBehaviour
{
    public bool disableOnce;

    [Header("References")]
    MenuButtonController menuButtonController;

    private void Awake()
    {
        menuButtonController = transform.root.GetComponent<MenuButtonController>();
    }

    void PlaySound(AudioClip sound)
    {
        if(!disableOnce)
        {
            menuButtonController.audioSource.PlayOneShot(sound);
        }
        else
        {
            disableOnce = false;
        }
    }
}
