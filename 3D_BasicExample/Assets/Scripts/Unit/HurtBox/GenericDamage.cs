using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericDamage : MonoBehaviour
{
    [Header("Propertie")]
    public float _damageValue = 10;

    [Header("Unit")]
    [HideInInspector] public GameObject _Senter;
    [HideInInspector] public GameObject _Receiver;
}
