using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AnimationController : MonoBehaviour
{
    Animator _animator;
    Rigidbody _rigidbody;

    [SerializeField] float _moveAnimationMultiplier = 25;


    [SerializeField] float _velocityUpdateDelayTime = 1;
    float _delayedVelocityUpdate = 0;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Update_Animation();
    }

    private void Update_Animation()
    {
        float currentSpeed = _rigidbody.velocity.magnitude / _moveAnimationMultiplier;
        _animator.SetFloat("VelocityLerp", currentSpeed);

        StartCoroutine(DelayedUpdateVelocity(currentSpeed));
    }

    IEnumerator DelayedUpdateVelocity(float _currentSpeed)
    {
        yield return new WaitForSeconds(_velocityUpdateDelayTime);

        _delayedVelocityUpdate = _currentSpeed;
    }
}
