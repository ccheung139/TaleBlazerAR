using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class PikachuScript : MonoBehaviour {
    public GameObject camera;

    void Update () {
        float distance = Vector3.Distance (camera.transform.position, transform.position);
        float fc = (float) Math.Round (distance * 100f) / 100f;
        if (distance >= 7f) {
            transform.localScale = new Vector3 (0, 0, 0);
        } else {
            transform.localScale = new Vector3 (0.04f, 0.04f, 0.04f);
        }

    }
}