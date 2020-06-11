using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsManager : MonoBehaviour
{
    public static BallsManager Instance;

    public GameObject ballPrefab;

    [HideInInspector]
    public List<GameObject> ballsList;
    [HideInInspector]
    public float currentTiltedValue;
    [HideInInspector]
    public int currentMaxNumberOfBalls;
    [HideInInspector]
    public float currentBallScale;

    //This is used for object pooling
    [HideInInspector]
    public List<GameObject> sleepingBallsList;

    [SerializeField]
    private float maxHorizontalForce;
    [SerializeField]
    private float maxTorque;

    private int numberOfPlayableBalls;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There's 2 BallsManager ingame");
            Destroy(this);
        }
        ballsList = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentMaxNumberOfBalls = 1;
        numberOfPlayableBalls = 1;
        GameManager.changeBoardEvent += OnChangeBoardEvent;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (GameObject ball in ballsList)
        {
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            //fake physics applied to every ball
            ballRb.AddForce(Vector3.right * maxHorizontalForce * (-currentTiltedValue));
            ballRb.AddTorque(ballRb.transform.forward * maxTorque * currentTiltedValue);
        }
    }

    private void OnChangeBoardEvent(object sender, EventArgs e)
    {
        ResetBallsManager();
    }

    public void ResetBallsManager()
    {
        //reset when board is changed
        currentMaxNumberOfBalls = 1;
        currentTiltedValue = 0;
        currentBallScale = 1;
        numberOfPlayableBalls = 1;
    }

    //This calculate the new scale of the balls
    public void CalculateNewScaleAndMax(int newMultiplicator)
    {
        currentMaxNumberOfBalls *= newMultiplicator;
        int maxedBallsNumber = Mathf.Min(BalancingValues.Instance.numberOfBallsToReachMinScale, currentMaxNumberOfBalls);
        float lerpingValue = (float)(maxedBallsNumber - 1) / (float)(BalancingValues.Instance.numberOfBallsToReachMinScale - 1);
        currentBallScale = Mathf.Lerp(1, BalancingValues.Instance.minBallScale, lerpingValue);
    }

    public void SpawnBall(bool isDuplicator, Transform spawnPolygon, List<int> previousUsedDuplicators)
    {
        //Spawner for duplicator and receivers
        GameObject newBall;
        if (sleepingBallsList.Count == 0)
        {
            newBall = Instantiate(BallsManager.Instance.ballPrefab, Vector3.up * 100, GameManager.Instance.GetCurrentBoard().transform.rotation, GameManager.Instance.GetCurrentBoard().transform);
        }
        else
        {
            newBall = UnloadBallFromPool();
            newBall.transform.SetParent(GameManager.Instance.GetCurrentBoard().transform);
            newBall.SetActive(true);
        }
        BallHandler ballScript = newBall.GetComponent<BallHandler>();
        //We add previously used duplicators to avoid loops
        ballScript.usedDuplicatorsId.AddRange(previousUsedDuplicators);
        newBall.transform.localScale = ballScript.baseScale * currentBallScale * Vector3.one;
        newBall.transform.position = RandomPositionInPolygon(spawnPolygon, newBall.transform.localScale.x);
        newBall.GetComponent<Collider>().enabled = true;
        ballsList.Add(newBall);
    }

    public GameObject SpawnBallAtPosition(Vector3 position)
    {
        GameObject newBall;
        if (sleepingBallsList.Count == 0)
        {
            newBall = Instantiate(BallsManager.Instance.ballPrefab, GameManager.Instance.GetCurrentBoard().transform);
        }
        else
        {
            newBall = UnloadBallFromPool();
            newBall.transform.SetParent(GameManager.Instance.GetCurrentBoard().transform);
            newBall.SetActive(true);
        }
        newBall.transform.position = position;
        newBall.transform.localRotation = Quaternion.identity;
        newBall.transform.localScale = newBall.GetComponent<BallHandler>().baseScale * Vector3.one;
        newBall.GetComponent<Collider>().enabled = true;
        ballsList.Add(newBall);
        return newBall;
    }

    public void LoadIntoPool(GameObject loadedBall)
    {
        //Reset the ball and put it back into the pool
        sleepingBallsList.Add(loadedBall);
        loadedBall.GetComponent<BallHandler>().ResetBallState();
        ballsList.Remove(loadedBall);
        loadedBall.SetActive(false);
    }

    private GameObject UnloadBallFromPool()
    {
        if (sleepingBallsList.Count == 0)
            Debug.LogError("Can't get ball : pool is empty");

        GameObject newBall = sleepingBallsList[0];
        sleepingBallsList.RemoveAt(0);
        return newBall;
    }

    public void UpdatePlayableBalls(int changeValue)
    {
        numberOfPlayableBalls += changeValue;
        if(numberOfPlayableBalls == 0)
        {
            GameManager.Instance.BoardIsOver();
        }
    }

    public Vector3 RandomPositionInPolygon(Transform polygon, float ballScale)
    {
        //gets a random point in transform : too unpredictable
        //return polygon.TransformPoint(new Vector3((polygon.lossyScale.x - ballScale) * UnityEngine.Random.Range(-0.5f, 0.5f), (polygon.lossyScale.y - ballScale) * UnityEngine.Random.Range(-0.5f, 0.5f), (polygon.lossyScale.z - ballScale) * UnityEngine.Random.Range(-0.5f, 0.5f)));
        return polygon.position;
    }
}
