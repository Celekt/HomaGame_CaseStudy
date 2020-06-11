using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunnelHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ball"))
        {
            BallsManager.Instance.LoadIntoPool(other.gameObject);
            BallsManager.Instance.UpdatePlayableBalls(-1);
            GameManager.Instance.IncrementScore();
            UIManager.Instance.InstantiatePlusOne(other.transform.position + transform.up - transform.forward);
        }
    }
}
