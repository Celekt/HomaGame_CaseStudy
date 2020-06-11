using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHandler : MonoBehaviour
{
    [HideInInspector]
    public List<int> usedDuplicatorsId;
    [HideInInspector]
    public float baseScale;

    private Rigidbody ballRB;
    private Collider ballColl;

    void Awake()
    {
        ballRB = GetComponent<Rigidbody>();
        ballColl = GetComponent<Collider>();
        usedDuplicatorsId = new List<int>();
        baseScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetBallState()
    {
        transform.SetParent(null);
        ballRB.velocity = Vector3.zero;
        ballRB.angularVelocity = Vector3.zero;
        transform.localScale = Vector3.one;
        ballColl.enabled = false;
        usedDuplicatorsId.Clear();
    }
}
