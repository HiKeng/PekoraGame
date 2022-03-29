using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class Bullet_Behavior : MonoBehaviour
{
    #region Variables

    enum State
    {
        No_Movement,
        Straight_Line,
        Aim_At_Player,
        LockOnPlayer
    }

    [SerializeField] State _bulletBehaviorType;
    [SerializeField] Vector3 _aimPlayerOffset;

    Transform _playerPosition;
    Vector3 _difference;
    float _rotationZ;
    Rigidbody _rigidBody;

    [SerializeField] float _force = 200;


    //public int _bulletBehaviorIndex;

    #endregion

    #region Awake

    void Awake()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    #endregion

    #region Methods

    public void Start()
    {
        bulletMode();
    }

    private void Update()
    {
        _playerPosition = Unit_Manager.Instance._player.transform;

        if (Input.GetKeyDown(KeyCode.P))
        {
            transform.position = _playerPosition.gameObject.transform.position;
            Debug.Log("Player = " + _playerPosition.gameObject);
        }
    }

    void FixedUpdate()
    {
        if(_bulletBehaviorType == State.No_Movement) { return; }

        _rigidBody.AddForce(transform.forward * _force);


        if (_bulletBehaviorType == State.LockOnPlayer)
        {
            _rigidBody.velocity = Vector3.zero;

            targetAtPlayer();

            Vector3 _finalAimPosition = _playerPosition.position + _aimPlayerOffset;

            Vector3 _finalDirection = _finalAimPosition - transform.position;

            _rigidBody.AddForce(_finalDirection.normalized * _force);

            //_rigidBody.AddForce(transform.forward * 2);
        }
    }

    void bulletMoving()
    {
        //_rigidBody.AddForce(transform.forward * _bullet._bulletProperties._bulletSpeed);
    }

    void bulletMode()
    {
        if(_bulletBehaviorType == State.Aim_At_Player)
        {
            targetAtPlayer();
        }
    }

    private void OnEnable()
    {
        //if (_bulletBehaviorType == State.Aim_At_Player)
        //{
        //    targetAtPlayer();
        //}
    }

    void targetAtPlayer()
    {
        //Vector3 _finalAimPosition = Unit_Manager.Instance._player.transform.position;
        //Vector3 _finalAimPosition = _playerPosition.gameObject.transform.position + _aimPlayerOffset;
        Vector3 _finalAimPosition = GameObject.FindGameObjectWithTag("Player").transform.position + _aimPlayerOffset;
        //Vector3 _finalAimPosition = _aimPlayerOffset;
        transform.LookAt(_finalAimPosition);
    }

    void updateBulletBehaviorState(int _behaviorIndex)
    {
        if(_behaviorIndex == 1)
        {
            _bulletBehaviorType = State.Straight_Line;
        }
        else if(_behaviorIndex == 2)
        {
            _bulletBehaviorType = State.Aim_At_Player;
        }
    }

    private void OnDisable()
    {
        _rigidBody.velocity = Vector3.zero;
    }

    #endregion

}
