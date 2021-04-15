using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureHandlerScript : MonoBehaviour
{
    public Camera arCamera;
    public ComeHereGestureScript comeHereGestureScript;
    public FollowMeGestureScript followMeGestureScript;
    public StayPutGestureScript stayPutGestureScript;
    public PunchGestureScript punchGestureScript;
    public ThrowGestureScript throwGestureScript;
    public KeyTurnGestureScript keyTurnGestureScript;
    public RaiseGestureScript raiseGestureScript;
    public SlashGestureScript slashGestureScript;
    public WaxOnGestureScript waxOnGestureScript;
    public DigGestureScript digGestureScript;
    public WaveGestureScript waveGestureScript;
    public Text gestureText;

    // public GameObject comeTutorialRobot;
    // public GameObject followTutorialRobot;
    // public GameObject stayTutorialRobot;

    private bool isPressing;

    private List<Vector3> points = new List<Vector3>();
    private List<float> pointsInTime = new List<float>();
    private List<float> shakeTimes = new List<float>();
    private List<float> rotations = new List<float>();
    private List<Vector3> eulerAngles = new List<Vector3>();

    private float timer = 0.0f;
    private float timerTotal = 0.01f;

    private Vector3 forwardVector;
    private Vector3 upVector;
    private Vector3 rightVector;

    void Start()
    {
        Accelerometer.Instance.OnShake += ShakeOccured;
        // comeTutorialButton.onClick.AddListener (delegate { TutorialPressed (comeTutorialRobot); });
        // followTutorialButton.onClick.AddListener (delegate { TutorialPressed (followTutorialRobot); });
        // stayTutorialButton.onClick.AddListener (delegate { TutorialPressed (stayTutorialRobot); });
        // clearTutorialButton.onClick.AddListener (ClearTutorialPressed);
    }

    void Update()
    {
        if (!isPressing)
        {
            return;
        }
        if (timer >= timerTotal)
        {
            points.Add(arCamera.transform.position);
            pointsInTime.Add(Time.time);
            rotations.Add(Vector3.Dot(arCamera.transform.up, Vector3.up));
            eulerAngles.Add(arCamera.transform.eulerAngles);
            timer = 0.0f;

            // Debug.Log(arCamera.transform.eulerAngles);
        }
        else
        {
            timer += Time.deltaTime * 1.0f;
        }
        KeyBoardShakeHelper();
    }

    public void PressedHold()
    {
        isPressing = true;
        forwardVector = arCamera.transform.forward;
        upVector = arCamera.transform.up;
        rightVector = arCamera.transform.right;
    }

    public void ReleasedHold()
    {
        if (points.Count == 0)
        {
            points = new List<Vector3>();
            pointsInTime = new List<float>();
            shakeTimes = new List<float>();
            rotations = new List<float>();
            eulerAngles = new List<Vector3>();
            return;
        }

        isPressing = false;

        bool isComeHere = comeHereGestureScript.CheckComeHere(points, pointsInTime, shakeTimes);
        bool isFollow = followMeGestureScript.CheckFollowMe(points, shakeTimes);
        bool isStay = stayPutGestureScript.CheckStayPut(points, pointsInTime, rotations);
        bool isPunch = punchGestureScript.CheckPunch(points, pointsInTime, forwardVector);
        bool isThrow = throwGestureScript.CheckThrow(points, pointsInTime, upVector, forwardVector);
        bool isKeyTurn = keyTurnGestureScript.CheckKeyTurn(eulerAngles, points);
        bool isRaise = raiseGestureScript.CheckRaise(points, pointsInTime);
        bool isSlash = slashGestureScript.CheckSlash(points, pointsInTime, upVector, forwardVector, rightVector);
        bool isWaxOn = waxOnGestureScript.CheckWaxOn(points, pointsInTime, upVector, rightVector);
        bool isDig = digGestureScript.CheckDig(points, pointsInTime, upVector, rightVector);
        bool isWave = waveGestureScript.CheckWave(eulerAngles, points, pointsInTime);

        if (isPunch)
        {
            gestureText.text = "Punched";
        }
        else if (isComeHere)
        {
            gestureText.text = "Come Here";
        }
        else if (isFollow)
        {
            gestureText.text = "Follow Me";
        }
        else if (isStay)
        {
            gestureText.text = "Stay Here";
        }
        else if (isThrow)
        {
            gestureText.text = "Threw";
        }
        else if (isKeyTurn)
        {
            gestureText.text = "Key Turned";
        }
        else if (isWaxOn)
        {
            gestureText.text = "Wax On";
        }
        else if (isSlash)
        {
            gestureText.text = "Slashed";
        }
        else if (isRaise)
        {
            gestureText.text = "Raised";
        }
        else if (isDig)
        {
            gestureText.text = "Dug";
        } else if (isWave) {
            gestureText.text = "Waved";
        }


        points = new List<Vector3>();
        pointsInTime = new List<float>();
        shakeTimes = new List<float>();
        rotations = new List<float>();
        eulerAngles = new List<Vector3>();
        // gameObject.SetActive (false);
    }

    private void ShakeOccured()
    {
        if (isPressing)
        {
            shakeTimes.Add(Time.time);
        }
    }

    // private void TutorialPressed (GameObject tutorialRobot) {
    //     Vector3 positionInFront = arCamera.transform.position + arCamera.transform.forward * 1.2f - new Vector3 (0, .3f, 0);
    //     Vector3 robotDifference = positionInFront - tutorialRobot.transform.position;

    //     tutorialRobot.transform.position += robotDifference;

    //     Vector3 flatToCamera = arCamera.transform.position;
    //     flatToCamera.y = tutorialRobot.transform.position.y;
    //     Quaternion direction = Quaternion.LookRotation (flatToCamera - tutorialRobot.transform.position);
    //     tutorialRobot.transform.rotation = direction;
    //     tutorialRobot.SetActive (true);
    // }

    // private void ClearTutorialPressed () {
    //     comeTutorialRobot.SetActive (false);
    //     followTutorialRobot.SetActive (false);
    //     stayTutorialRobot.SetActive (false);
    // }

    private void KeyBoardShakeHelper()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown ("0")) {
            ShakeOccured ();
        }
#endif
    }
}