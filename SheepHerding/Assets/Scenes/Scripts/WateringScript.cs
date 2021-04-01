using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WateringScript : MonoBehaviour {
    public Button openWaterCanButton;
    public Button closeWaterCanButton;
    public Button plantGrassButton;
    public Camera arCamera;
    public GameObject wateringCan;
    public GameObject waterCylinder;
    public GameObject waterParent;

    void Start () {
        openWaterCanButton.onClick.AddListener (OpenWaterPressed);
        closeWaterCanButton.onClick.AddListener (CloseWaterPressed);
    }

    void Update () {
        if (wateringCan.gameObject.activeSelf) {
            if (arCamera.transform.eulerAngles.x > 45f && arCamera.transform.eulerAngles.x < 90f) {
                UpdateWaterCylinder ();
                waterCylinder.gameObject.SetActive (true);

                GameObject[] allGrass = GameObject.FindGameObjectsWithTag ("Grass");
                if (allGrass.Length > 0) {
                    GameObject closestGrass = FindClosestGrass (allGrass);
                    Vector3 flatGrass = closestGrass.transform.position;
                    flatGrass.y = 0;
                    Vector3 flatCan = wateringCan.transform.position;
                    flatCan.y = 0;

                    if (Vector3.Distance (flatGrass, flatCan) < 0.5f) {
                        closestGrass.GetComponent<WateredGrassScript> ().TryWateringGrass ();
                    }
                }
            } else {
                waterCylinder.gameObject.SetActive (false);
            }
        }
    }

    private void UpdateWaterCylinder () {
        Transform spigot = wateringCan.transform.Find ("Spigot");

        float ySpigot = spigot.position.y;
        float difference = ySpigot + 0.8f; // estimated ground level
        float ySize = waterCylinder.gameObject.GetComponent<Renderer> ().bounds.size.y;

        Vector3 scale = waterCylinder.transform.localScale;
        scale.y = difference * scale.y / ySize;
        waterCylinder.transform.localScale = scale;

        Vector3 newPos = spigot.position;
        newPos.y = spigot.position.y - difference / 2;
        waterCylinder.transform.position = newPos;

        // float ratio = yBounds * 
    }

    private GameObject FindClosestGrass (GameObject[] allGrass) {
        float minDistance = float.PositiveInfinity;
        GameObject minGrass = allGrass[0];
        foreach (GameObject grass in allGrass) {
            float distance = Vector3.Distance (grass.transform.position, wateringCan.transform.position);
            if (distance < minDistance) {
                minGrass = grass;
                minDistance = distance;
            }
        }
        return minGrass;
    }

    private void OpenWaterPressed () {
        openWaterCanButton.gameObject.SetActive (false);
        closeWaterCanButton.gameObject.SetActive (true);
        plantGrassButton.gameObject.SetActive (false);
        wateringCan.gameObject.SetActive (true);
    }

    private void CloseWaterPressed () {
        openWaterCanButton.gameObject.SetActive (true);
        closeWaterCanButton.gameObject.SetActive (false);
        plantGrassButton.gameObject.SetActive (true);
        wateringCan.gameObject.SetActive (false);
    }
}