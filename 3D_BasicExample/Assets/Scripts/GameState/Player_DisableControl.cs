using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_DisableControl : MonoBehaviour
{
    [SerializeField] bool _isActive = true;

    PlayerHealth _playerHealth;
    Player_Camera _playerCamera;
    Player_MovementController _playerMovement;
    Player_Attack _playerAttack;

    void Awake()
    {
        _playerHealth = GetComponent<PlayerHealth>();
        _playerCamera = GetComponent<Player_Camera>();
        _playerMovement = GetComponent<Player_MovementController>();
        _playerAttack = GetComponent<Player_Attack>();

        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void Update()
    {
        _SetDisable(!PauseMenu._isPause);
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    void _SetDisable(bool _state)
    {
        _isActive = _state;

        _playerHealth.enabled = _isActive;
        _playerCamera.enabled = _isActive;
        _playerMovement.enabled = _isActive;
        _playerAttack.enabled = _isActive;
    }

    void OnGameStateChanged(GameState newGameState)
    {
        _SetDisable(newGameState == GameState.Gameplay);
    }
}
