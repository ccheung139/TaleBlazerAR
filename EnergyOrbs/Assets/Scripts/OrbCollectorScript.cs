// using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class OrbCollectorScript : MonoBehaviour {
    [SerializeField]
    private Button spawnOrbButton;

    [SerializeField]
    private GameObject orbPrefab;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private Text text;

    [SerializeField]
    private Button collectButton;

    [SerializeField]
    private GameObject panel;

    public static int collectedOrbs = 0;
    private List<GameObject> orbsInPlay = new List<GameObject> ();
    private int count = 0;

    private float generateAfterSeconds = 5.0f;
    private float placementTimer = 0;

    public static bool foundObject = false;
    public static GameObject potentialOrb = null;

    // Start is called before the first frame update
    void Start () {
        panel.SetActive (false);
        spawnOrbButton.onClick.AddListener (SpawnOrbs);
        collectButton.gameObject.SetActive (false);
    }

    // Update is called once per frame
    void Update () {
        if (panel.active) {
            if (placementTimer < generateAfterSeconds) {
                placementTimer += Time.deltaTime * 1.0f;
            } else {
                panel.SetActive (false);
            }
        }
        text.text = "Orbs collected: " + collectedOrbs.ToString ();
    }

    private void SpawnOrbs () {
        Vector3 playerPosition = arCamera.transform.position;

        System.Random rand = new System.Random ();
        float randX = (float) ((rand.NextDouble () * 4) - 2.0);
        float randZ = (float) ((rand.NextDouble () * 4) - 2.0);

        Vector3 newPosition = playerPosition + new Vector3 (randX, 0, randZ);
        GameObject newOrb = Instantiate (orbPrefab, newPosition, Quaternion.identity);
        VisibleScript script = newOrb.AddComponent<VisibleScript> ();

        script.arCamera = arCamera;
        script.text = text;
        script.collectButton = collectButton;

        orbsInPlay.Add (newOrb);

        placementTimer = 0f;
        panel.SetActive (true);
    }
}