using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimator : MonoBehaviour
{
    public Quaternion target = Quaternion.identity;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnAnimatorIK(int layerIndex)
    {
        Debug.Log(target);
       this.GetComponent<Animator>().SetBoneLocalRotation(HumanBodyBones.Chest, target);
    }
}
