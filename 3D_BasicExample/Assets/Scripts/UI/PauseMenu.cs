using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] bool _isPause = false;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(_isPause)
            {
                Time.timeScale = 1;
                _isPause = false;
            }
            else
            {
                Time.timeScale = 0;
                _isPause = true;
            }
        }
    }
}
