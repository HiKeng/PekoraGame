using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class BuildingCounter : SingletonBase<BuildingCounter>
{
    #region Variables

    [SerializeField] Transform _buildingGroup;
    [SerializeField] List<BuildingHealth> _buildingList;

    [SerializeField] TextMeshProUGUI _currentBuildingLeftCount;
    [SerializeField] TextMeshProUGUI _totalBuildingCount;
    int _buildingLeftCounter = 0;

    [SerializeField] UnityEvent _onAllBuildingDestoryed;

    #endregion

    #region Methods

    void Start()
    {
        // Set buildings and add it into list
        _GetBuildingListFromGroup();

        // Set how many buildings is in the scene
        _buildingLeftCounter = _buildingList.Count;

        // Update UI value depends on building count
        _SetUpUI();
    }

    public void _RemoveEnemyFromCurrentActiveList(BuildingHealth _enemyToRemove)
    {
        _buildingList.Remove(_enemyToRemove);
        _buildingLeftCounter--;

        _UpdateBuildLeftCount(); // Count eliminated enemy

        if (_buildingList.Count <= 0)
        {
            _onAllBuildingDestoryed.Invoke();
        }
    }

    void _GetBuildingListFromGroup()
    {
        foreach(Transform building in _buildingGroup)
        {
            _buildingList.Add(building.gameObject.GetComponent<BuildingHealth>());
        }
    }

    void _UpdateBuildLeftCount()
    {
        _currentBuildingLeftCount.text = _buildingLeftCounter.ToString();
    }

    void _SetUpUI()
    {
        _totalBuildingCount.text = _buildingList.Count.ToString();
        _currentBuildingLeftCount.text = _buildingLeftCounter.ToString();
    }

    #endregion
}
