using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrassPlacementScript : MonoBehaviour {
    public Button plantGrassButton;
    public Button donePlantingButton;
    public Button openWaterCanButton;
    public GameObject grassPrefab;
    public Camera arCamera;

    public Image maxGrassImage;
    public Image sheepInBoundsImage;
    public Image mainRoomsImage;

    public Bounds room1Bounds;
    public Bounds room2Bounds;

    private bool isPlanting = false;
    private GameObject currentGrass;

    private int room1Grass = 0;
    private int room2Grass = 0;

    private int maxGrassPerRoom = 5;
    private float maxGrassTimer = 0.0f;
    private float maxGrassMaxTime = 3.0f;

    private float sheepInBoundsTimer = 0.0f;
    private float sheepInBoundsMaxTime = 3.0f;

    private float mainRoomsTimer = 0.0f;
    private float mainRoomsMaxTime = 3.0f;

    void Start () {
        plantGrassButton.onClick.AddListener (PlantGrassPressed);
        donePlantingButton.onClick.AddListener (DonePlantingPressed);
    }

    void Update () {
        if (isPlanting) {
            if (currentGrass.transform.position.y < -0.8f) {
                CheckBounds (currentGrass.transform.position);

                currentGrass.transform.parent = null;
                currentGrass.transform.rotation = Quaternion.Euler (0, currentGrass.transform.rotation.y, 0);
                isPlanting = false;
                plantGrassButton.gameObject.SetActive (true);
                donePlantingButton.gameObject.SetActive (false);
                openWaterCanButton.gameObject.SetActive (true);

                currentGrass.AddComponent<WateredGrassScript> ();
            }
        }
        maxGrassTimer = HandleTimer (maxGrassTimer, maxGrassMaxTime, maxGrassImage);
        sheepInBoundsTimer = HandleTimer (sheepInBoundsTimer, sheepInBoundsMaxTime, sheepInBoundsImage);
        mainRoomsTimer = HandleTimer (mainRoomsTimer, mainRoomsMaxTime, mainRoomsImage);
    }

    private float HandleTimer (float timer, float maxTime, Image imageToHide) {
        if (imageToHide.gameObject.activeSelf) {
            if (timer < maxTime) {
                timer += Time.deltaTime;
            } else {
                imageToHide.gameObject.SetActive (false);
            }
        }
        return timer;
    }

    private void PlantGrassPressed () {
        isPlanting = true;
        plantGrassButton.gameObject.SetActive (false);
        donePlantingButton.gameObject.SetActive (true);
        openWaterCanButton.gameObject.SetActive (false);

        Vector3 position = arCamera.transform.position + (arCamera.transform.forward * 0.9f) - new Vector3 (0, 0.1f, 0);
        currentGrass = Instantiate (grassPrefab, position, Quaternion.identity, arCamera.transform);
    }

    private void DonePlantingPressed () {
        isPlanting = false;
        plantGrassButton.gameObject.SetActive (true);
        donePlantingButton.gameObject.SetActive (false);
        openWaterCanButton.gameObject.SetActive (true);
        Destroy (currentGrass);
    }

    private void CheckBounds (Vector3 position) {
        CheckGrassBounds (position);
        CheckSheepInBounds (position);
    }

    private void CheckGrassBounds (Vector3 position) {
        if (room1Bounds.Contains (position)) {
            if (room1Grass >= maxGrassPerRoom) {
                DonePlantingPressed ();
                maxGrassImage.gameObject.SetActive (true);
                maxGrassTimer = 0;
            } else {
                room1Grass += 1;
            }
        } else if (room2Bounds.Contains (position)) {
            if (room2Grass >= maxGrassPerRoom) {
                DonePlantingPressed ();
                maxGrassImage.gameObject.SetActive (true);
                maxGrassTimer = 0;
            } else {
                room2Grass += 1;
            }
        } else {
            DonePlantingPressed ();
            mainRoomsImage.gameObject.SetActive (true);
            mainRoomsTimer = 0;
        }
    }

    private void CheckSheepInBounds (Vector3 position) {
        GameObject[] allSheep = GameObject.FindGameObjectsWithTag ("Sheep");

        foreach (GameObject sheep in allSheep) {
            if (room1Bounds.Contains (sheep.transform.position) && room1Bounds.Contains (position)) {
                DonePlantingPressed ();
                sheepInBoundsImage.gameObject.SetActive (true);
                sheepInBoundsTimer = 0;
                return;
            }
            if (room2Bounds.Contains (sheep.transform.position) && room2Bounds.Contains (position)) {
                DonePlantingPressed ();
                sheepInBoundsImage.gameObject.SetActive (true);
                sheepInBoundsTimer = 0;
                return;
            }
        }
    }

}