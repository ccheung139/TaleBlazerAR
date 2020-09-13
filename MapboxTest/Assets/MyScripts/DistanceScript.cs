using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DistanceScript : MonoBehaviour {
    [SerializeField]
    private Text text;

    [SerializeField]
    public GameObject pikachu;

    [SerializeField]
    public Shader invisibleShader;

    public StartNavigationScript started;

    public GameObject camera;
    public GameObject ArAlignedMap;

    private List<GameObject> childrens = new List<GameObject> ();
    private List<GameObject> middleRangeBuildings = new List<GameObject> ();
    private List<GameObject> placedBuildings = new List<GameObject> ();

    private float generateAfterSeconds = 10.0f;
    private float placementTimer = 0;

    static System.Random random = new System.Random ();

    void Update () {

        if (!started.hasStarted) return;
        childrens = started.childrens;

        if (childrens.Count > 0) {
            List<float> distances = new List<float> ();
            List<GameObject> nowBuildings = new List<GameObject> ();

            foreach (GameObject myBuilding in childrens) {
                float distance = Vector3.Distance (camera.transform.position, myBuilding.transform.position);
                float fc = (float) Math.Round (distance * 100f) / 100f;
                distances.Add (fc);

                if (fc <= 15f) {
                    nowBuildings.Add (myBuilding);
                }
                middleRangeBuildings = nowBuildings;
            }

            if (placementTimer >= generateAfterSeconds) {
                placementTimer = 0;
                if (middleRangeBuildings.Count > 0) {

                    List<GameObject> availableBuildings = new List<GameObject> ();
                    foreach (GameObject potentialBuilding in middleRangeBuildings) {
                        if (!placedBuildings.Contains (potentialBuilding)) {
                            availableBuildings.Add (potentialBuilding);
                        }
                    }

                    if (availableBuildings.Count > 0) {
                        int index = random.Next (availableBuildings.Count);
                        GameObject randomMiddleBuilding = availableBuildings[index];

                        Renderer renderer = randomMiddleBuilding.transform.GetComponent<Renderer> ();
                        Vector3 sizeVec = renderer.bounds.size;
                        GameObject newPikachu = Instantiate (pikachu, new Vector3 (renderer.bounds.center.x, sizeVec.y,
                            renderer.bounds.center.z), Quaternion.identity) as GameObject;
                        newPikachu.transform.parent = randomMiddleBuilding.transform;

                        placedBuildings.Add (randomMiddleBuilding);
                    }

                }
            } else {
                placementTimer += Time.deltaTime * 1.0f;
            }

            text.text = $"time: {placementTimer}";

        }

    }
}