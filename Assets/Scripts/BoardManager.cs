using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Transform topLinker, botLinker;
    public GameObject startingBall;

    [HideInInspector]
    public Transform boardWorldCanvas;

    [SerializeField]
    private Vector3 localCameraPosition;
    [SerializeField]
    private GameObject boardBounds;

    private Vector3 firstBallPosition;

    private void Awake()
    {
        boardWorldCanvas = GetComponentInChildren<Canvas>().transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        //hiding visual help for linking boards / linkers
        topLinker.GetComponent<Renderer>().enabled = false;
        botLinker.GetComponent<Renderer>().enabled = false;

        firstBallPosition = startingBall.transform.position;
    }
    
    public void SetAsNewBoard()
    {
        BallsManager.Instance.ballsList.Add(startingBall);
        startingBall.GetComponent<Rigidbody>().isKinematic = false;
        boardBounds.SetActive(true);
    }

    public void SetAsPreviousBoard()
    {
        boardBounds.SetActive(false);
    }

    public Vector3 GetWorldPositionCamera()
    {
        return transform.TransformPoint(localCameraPosition);
    }

    public void ResetBoard()
    {
        //Reset board rotation
        transform.rotation = Quaternion.Euler(-45, 0, 0);
        startingBall = BallsManager.Instance.SpawnBallAtPosition(firstBallPosition);

        //reset stored balls in receivers
        ReceiverHandler[] receiverList = GetComponentsInChildren<ReceiverHandler>();
        foreach(ReceiverHandler receiver in receiverList)
        {
            receiver.ResetReceiver();
        }
    }
}
