using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
//using UnityEngine.Experimental.XR

using System;
using UnityEngine.XR.ARSubsystems;

public class ARTapToPlaceObject : MonoBehaviour {
    public GameObject placementIndicator;
    public Camera arCamera;
    public GameObject objectToPlace;
    //private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;

    void Start () {
        //arOrigin = FindObjectOfType<ARSessionOrigin>();
        aRRaycastManager = FindObjectOfType<ARRaycastManager> ();
    }

    void Update () {
        UpdatePlacementPose ();
        UpdatePlacementIndicator ();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {
            PlaceObject ();
        }
    }

    private void PlaceObject () {
        Instantiate (objectToPlace, placementPose.position, placementPose.rotation);
    }

    private void UpdatePlacementIndicator () {
        if (placementPoseIsValid) {
            placementIndicator.SetActive (true);
            placementIndicator.transform.SetPositionAndRotation (placementPose.position, placementPose.rotation);
        } else {
            placementIndicator.SetActive (false);
        }
    }

    private void UpdatePlacementPose () {
        var screenCenter = arCamera.ViewportToScreenPoint (new Vector3 (0.5f, 0.5f));
        var hits = new List<ARRaycastHit> ();
        aRRaycastManager.Raycast (screenCenter, hits, TrackableType.PlaneEstimated);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid) {
            placementPose = hits[0].pose;

            var cameraForward = arCamera.transform.forward;
            var cameraBearing = new Vector3 (cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation (cameraBearing);
        }
    }
}