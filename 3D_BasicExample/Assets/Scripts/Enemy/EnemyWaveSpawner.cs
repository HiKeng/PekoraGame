using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

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

    int _currentWaveID = 0;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI _eliminatedEnemyCount;
    [SerializeField] TextMeshProUGUI _totalEnemyAmount;
    int _eliminateEnemyCounter = 0;

    [Header("Delay Spawn")]
    [SerializeField] float _delaySpawnForEachEnemy = 1f;

    [Header("VFX")]
    [SerializeField] GameObject _spawnVFX;

    [Header("Events")]
    [SerializeField] UnityEvent _onSpawnWave;
    [SerializeField] UnityEvent _onFinishedAllWave;

    #endregion

    void Start()
    {
        _StartSpawnEnemyWave(_currentWaveID); // Spawn first wave on start.

        _SetUpUI(); // Setup total enemy amount.
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            foreach (EnemyHealth enemy in _currentActiveEnemy)
            {
                enemy._GoDead();
            }
        }
    }

    #region Spawn Methods

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
            // Spawn VFX before actually spawn enemy.
            GameObject _newSpawnVFX = Instantiate(_spawnVFX,
                                               _spawnPointList[_enemyWave[_currentWaveID]._enemyInWave[i]._spawnPointID].position,
                                               transform.rotation);
            _newSpawnVFX.transform.localScale = new Vector3(2, 2, 2);

            // Actually spawn enemy.
            StartCoroutine(_StartSpawnEachEnemy(_enemyWave[_currentWaveID]._enemyInWave[i]._enemyToSpawn.gameObject,
                                                _spawnPointList[_enemyWave[_currentWaveID]._enemyInWave[i]._spawnPointID].position));

        }

        _onSpawnWave.Invoke();
    }

    IEnumerator _StartSpawnEachEnemy(GameObject _enemyToSpawn, Vector3 _spawnPosition)
    {
        yield return new WaitForSeconds(_delaySpawnForEachEnemy);

        GameObject _newEnemy = Instantiate(_enemyToSpawn, _spawnPosition, transform.rotation);
        _currentActiveEnemy.Add(_newEnemy.GetComponent<EnemyHealth>());
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

        _UpdateEliminatedEnemyCount(); // Count eliminated enemy
    }

    int _GetTotalEnemyAmount()
    {
        int _enemyCount = 0;

        for (int i = 0; i < _enemyWave.Count; i++)
        {
            for (int j = 0; j < _enemyWave[i]._enemyInWave.Count; j++)
            {
                _enemyCount++;
            }
        }

        return _enemyCount;
    }

    #endregion

    #region UI Methods

    void _SetUpUI()
    {
        _eliminatedEnemyCount.text = _eliminateEnemyCounter.ToString();
        _totalEnemyAmount.text = _GetTotalEnemyAmount().ToString();
    }

    void _UpdateEliminatedEnemyCount()
    {
        _eliminateEnemyCounter++;
        _eliminatedEnemyCount.text = _eliminateEnemyCounter.ToString();
    }

    #endregion
}
