using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static EventHandler<ChangeBoardArg> changeBoardEvent;   

    [SerializeField]
    private List<LevelData> levels;
    [SerializeField]
    private Camera mainGameCamera;
    [SerializeField]
    private float timeToChangeBoard;

    private List<BoardManager> boardsList;
    private int currentBoardNumber;
    private int currentLevelNumber;
    private LevelData currentLevel;
    private int currentScore;

    private const string KEY_CURRENT_LEVEL = "currentLevel";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There's 2 GameManager ingame");
            Destroy(this);
        }
        currentLevelNumber = PlayerPrefs.GetInt(KEY_CURRENT_LEVEL, 0);
        currentLevel = levels[currentLevelNumber];
        currentBoardNumber = 0;
        boardsList = new List<BoardManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadLevel(false);
        GetCurrentBoard().SetAsNewBoard();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public BoardManager GetCurrentBoard()
    {
        return boardsList[currentBoardNumber];
    }

    public void IncrementScore()
    {
        UIManager.Instance.UpdateScore(++currentScore);
    }

    public void BoardIsOver()
    {
        if (currentScore == 0)
        {
            Debug.Log("Board defeat");
            StartCoroutine(ShowReplayUICoroutine());
        }
        else
        {
            Debug.Log("Board victory");
            StartCoroutine(NextBoardCoroutine());
        }
    }

    public void RestartCurrentBoard()
    {
        GetCurrentBoard().ResetBoard();
        UIManager.Instance.pregameUI.SetActive(true);
        InputManager.Instance.blockInputs = false;
    }

    private IEnumerator ShowReplayUICoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        BallsManager.Instance.ResetBallsManager();
        InputManager.Instance.blockInputs = true;
        UIManager.Instance.replayUI.SetActive(true);
    }

    private IEnumerator NextBoardCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        GoToNextBoard();
    }

    public void GoToNextBoard()
    {
        GetCurrentBoard().SetAsPreviousBoard();

        float currentLocalZRotation = GetCurrentBoard().transform.localEulerAngles.z;
        Vector3 currentCameraPosition = mainGameCamera.transform.position;
        if(++currentBoardNumber >= currentLevel.boards.Count)
        {
            //Level completed
            return;
        }
        else if (currentBoardNumber == currentLevel.boards.Count - 1)
        {
            //Boss level
        }
        else
        {

        }
        changeBoardEvent(this, new ChangeBoardArg(currentBoardNumber));
        currentScore = 0;
        GetCurrentBoard().SetAsNewBoard();

        StartCoroutine(NextBoardCoroutine(currentLocalZRotation, currentCameraPosition));
    }

    private IEnumerator NextBoardCoroutine(float previousBoardRotation, Vector3 previousCameraPosition)
    {
        InputManager.Instance.blockInputs = true;

        Vector3 cameraTargetPosition = GetCurrentBoard().GetWorldPositionCamera();
        float targetRotation = (previousBoardRotation < 0) ? -180 : 180;

        float startTime = Time.time;
        float lerpingValue = 0;
        while(lerpingValue <= 1)
        {
            lerpingValue = (Time.time - startTime) / timeToChangeBoard;
            boardsList[currentBoardNumber - 1].transform.localEulerAngles = new Vector3(-45 ,0 ,Mathf.Lerp(previousBoardRotation, targetRotation, lerpingValue));
            mainGameCamera.transform.position = Vector3.Lerp(previousCameraPosition, cameraTargetPosition, lerpingValue);
            yield return null;
        }

        InputManager.Instance.blockInputs = false;
    }

    private void LoadLevel(bool next)
    {
        if (next)
            currentLevel = levels[++currentLevelNumber];

        int boardIndex = 0;
        Vector3 previousBotLinker = Vector3.zero;
        foreach(GameObject board in currentLevel.boards)
        {
            BoardManager currentLoadedBoard = Instantiate(board, Vector3.zero, Quaternion.Euler(-45,0,0)).GetComponent<BoardManager>();
            boardsList.Add(currentLoadedBoard);
            if(boardIndex > 0)
            {
                Vector3 localPositionFromTopLinker = currentLoadedBoard.transform.position - currentLoadedBoard.topLinker.position;
                currentLoadedBoard.transform.position = previousBotLinker + localPositionFromTopLinker;
            }
            else
            {
                mainGameCamera.transform.position = currentLoadedBoard.GetWorldPositionCamera();
            }
            previousBotLinker = currentLoadedBoard.botLinker.position;
         
            currentLoadedBoard.gameObject.SetActive(true);
            boardIndex++;
        }

        UIManager.Instance.SetLevelDisplay(currentLevel.boards.Count, PlayerPrefs.GetInt(KEY_CURRENT_LEVEL, 0) + 1);
    }
}
