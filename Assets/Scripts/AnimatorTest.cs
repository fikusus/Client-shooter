using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTest : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < animator.parameterCount; i++)
        {
            if (animator.GetParameter(i).type == AnimatorControllerParameterType.Bool)
            {
                Debug.Log(animator.GetBool(animator.GetParameter(i).name));
            }
            else if (animator.GetParameter(i).type == AnimatorControllerParameterType.Float)
            {
                Debug.Log(animator.GetFloat(animator.GetParameter(i).name));
            }
            else if (animator.GetParameter(i).type == AnimatorControllerParameterType.Int)
            {
                Debug.Log(animator.GetInteger(animator.GetParameter(i).name));
            }
        }
    }
}
