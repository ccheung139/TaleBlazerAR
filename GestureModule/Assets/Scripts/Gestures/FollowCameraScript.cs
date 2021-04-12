using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowCameraScript : MonoBehaviour {
    public Camera arCamera;
    public List<Vector3> trail = new List<Vector3> ();
    public Vector3 rightDirection;
    public Vector3 forwardDirection;

    private bool isFollow = false;
    private Vector3 lastPos;
    private float yPos;

    private bool followedOnce = false;

    void Update () {
        if (isFollow) {
            Vector3 currentPos = new Vector3 (arCamera.transform.position.x, yPos, arCamera.transform.position.z);
            float distance = Vector3.Distance (lastPos, currentPos);
            if (distance > 0.3f) {
                trail.Add (currentPos);
                lastPos = currentPos;
            }
        }
    }

    public List<Vector3> GetTrail () {
        return trail;
    }

    public Vector3 GetRight () {
        return rightDirection;
    }

    public Vector3 GetForward () {
        return forwardDirection;
    }

    public void StartFollow () {
        // yPos = initializeAreaScript.GetAreas () [0].transform.position.y;
        isFollow = true;
        lastPos = arCamera.transform.position;
        trail.Add (lastPos);

        rightDirection = arCamera.transform.right;
        rightDirection.y = 0;

        forwardDirection = arCamera.transform.forward;
        forwardDirection.y = 0;
    }

    public void EndFollow () {
        isFollow = false;
        trail = new List<Vector3> ();
    }
}