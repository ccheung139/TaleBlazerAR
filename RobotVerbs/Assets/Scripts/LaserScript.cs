using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour {
    private LineRenderer beam;
    private Vector3 cameraOrigin;
    private Vector3 originOffset;
    private Vector3 endPoint;

    void Start () {
        beam = this.gameObject.AddComponent<LineRenderer> ();
        beam.startWidth = 0.1f;
        beam.endWidth = 0.1f;
    }

    void Update () {
        checkLaser ();

    }

    void checkLaser () {
        cameraOrigin = this.transform.position + this.transform.forward * 1.0f;
        originOffset = cameraOrigin + this.transform.right * 0.25f - new Vector3 (0, 0.3f, 0);
        endPoint = cameraOrigin + this.transform.forward * 2f;

        beam.SetPosition (0, originOffset);
        beam.SetPosition (1, endPoint);
        beam.enabled = true;
        checkHit ();
    }

    void checkHit () {
        RaycastHit hit;

        Vector3 direction = endPoint - originOffset;
        if (Physics.Raycast (originOffset, direction, out hit, Vector3.Distance (endPoint, originOffset))) {
            Debug.Log ("Did Hit");
        }
    }
}