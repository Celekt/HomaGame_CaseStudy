using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsHandler : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Ball"))
        {
            BallsManager.Instance.UpdatePlayableBalls(-1);
            BallsManager.Instance.LoadIntoPool(other.gameObject);
        }
    }
}
