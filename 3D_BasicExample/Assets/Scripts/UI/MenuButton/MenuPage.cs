using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPage : MonoBehaviour
{
    [SerializeField] MenuButtonController _menuButtonController;

    [SerializeField] List<GameObject> _buttonList;

    void Awake()
    {
        if(_menuButtonController == null)
        {
            _menuButtonController = transform.root.gameObject.GetComponent<MenuButtonController>();
        }
    }

    private void OnEnable()
    {
        _menuButtonController._SetNewMaxIndex(_buttonList.Count);
    }
}
