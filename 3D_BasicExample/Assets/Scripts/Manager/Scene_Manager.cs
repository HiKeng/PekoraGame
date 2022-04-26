using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Animator _transitionAnimation;

    [Header("Debug")]
    [SerializeField] bool _UseDebugButton = false;
    AsyncOperation _operation;

    private void Awake()
    {
    }

    void Update()
    {
        if(_UseDebugButton)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _RestartScene();
            }
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

    public void _AsynchronouslyLoadScene(string _sceneName)
    {
        StartCoroutine(_LoadAsynchronously(_sceneName));
    }

    IEnumerator _LoadAsynchronously(string _sceneName)
    {
        _operation = SceneManager.LoadSceneAsync(_sceneName);
        _operation.allowSceneActivation = false;

        _transitionAnimation.SetTrigger("FadeIn");

        while (!_operation.isDone)
        {
            float progress = Mathf.Clamp01(_operation.progress / 0.9f);
            Debug.Log(progress);

            if(progress == 1)
            {
                yield return new WaitForSeconds(2f);
                _operation.allowSceneActivation = true;
                _transitionAnimation.SetTrigger("FadeOut");
            }

            yield return null;
        }
    }
}
