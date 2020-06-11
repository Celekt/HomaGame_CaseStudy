using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Level display")]
    [SerializeField]
    private float partOfScreenBetweenLevel;
    [SerializeField]
    private Sprite square;
    [SerializeField]
    private Sprite circle;
    [SerializeField]
    private Color completedColor;
    [SerializeField]
    private RectTransform levelDisplayParent;
    [SerializeField]
    private GameObject imagePrefab;
    [SerializeField]
    private GameObject startPointPrefab;
    [SerializeField]
    private GameObject bossPointPrefab;

    [Header("Score")]
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private GameObject plusOnePrefab;

    [Header("Menus")]
    public GameObject pregameUI;
    public GameObject replayUI;

    private List<Image> levelImages;
    private List<Image> linkImages;
    //private int currentScore;
    private List<GameObject> pooledPlusOne;
    private Coroutine scoreBouncyCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There's 2 UIManager ingame");
            Destroy(this);
        }
        levelImages = new List<Image>();
        linkImages = new List<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        pooledPlusOne = new List<GameObject>();
        GameManager.changeBoardEvent += OnChangeBoardEvent;
        ResetScore();
    }

    private void OnChangeBoardEvent(object sender, ChangeBoardArg e)
    {
        UpdateLevelDisplay(e.boardNumber);
        ResetScore();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevelDisplay(int numberOfLevels, int levelIndex)
    {
        levelImages.Clear();
        linkImages.Clear();
        levelDisplayParent.anchorMin = new Vector2(0.5f - ((float)numberOfLevels - 1f)/2f * partOfScreenBetweenLevel, levelDisplayParent.anchorMin.y);
        levelDisplayParent.anchorMax = new Vector2(0.5f + ((float)numberOfLevels - 1f)/2f * partOfScreenBetweenLevel, levelDisplayParent.anchorMax.y);


        for (int i = 0; i < numberOfLevels; i++)
        {
            Image currentCircle;
            if (i == 0)
            {
                currentCircle = Instantiate(startPointPrefab, levelDisplayParent).GetComponent<Image>();
                currentCircle.color = completedColor;
                currentCircle.GetComponentInChildren<Text>().text = levelIndex.ToString();
            }
            else if (i == numberOfLevels - 1)
            {
                currentCircle = Instantiate(bossPointPrefab, levelDisplayParent).GetComponent<Image>();
            }
            else
            {
                currentCircle = Instantiate(imagePrefab, levelDisplayParent).GetComponent<Image>();
                currentCircle.sprite = circle;
            }
            levelImages.Add(currentCircle);
            RectTransform currentRect = currentCircle.rectTransform;

            if(i == 0)
            {
                currentRect.anchorMin = new Vector2(-1 + (float)i / (numberOfLevels-1), 0.2f);
                currentRect.anchorMax = new Vector2(1 + (float)i / (numberOfLevels - 1), 0.8f);
            }
            else if (i == numberOfLevels - 1)
            {
                currentRect.anchorMin = new Vector2(-1 + (float)i / (numberOfLevels - 1), 0);
                currentRect.anchorMax = new Vector2(1 + (float)i / (numberOfLevels - 1),1);
            }
            else 
            {
                currentRect.anchorMin = new Vector2(-1 + (float)i / (numberOfLevels - 1), 0.5f - 0.15f);
                currentRect.anchorMax = new Vector2(1 + (float)i / (numberOfLevels - 1), 0.5f + 0.15f);
            }
            currentRect.offsetMax = Vector2.zero;
            currentRect.offsetMin = Vector2.zero;

            
            if(i != numberOfLevels - 1)
            {
                Image currentLink = Instantiate(imagePrefab, levelDisplayParent).GetComponent<Image>();
                currentLink.preserveAspect = false;
                currentLink.sprite = square;
                linkImages.Add(currentLink);
                currentRect = currentLink.rectTransform;
                currentRect.anchorMin = new Vector2((float)i / (numberOfLevels - 1), 0.5f - 0.066f);
                currentRect.anchorMax = new Vector2((float)(i + 1) / (numberOfLevels - 1), 0.5f + 0.066f);
                currentRect.offsetMax = Vector2.zero;
                currentRect.offsetMin = Vector2.zero;
                currentRect.SetAsFirstSibling();
            }
        }        
    }

    public void UpdateScore(int newScore)
    {
        scoreText.text = newScore.ToString();
        if (scoreBouncyCoroutine != null)
            StopCoroutine(scoreBouncyCoroutine);
        scoreBouncyCoroutine = StartCoroutine(BouncyDisplayCoroutine());
    }

    public void ResetScore()
    {
        scoreText.text = "0";
    }

    public void InstantiatePlusOne(Vector3 spawnPosition)
    {
        if(pooledPlusOne.Count == 0)
        {
            GameObject newPlusOne = Instantiate(plusOnePrefab, GameManager.Instance.GetCurrentBoard().boardWorldCanvas.transform);
            newPlusOne.transform.position = spawnPosition;
        }
        else
        {
            pooledPlusOne[0].transform.SetParent(GameManager.Instance.GetCurrentBoard().boardWorldCanvas.transform);
            pooledPlusOne[0].SetActive(true);            
            pooledPlusOne[0].transform.position = spawnPosition;
            pooledPlusOne[0].transform.localRotation = Quaternion.identity;
            pooledPlusOne.RemoveAt(0);
        }
    }

    public void LoadIntoPool(GameObject plusOne)
    {
        pooledPlusOne.Add(plusOne);
        plusOne.SetActive(false);
    }

    private IEnumerator BouncyDisplayCoroutine()
    {
        float startTime = Time.time;
        float halfEffectTime = 0.1f;
        while (Time.time < startTime + halfEffectTime)
        {
            scoreText.transform.localScale = (1 + Mathf.Sin((Time.time - startTime) / halfEffectTime) * 0.4f) * Vector3.one;
            yield return null;
        }
        startTime = Time.time;
        while (Time.time < startTime + halfEffectTime)
        {
            scoreText.transform.localScale = (1.4f - Mathf.Sin((Time.time - startTime) / halfEffectTime) * 0.4f) * Vector3.one;
            yield return null;
        }
        scoreText.transform.localScale = Vector3.one;
        scoreBouncyCoroutine = null;
    }

    public void UpdateLevelDisplay(int boardNumber)
    {
        levelImages[boardNumber].color = completedColor;
        linkImages[boardNumber - 1].color = completedColor;
    }

    public void StartGameOnTouch(GameObject uiToHide)
    {
        uiToHide.SetActive(false);
        InputManager.Instance.blockInputs = false;
    }
}
