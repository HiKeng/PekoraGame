using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuButtonController : MonoBehaviour
{
    [Header("UI Index")]
    public int index;
    [SerializeField] int maxIndex;

    [Header("Pages")]
    [SerializeField] List<GameObject> _pageList;

    [Header("Key Down")]
    [SerializeField] bool keyDown;

    [Header("Audio")]
    [HideInInspector] public AudioSource audioSource;

    [Header("Events")]
    [SerializeField] UnityEvent _OnChangeUIChoice;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(Input.GetAxis("Vertical") != 0)
        {
            if(!keyDown)
            {
                if(Input.GetAxis("Vertical") < 0)
                {
                    if(index < maxIndex)
                    {
                        index++;
                    }
                    else
                    {
                        index = 0;
                    }
                }
                else if(Input.GetAxis("Vertical") > 0)
                {
                    if(index > 0)
                    {
                        index--;
                    }
                    else
                    {
                        index = maxIndex;
                    }
                }

                keyDown = true;
                _OnChangeUIChoice.Invoke();
            }
        }
        else
        {
            keyDown = false;
        }
    }

    // To change index when mouse on a button.
    public void _MouseOnChooseThisButton(MenuButton _button)
    {
        index = _button._GetIndex();
    }

    public void _ChangeToOtherPage(GameObject _page)
    {
        for (int i = 0; i < _pageList.Count; i++)
        {
            _pageList[i].SetActive(_pageList[i] == _page ? true : false);
        }

        index = 0;
    }
}
