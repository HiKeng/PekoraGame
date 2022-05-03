using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetHeadBone : MonoBehaviour
{
    [SerializeField] Transform _headBone;

    void Start()
    {
        _headBone = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
    }

    void Update()
    {
        
    }
}
