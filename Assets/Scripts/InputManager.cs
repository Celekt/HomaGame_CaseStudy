using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [HideInInspector]
    public bool blockInputs;

    private float previousScreenXPosition;
    private int currentTouchId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There's 2 InputManagers ingame");
            Destroy(this);
        }
        blockInputs = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (blockInputs)
            return;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            previousScreenXPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition).x;
        }
        if (Input.GetMouseButton(0))
        {
            Transform boardTransform = GameManager.Instance.GetCurrentBoard().transform;

            //get touch space during drag and convert to rotation from sensibility
            float updatedScreenXPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition).x;
            float currentSpacing = updatedScreenXPosition - previousScreenXPosition;
            float convertToZRotation = currentSpacing / BalancingValues.Instance.editorSensibility * BalancingValues.Instance.maxBoardAngle;

            //Convert [0,360] angle to [-180,180] angle
            Vector3 currentRotation = boardTransform.localEulerAngles;
            float updatedZRotation = currentRotation.z - convertToZRotation;
            updatedZRotation = (updatedZRotation > 180) ? updatedZRotation - 360 : updatedZRotation;
            updatedZRotation = Mathf.Clamp(updatedZRotation, -BalancingValues.Instance.maxBoardAngle, BalancingValues.Instance.maxBoardAngle);
            currentRotation.z = updatedZRotation;
            boardTransform.localEulerAngles = currentRotation;

            previousScreenXPosition = updatedScreenXPosition;

            //tiltedValue used for fake physics
            BallsManager.Instance.currentTiltedValue = updatedZRotation / BalancingValues.Instance.maxBoardAngle;
        }
        if (Input.GetMouseButtonUp(0))
        {

        }
#else
        foreach (Touch currentTouch in Input.touches)
        {
            if (Input.touches.Length > 1 && currentTouch.fingerId != currentTouchId)
                continue;
            if (currentTouch.phase == TouchPhase.Began)
            {
                previousScreenXPosition = Camera.main.ScreenToViewportPoint(currentTouch.position).x;
                currentTouchId = currentTouch.fingerId;
            }
            else if (currentTouch.phase == TouchPhase.Moved || currentTouch.phase == TouchPhase.Stationary)
            {
                Transform boardTransform = GameManager.Instance.GetCurrentBoard().transform;
                float updatedScreenXPosition = Camera.main.ScreenToViewportPoint(currentTouch.position).x;
                float currentSpacing = updatedScreenXPosition - previousScreenXPosition;
                float convertToZRotation = currentSpacing / BalancingValues.Instance.smartphoneSensibility * BalancingValues.Instance.maxBoardAngle;

                Vector3 currentRotation = boardTransform.localEulerAngles;
                float updatedZRotation = currentRotation.z - convertToZRotation;
                updatedZRotation = (updatedZRotation > 180) ? updatedZRotation - 360 : updatedZRotation;
                updatedZRotation = Mathf.Clamp(updatedZRotation, -BalancingValues.Instance.maxBoardAngle, BalancingValues.Instance.maxBoardAngle);
                currentRotation.z = updatedZRotation;
                boardTransform.localEulerAngles = currentRotation;

                previousScreenXPosition = updatedScreenXPosition;

                BallsManager.Instance.currentTiltedValue = updatedZRotation / BalancingValues.Instance.maxBoardAngle;
            }
        }
#endif
    }
}
