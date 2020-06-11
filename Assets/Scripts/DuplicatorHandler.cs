using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using RDG;

//Should have used a common class with ReceiverHandler
public class DuplicatorHandler : MonoBehaviour
{
    [SerializeField]
    private int multiplicator;
    [SerializeField]
    private GameObject infoDisplayPrefab;

    private int ballsNeededToSpawn;
    private float loopTimer;
    private int overlappingBalls;
    private bool activeSpawningCoroutine;
    private bool firstBall;
    private GameObject infoDisplay;
    private Coroutine bouncyCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        infoDisplay = Instantiate(infoDisplayPrefab, GetComponentInParent<BoardManager>().boardWorldCanvas.transform);
        infoDisplay.transform.position = transform.TransformPoint(Vector3.up * transform.lossyScale.y * 2);
        infoDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "x" + multiplicator.ToString();

        Color randomColor = BalancingValues.Instance.duplicatorColors[Random.Range(0, BalancingValues.Instance.duplicatorColors.Count)];
        infoDisplay.GetComponent<Image>().color = randomColor;
        GetComponent<Renderer>().material.color = randomColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ball"))
        {
            if(!firstBall)
            {
                firstBall = true;
                BallsManager.Instance.CalculateNewScaleAndMax(multiplicator);
                Vibration.Vibrate(50, 50, true);
            }

            BallHandler ballScript = other.GetComponent<BallHandler>();
            //checking if already been through
            if(ballScript.usedDuplicatorsId.Contains(GetInstanceID()))
            {
                overlappingBalls++;
                return;
            }
            
            //inherit used duplicators
            List<int> currentBallDuplicatorList = new List<int>();
            currentBallDuplicatorList.AddRange(ballScript.usedDuplicatorsId);
            currentBallDuplicatorList.Add(GetInstanceID());
            
            //triggered ball back to pool
            BallsManager.Instance.LoadIntoPool(other.gameObject);

            BallsManager.Instance.UpdatePlayableBalls(multiplicator - 1);
            StartCoroutine(SpawningCoroutine(multiplicator, currentBallDuplicatorList));

            if (bouncyCoroutine != null)
                StopCoroutine(bouncyCoroutine);
            bouncyCoroutine = StartCoroutine(BouncyDisplayCoroutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ball")
        {
            overlappingBalls--;
        }
    }

    private IEnumerator SpawningCoroutine(int numberOfBalls, List<int> previousUsedDuplicators)
    {
        //we wait other coroutine to stop
        yield return new WaitUntil(() => !activeSpawningCoroutine);

        //we block other coroutines
        activeSpawningCoroutine = true;

        //looping until all balls are spawned
        int spawnsLeft = numberOfBalls;
        float coroutineTimer = 1/BalancingValues.Instance.duplicatedBallSpeedRate;
        while(numberOfBalls > 0)
        {
            //overlapping too much 
            if (overlappingBalls >= BalancingValues.Instance.overlapingBallsToStopSpawn)
            {
                yield return null;
            }

            if (coroutineTimer < 1/BalancingValues.Instance.duplicatedBallSpeedRate)
            {
                coroutineTimer += Time.deltaTime;
                yield return null;
            }
            else
            {
                BallsManager.Instance.SpawnBall(true, transform, previousUsedDuplicators);
                coroutineTimer = 0;
                numberOfBalls--;
                yield return null;
            }
        }
        
        activeSpawningCoroutine = false;
    }

    private IEnumerator BouncyDisplayCoroutine()
    {
        float startTime = Time.time;
        float halfEffectTime = 0.1f;
        while (Time.time < startTime + halfEffectTime)
        {
            infoDisplay.transform.localScale = (1 + Mathf.Sin((Time.time - startTime) / halfEffectTime) * 0.4f) * Vector3.one;
            yield return null;
        }
        startTime = Time.time;
        while (Time.time < startTime + halfEffectTime)
        {
            infoDisplay.transform.localScale = (1.4f - Mathf.Sin((Time.time - startTime) / halfEffectTime) * 0.4f) * Vector3.one;
            yield return null;
        }
        infoDisplay.transform.localScale = Vector3.one;
        bouncyCoroutine = null;
    }
}
