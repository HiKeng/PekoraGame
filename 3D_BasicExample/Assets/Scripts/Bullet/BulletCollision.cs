using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Invector.vShooter
{
    using Invector.vEventSystems;

    [RequireComponent(typeof(SphereCollider))]
    public class BulletCollision : vMonoBehaviour
    {
        #region Variables

        [Header("References")]
        [SerializeField] HurtBox _HurtboxPrefab;

        [Header("Parameters")]
        [SerializeField] float _damage;
        [SerializeField] float _hurtBoxDuration = 0.1f;
        [SerializeField] float _knockbackForce = 5f;
        [SerializeField] LayerMask _objectCanHit = new LayerMask();

        [System.Serializable]
        public enum DoKnockBack
        {
            None,
            Weak,
            Strong
        }
        [SerializeField] DoKnockBack _doKnockBack;

        [Header("Events")]
        [SerializeField] UnityEvent _onEnabled;
        [SerializeField] UnityEvent _onHit;

        #endregion

        #region Methods

        private void OnEnable()
        {
            _onEnabled.Invoke();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (IsInLayerMask(other.gameObject, _objectCanHit))
            {
                _GenerateHurtBox(other.gameObject);
                _onHit.Invoke();

                gameObject.SetActive(false);
            }
        }

        private void _GenerateHurtBox(GameObject _hitWithTarget)
        {
            if(_HurtboxPrefab == null) { return; }

            HurtBox _newHurtbox = Instantiate(_HurtboxPrefab, transform.position, Quaternion.identity);
            _SetDamageHurtBox(_newHurtbox);
        }

        public bool IsInLayerMask(GameObject obj, LayerMask layerMask)
        {
            return ((layerMask.value & (1 << obj.layer)) > 0);
        }

        void _SetDamageHurtBox(HurtBox _target)
        {
            _target._hurtBoxSO._damage = _damage;
        }

        #endregion

        #region Generic Functions
        public void _RemoveParentOfOther(Transform _target)
        {
            _target.parent = null;
        }

        public void _SetParentOfOtherToThis(Transform _target)
        {
            _target.parent = transform;
        }

        public void _SetPositionOfOtherToThis(Transform _target)
        {
            _target.transform.position = transform.position;
        }

        #endregion
    }
}

