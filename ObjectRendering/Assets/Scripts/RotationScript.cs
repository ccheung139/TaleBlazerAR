using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour {
    public GameObject camera;

    void Update () {
        var n = camera.transform.position - transform.position;
        n.y = 0;
        var newRotation = Quaternion.LookRotation (n) * Quaternion.Euler (0, 0, 0);

        transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, Time.deltaTime * 1f);
    }
}