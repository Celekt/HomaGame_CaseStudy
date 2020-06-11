using RDG;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//Should have used a common class with DuplicatorHandler
public class ReceiverHandler : MonoBehaviour
{
    [SerializeField]
    private int ballGoal;
    [SerializeField]
    private GameObject infoDisplayPrefab;
    
    private List<GameObject> ballsCaught;
    private GameObject infoDisplay;
    private Coroutine bouncyCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        ballsCaught = new List<GameObject>();
        infoDisplay = Instantiate(infoDisplayPrefab, GetComponentInParent<BoardManager>().boardWorldCanvas.transform);
        infoDisplay.transform.position = transform.TransformPoint(Vector3.up * transform.lossyScale.y * 2);
        infoDisplay.GetComponentInChildren<TextMeshProUGUI>().text = ballGoal.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ball"))
        {
            ballsCaught.Add(other.gameObject);
            if (ballsCaught.Count >= ballGoal)
            {
                //The goal of balls has been reached
                DisableReceiver();
                //Now start coroutine to release all balls
                BallsManager.Instance.UpdatePlayableBalls(ballsCaught.Count);
                StartCoroutine(ReleaseAllStoredBalls());

                Vibration.Vibrate(100, 150, true);
            }
            else
            {
                other.gameObject.SetActive(false);

                //feedbacks
                infoDisplay.GetComponentInChildren<TextMeshProUGUI>().text = (ballGoal - ballsCaught.Count).ToString();
                if (bouncyCoroutine != null)
                    StopCoroutine(bouncyCoroutine);
                bouncyCoroutine = StartCoroutine(BouncyDisplayCoroutine());
                Vibration.Vibrate(50, 50, true);
            }
            BallsManager.Instance.UpdatePlayableBalls(-1);
        }
    }

    private IEnumerator ReleaseAllStoredBalls()
    {
        Transform currentBall;
        foreach(GameObject ball in ballsCaught)
        {
            currentBall = ball.transform;
            currentBall.position = RandomPositionInPolygon(currentBall.localScale.x);
            currentBall.gameObject.SetActive(true);
            yield return null;
        }
        ballsCaught.Clear();
    }

    public Vector3 RandomPositionInPolygon(float ballScale)
    {
        //gets a random point in transform : unpredictable
        return transform.TransformPoint(new Vector3((transform.lossyScale.x - ballScale) * Random.Range(-0.5f, 0.5f), (transform.lossyScale.y - ballScale) * Random.Range(-0.5f, 0.5f), (transform.lossyScale.z - ballScale) * Random.Range(-0.5f, 0.5f)));
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

    private void DisableReceiver()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        infoDisplay.SetActive(false);
    }

    public void ResetReceiver()
    {
        GetComponent<Collider>().enabled = true;
        GetComponent<Renderer>().enabled = true;
        infoDisplay.SetActive(true);
        infoDisplay.GetComponentInChildren<TextMeshProUGUI>().text = ballGoal.ToString();

        foreach(GameObject ballInReceiver in ballsCaught)
        {
            BallsManager.Instance.LoadIntoPool(ballInReceiver);
        }
        ballsCaught.Clear();
    }
}
