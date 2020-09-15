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

    public StartNavigationScript started;
    public NewBuildingScript newBuilding;
    public SearchScript searchScript;

    public GameObject camera;
    public GameObject ArAlignedMap;

    public GameObject chosenBuilding = null;

    private List<GameObject> childrens = new List<GameObject> ();
    private List<GameObject> middleRangeBuildings = new List<GameObject> ();
    private List<GameObject> placedBuildings = new List<GameObject> ();

    static System.Random random = new System.Random ();

    void Start() {
        childrens = started.childrens;
    }

    void Update () {

        if (!started.hasStarted) return;
        

        if (childrens.Count > 0) {
            if (chosenBuilding == null) {
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

                List<GameObject> availableBuildings = new List<GameObject> ();
                foreach (GameObject potentialBuilding in middleRangeBuildings) {
                    if (!placedBuildings.Contains (potentialBuilding)) {
                        availableBuildings.Add (potentialBuilding);
                    }
                }

                if (availableBuildings.Count > 0) {
                    int index = random.Next (availableBuildings.Count);
                    GameObject randomMiddleBuilding = availableBuildings[index];

                    MeshRenderer meshRenderer = randomMiddleBuilding.gameObject.GetComponent<MeshRenderer> ();
                    meshRenderer.enabled = true;

                    foreach (Material material in meshRenderer.materials) {
                        material.color = Color.blue;
                    }
                    randomMiddleBuilding.layer = LayerMask.NameToLayer ("LowerMap");
                    chosenBuilding = randomMiddleBuilding;

                    placedBuildings.Add (chosenBuilding);
                } else {
                    text.text = "No new buildings nearby! Walk to a new area and press 'New'.";
                }

            } else {
                text.text = "Walk towards the blue building on your map!";

                if (newBuilding.changeBuilding) {
                    placedBuildings.Add (chosenBuilding);
                    MeshRenderer meshRenderer = chosenBuilding.gameObject.GetComponent<MeshRenderer> ();
                    foreach (Material material in meshRenderer.materials) {
                        material.color = Color.gray;
                    }
                    meshRenderer.enabled = false;
                    newBuilding.changeBuilding = false;
                    chosenBuilding = null;
                    return;
                }

                if (searchScript.found) {
                    searchScript.found = false;
                    chosenBuilding = null;
                }

                // Renderer renderer = chosenBuilding.transform.GetComponent<Renderer> ();

                // Vector3 groundOfBuilding = new Vector3 (renderer.bounds.center.x, 0, renderer.bounds.center.z);
                // float distance = Vector3.Distance (camera.transform.position, groundOfBuilding);

                // if (distance <= 5f) {
                //     MeshRenderer meshRenderer = chosenBuilding.gameObject.GetComponent<MeshRenderer> ();
                //     foreach (Material material in meshRenderer.materials) {
                //         material.color = Color.red;
                //     }
                //     Vector3 sizeVec = renderer.bounds.size;
                //     Vector3 closestPoint = findClosestEdge (groundOfBuilding, sizeVec, camera.transform.position);
                //     GameObject newPikachu = Instantiate (pikachu, closestPoint, Quaternion.identity) as GameObject;
                //     newPikachu.transform.parent = chosenBuilding.transform;
                //     newPikachu.AddComponent<PikachuScript> ().camera = camera;

                //     chosenBuilding = null;
                // }

            }
        }
    }

    
}