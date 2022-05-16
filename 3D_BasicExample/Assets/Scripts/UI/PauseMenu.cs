using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour
{
    public static bool _isPause = false;

    [Header("Events")]
    [SerializeField] UnityEvent _onStartPause;
    [SerializeField] UnityEvent _onEndPause;

    private void Start()
    {
        _isPause = false;
        Time.timeScale = 1;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //GameState currentGameState = GameStateManager.Instance.CurrentGameState;
            //GameState newGameState = currentGameState == GameState.Gameplay ? GameState.Paused : GameState.Gameplay;

            //GameStateManager.Instance.SetState(newGameState);

            if (_isPause)
            {
                _ResumeGame();
            }
            else
            {
                _PauseGame();
            }
        }
    }

    public void _PauseGame()
    {
        Time.timeScale = 0.00001f;
        _isPause = true;
        _onStartPause.Invoke();
    }

    public void _ResumeGame()
    {
        Time.timeScale = 1;
        _isPause = false;
        _onEndPause.Invoke();
    }
}
