using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent (typeof (ARRaycastManager))]
public class SpawnZombies : MonoBehaviour {
    [SerializeField]
    private GameObject zombiePrefab;

    [SerializeField]
    private GameObject zombieBulb;
    
    [SerializeField]
    private GameObject orangeBulb;

    [SerializeField]
    private GameObject yellowBulb;

    [SerializeField]
    private GameObject wallPrefab;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private Camera zombieCamera;

    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private GameObject gameWonPanel;

    [SerializeField]
    private GameObject outOfWallsPanel;

    [SerializeField]
    private Text wallText;

    [SerializeField]
    private Text healthText;

    [SerializeField]
    private Material blackMat;

    [SerializeField]
    private Material redMat;

    [SerializeField]
    private Button retrieveWallButton;

    [SerializeField]
    private EventSystem eventSystem;

    [SerializeField]
    private GameObject trophyPrefab;

    private int totalZombies = 5;
    private int numWalls = 5;
    private int healthPoints = 100;
    private GameObject selectedWall = null;

    [Header ("For detecting button press")]
    private ARRaycastManager arRaycastManager;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit> ();

    void Start () {
        healthText.text = "Health: " + healthPoints;
        outOfWallsPanel.SetActive (false);
        wallText.text = "Walls left: " + numWalls;
        zombieCamera.enabled = false;
        gameOverPanel.SetActive (false);
        gameWonPanel.SetActive (false);
        retrieveWallButton.onClick.AddListener (RetrieveWall);
        arRaycastManager = GetComponent<ARRaycastManager> ();
        Invoke ("StartZombieRun", 3);
    }

    private void StartZombieRun () {
        System.Random rand = new System.Random ();
        Vector3 cumulativePositions = new Vector3 (0, 0, 0);
        for (int i = 0; i < totalZombies; i++) {
            Vector3 zombiePosition = SpawnNewZombie (rand);
            cumulativePositions += zombiePosition;
        }
        Vector3 trophyPosition = (cumulativePositions / (float) totalZombies) + new Vector3 (0, 0, 10);
        GameObject trophy = Instantiate (trophyPrefab, trophyPosition, Quaternion.identity);
        TrophyScript script = trophy.AddComponent<TrophyScript>();
        script.arCamera = arCamera;
        script.gameOverPanel = gameOverPanel;
        script.gameWonPanel = gameWonPanel;
    }

    private Vector3 SpawnNewZombie (System.Random rand) {
        Vector3 playerPosition = arCamera.transform.position;

        float randX = (float) (rand.NextDouble () * 7);
        float randZ = (float) ((rand.NextDouble () * 10) + 2);
        Vector3 newPosition = playerPosition + new Vector3 (rand.Next (2) == 1 ? randX : -randX, -1.5f, randZ);

        GameObject newZombie = Instantiate (zombiePrefab, newPosition, Quaternion.identity);
        GameObject bulb1 = Instantiate (zombieBulb, newPosition + new Vector3 (0, 2.1f, 0), Quaternion.identity, newZombie.transform);
        GameObject bulb2 = Instantiate (orangeBulb, newPosition + new Vector3 (0, 2.1f, 0), Quaternion.identity, newZombie.transform);
        GameObject bulb3 = Instantiate (yellowBulb, newPosition + new Vector3 (0, 2.1f, 0), Quaternion.identity, newZombie.transform);
        bulb1.SetActive (false);
        bulb2.SetActive (false);
        bulb3.SetActive (false);
        ZombieMovement script = newZombie.AddComponent<ZombieMovement> ();
        Camera camera = Instantiate (zombieCamera, newPosition + new Vector3 (0, 1.8f, 0), newZombie.transform.rotation, newZombie.transform);

        script.rand = rand;
        script.arCamera = arCamera;
        script.zombieBulb = bulb1;
        script.zombieCamera = camera;
        script.gameOverPanel = gameOverPanel;
        script.healthText = healthText;
        script.gameWonPanel = gameWonPanel;
        script.orangeBulb = bulb2;
        script.yellowBulb = bulb3;
        return newPosition;
    }

    void Update () {
        if (Input.touchCount == 0) {
            return;
        }
        Touch touch = Input.GetTouch (0);

        if (EventSystem.current.currentSelectedGameObject != null || touch.phase != TouchPhase.Began) {
            return;
        }

        HandleTouch (touch);
    }

    private void HandleTouch (Touch touch) {
        Ray ray = arCamera.ScreenPointToRay (touch.position);
        RaycastHit hitObject;
        if (Physics.Raycast (ray, out hitObject)) {
            GameObject obj = hitObject.transform.gameObject;
            if (obj.transform.childCount == 0) {
                SelectWall (obj);
            }
        } else {
            if (selectedWall != null) {
                selectedWall.GetComponent<Renderer> ().sharedMaterial = blackMat;
                selectedWall = null;
            }

            PlaceWall (touch);
        }
    }

    private void PlaceWall (Touch touch) {
        if (arRaycastManager.Raycast (touch.position, hits, TrackableType.FeaturePoint)) {
            if (numWalls > 0) {
                Pose hitPose = hits[0].pose;

                float distance = Vector3.Distance (arCamera.transform.position, hitPose.position);
                if (distance < 0.5f) {
                    Debug.Log ("Too close!");
                    return;
                }

                GameObject wall = Instantiate (wallPrefab, hitPose.position, Quaternion.identity);
                RotateWall (wall);
                SpikyWall script = wall.AddComponent<SpikyWall> ();
                script.arCamera = arCamera;
                script.healthText = healthText;
                script.gameOverPanel = gameOverPanel;
                script.gameWonPanel = gameWonPanel;

                numWalls -= 1;
                wallText.text = "Walls left: " + numWalls;
            } else {
                outOfWallsPanel.SetActive (true);
            }
        }
    }

    private void SelectWall (GameObject obj) {
        if (selectedWall != null) {
            selectedWall.GetComponent<Renderer> ().sharedMaterial = blackMat;
        }

        Renderer renderer = obj.GetComponent<Renderer> ();
        if (obj.GetComponent<Renderer> ().sharedMaterial == blackMat) {
            obj.GetComponent<Renderer> ().sharedMaterial = redMat;
            selectedWall = obj;
        } else {
            obj.GetComponent<Renderer> ().sharedMaterial = blackMat;
            selectedWall = null;
        }
    }

    private void RotateWall (GameObject wall) {
        var n = arCamera.transform.position - wall.transform.position;
        n.y = 0;
        wall.transform.rotation = Quaternion.LookRotation (n);
    }

    private void RetrieveWall () {
        if (selectedWall == null) {
            return;
        }
        Destroy (selectedWall);
        selectedWall = null;
        numWalls += 1;
        wallText.text = "Walls left: " + numWalls;
    }
}