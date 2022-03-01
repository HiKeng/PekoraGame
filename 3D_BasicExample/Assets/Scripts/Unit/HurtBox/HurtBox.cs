using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HurtBox : MonoBehaviour
{
    #region Variables

    [Header("Properties")]
    public SO_HurtBox _hurtBoxSO;
    [SerializeField] LayerMask _damageLayer = new LayerMask();
    [HideInInspector] public List<GameObject> _alreadyHitList;

    [Header("Debug")]
    [SerializeField] bool _showGizmos = false;

    #endregion

    #region Methods

    private void Update()
    {
        _checkColliderEnable();
    }

    private void OnTriggerEnter(Collider other)
    {
        _checkHitTarget(other);
    }

    private void OnTriggerStay(Collider other)
    {
        _checkHitTarget(other);
    }

    void _checkColliderEnable()
    {
        if(!GetComponent<Collider>().enabled)
        {
            _alreadyHitList.Clear();
        }
    }

    private void _checkHitTarget(Collider other)
    {
        if (other.GetComponent<UnitHealth>() != null)
        {
            Debug.Log("Hit with: " + other.name);

            if (IsInLayerMask(other.gameObject, _damageLayer))
            {
                if (!_alreadyHitList.Contains(other.gameObject))
                {
                    _sentHitToUnit(other);
                }
            }
        }
    }

    private void _sentHitToUnit(Collider other)
    {
        other.GetComponent<UnitHealth>()._GetHit(_hurtBoxSO);
        _hurtBoxSO._Senter = transform.root.gameObject;
        _hurtBoxSO._Receiver = other.gameObject;

        _alreadyHitList.Add(other.gameObject);
    }

    public bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        // Do not change format
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        if (!_showGizmos) { return; }

        Gizmos.color = new Vector4(1f, 0f, 0f, 0.5f);
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawCube(Vector3.zero, GetComponent<BoxCollider>().size);
    }

    #endregion
}
