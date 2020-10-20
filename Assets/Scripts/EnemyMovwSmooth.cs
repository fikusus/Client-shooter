using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovwSmooth : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 10;
    [SerializeField]
    private float positionSpeed = 6;
    public Vector3 targetPosition = Vector3.zero;
    public Vector3 targetRoration = Vector3.zero;

    private Vector3 from;
    private Vector3 fromPos;
    float t = 0f;
    void Start()
    {
        fromPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
         float koef = (Vector3.Distance(transform.position, targetPosition) <= 1) ? (Vector3.Distance(transform.position, targetPosition) + 0.2f): Vector3.Distance(transform.position, targetPosition);
        //float koef = Vector3.Distance(transform.position, targetPosition);
        float rotationStep = rotationSpeed * Time.deltaTime; // calculate distance to move
        float positionStep = Mathf.Pow(positionSpeed, koef) * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, positionStep);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRoration), rotationStep * 90);
    }



    public void setTargetPosition(Vector3 newTarget)
    {
        targetPosition = newTarget;
    }


    public void setTargetRotation(Vector3 newTarget)
    {
        from = targetRoration;
        targetRoration = newTarget;

    }

}
