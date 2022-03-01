using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage_SO_001", menuName = "ScriptableObjects/Generic Damage")]
public class SO_HurtBox : ScriptableObject
{
    public float _damage = 10;

    public GameObject _Senter;
    public GameObject _Receiver;
}
