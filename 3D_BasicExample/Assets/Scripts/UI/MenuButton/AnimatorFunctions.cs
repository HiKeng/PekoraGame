using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorFunctions : MonoBehaviour
{
    [SerializeField] MenuButtonController menuButtonController;
    public bool disableOnce;

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
