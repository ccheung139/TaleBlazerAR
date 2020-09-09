using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerScript : MonoBehaviour {

    [SerializeField]
    private Text text;

    [SerializeField]
    private Text spawnInstructions;

    public GameObject camera;
    public GameObject prefab;
    private Vector3 lastPosition;
    public GameObject spawnPanel;

    private static int count = 1;
    private static DateTime startTime = DateTime.Now;

    void Start () {
        lastPosition = prefab.transform.position;
        text.text = "";
        spawnInstructions.text = "";
        spawnPanel.SetActive (false);
    }

    void Update () {
        DateTime newTime = DateTime.Now;
        TimeSpan difference = newTime - startTime;
        if (difference.Seconds > 1) {
            spawnInstructions.text = "";
            spawnPanel.SetActive (false);
        }

        float distance = Vector3.Distance (camera.transform.position, lastPosition);
        if (distance > 2f) {
            text.text = $"Walk towards the {(count > 1 ? "next " : "")}Pikachu!";
        }

        if (distance <= 2f) {
            GameObject a = Instantiate (prefab) as GameObject;

            Vector3 temp = new Vector3 (0, 0, 3 * count);
            a.transform.position += temp;
            count += 1;
            lastPosition = a.transform.position;

            spawnPanel.SetActive (true);
            spawnInstructions.text = $"Another Pikachu appeared! You've found {count} Pikachu's!";
            startTime = DateTime.Now;
        }
    }
}