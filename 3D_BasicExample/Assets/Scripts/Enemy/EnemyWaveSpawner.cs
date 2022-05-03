using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyWaveSpawner : SingletonBase<EnemyWaveSpawner>
{
    #region Setup Classes

    [System.Serializable]
    public class EnemyInWaveList
    {
        public string _waveName = "Wave Name";
        public float _delaySpawnTime = 0f;
        public List<EnemyToSpawn> _enemyInWave;
    }

    [System.Serializable]
    public class EnemyToSpawn
    {
        public EnemyHealth _enemyToSpawn;
        public int _spawnPointID;
    }

    #endregion

    #region Variables

    [Header("Parameters")]
    [SerializeField] List<EnemyInWaveList> _enemyWave;
    [SerializeField] List<Transform> _spawnPointList;

    [SerializeField] List<EnemyHealth> _currentActiveEnemy;

    [SerializeField] int _currentWaveID = 0;

    [Header("Events")]
    [SerializeField] UnityEvent _onSpawnWave;
    [SerializeField] UnityEvent _onFinishedAllWave;

    #endregion

    #region Methods

    void Start()
    {
        _StartSpawnEnemyWave(_currentWaveID); // Spawn first wave on start.
    }

    void _SpawnNextEnemyWave()
    {
        _currentWaveID++;
        _StartSpawnEnemyWave(_currentWaveID);
    }

    void _StartSpawnEnemyWave(int _enemyWaveID)
    {
        StartCoroutine(_StartCountSpawnEnemyWave(_enemyWave[_currentWaveID]._delaySpawnTime));
    }

    IEnumerator _StartCountSpawnEnemyWave(float WaitTime)
    {
        yield return new WaitForSeconds(WaitTime);
        HandleSpawnNewEnemyWave();
    }

    void HandleSpawnNewEnemyWave()
    {
        for (int i = 0; i < _enemyWave[_currentWaveID]._enemyInWave.Count; i++)
        {
            GameObject _newEnemy = Instantiate(_enemyWave[_currentWaveID]._enemyInWave[i]._enemyToSpawn.gameObject,
                                               _spawnPointList[_enemyWave[_currentWaveID]._enemyInWave[i]._spawnPointID].position,
                                               transform.rotation);

            _currentActiveEnemy.Add(_newEnemy.GetComponent<EnemyHealth>());
        }

        _onSpawnWave.Invoke();
    }

    public void _RemoveEnemyFromCurrentActiveList(EnemyHealth _enemyToRemove)
    {
        // Check the enemy target to remove from current active list of enemy. ---------------------------------------------
        for (int i = 0; i < _currentActiveEnemy.Count; i++)
        {
            if (_currentActiveEnemy[i] == _enemyToRemove)
            {
                _currentActiveEnemy.Remove(_currentActiveEnemy[i]);
            }
        }
        // -----------------------------------------------------------------------------------------------------------------


        // If all enemy in current were eliminated, spawn next wave of enemy. ----------------------------------------------
        if (_currentActiveEnemy.Count == 0)
        {
            // Check if the finished wave is the last wave.
            // If it is not the last wave, then spawn the next wave.
            // If it is the last wave, the finish all wave will be invoked.
            if (_currentWaveID < _enemyWave.Count - 1)
            {
                _SpawnNextEnemyWave();
            }
            else
            {
                _onFinishedAllWave.Invoke();
            }
        }
        // -----------------------------------------------------------------------------------------------------------------
    }

    #endregion
}
