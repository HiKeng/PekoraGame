using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            _RestartScene();
        }
    }

    public void _RestartSceneWithDelay(float _delayTime)
    {
        StartCoroutine(_StartRestartSceneWithDelay(_delayTime));
    }

    IEnumerator _StartRestartSceneWithDelay(float WaitTime)
    {
        yield return new WaitForSeconds(WaitTime);

        _RestartScene();
    }

    public void _RestartScene()
    {
        SceneManager.LoadScene(Application.loadedLevel);
    }

    public void _LoadSceneWithName(string _scene)
    {
        SceneManager.LoadScene(_scene);
    }

    public void _ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }
}
