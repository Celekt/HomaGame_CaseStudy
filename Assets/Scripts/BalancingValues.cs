using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalancingValues : MonoBehaviour
{
    public static BalancingValues Instance;

    [Header("Duplicator")]
    public float duplicatedBallSpeedRate;
    public float minBallScale;
    public int numberOfBallsToReachMinScale;
    public int overlapingBallsToStopSpawn;
    public List<Color> duplicatorColors;

    [Header("Board")]
    public float maxBoardAngle;
    [Range(0, 0.5f)]
    public float editorSensibility;
    [Range(0, 0.5f)]
    public float smartphoneSensibility;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There's 2 BalancingValues ingame");
            Destroy(this);
        }
    }
}
