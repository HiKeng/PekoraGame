using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MenuButton))]
public class AnimatorFunctions : MonoBehaviour
{
    public bool disableOnce;

    [Header("References")]
    [SerializeField] MenuButtonController menuButtonController;

    private void Awake()
    {
        if (menuButtonController == null)
        {
            menuButtonController = transform.root.gameObject.GetComponent<MenuButtonController>();
        }
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
