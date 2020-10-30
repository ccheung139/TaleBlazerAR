using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour {
    public Canvas gameCanvas;
    public Canvas inventoryCanvas;
    public Button inventoryButton;
    public Button breadButton;
    public Text breadButtonText;
    public GameObject breadOwnedPrefab;
    public Camera arCamera;
    public Button cancelButton;
    public Text actionText;

    public Button exitInventoryButton;
    public int breadsCollected = 0;

    public bool holdingBread = false;
    public GameObject activeBread;

    private float offset = 0.5f;
    private float timer = 0;
    private float timerTotal = 3.0f;

    void Start () {
        cancelButton.onClick.AddListener (CancelBreadAction);
        inventoryButton.onClick.AddListener (EnableInventory);
        exitInventoryButton.onClick.AddListener (DisableInventory);
        breadButtonText.text = breadsCollected.ToString ();
        breadButton.onClick.AddListener (SpawnBread);
    }

    void Update () {
        if (actionText.text != "") {
            if (timer >= timerTotal) {
                actionText.text = "";
            } else {
                timer += Time.deltaTime * 1.0f;
            }
        }
    }

    private void EnableInventory () {
        gameCanvas.enabled = false;
        inventoryCanvas.enabled = true;
    }

    private void DisableInventory () {
        gameCanvas.enabled = true;
        inventoryCanvas.enabled = false;
    }

    public void AddBread () {
        breadsCollected += 1;
        breadButtonText.text = breadsCollected.ToString ();
    }

    public void SubtractBread () {
        breadsCollected -= 1;
        breadButtonText.text = breadsCollected.ToString ();
    }

    private void SpawnBread () {
        if (breadsCollected == 0) {
            actionText.text = "Collect bread first!";
            DisableInventory ();
            return;
        }
        timer = 0;
        SubtractBread ();
        Vector3 inFront = arCamera.transform.right * 0.5f;
        Quaternion orientation = Quaternion.LookRotation (inFront, Vector3.up);
        GameObject newBread = Instantiate (breadOwnedPrefab,
            arCamera.transform.position + new Vector3 (0, 0.8f, 0), orientation, arCamera.transform);
        newBread.transform.position = (arCamera.transform.position + arCamera.transform.forward * offset) - new Vector3 (0, 0.1f, 0);
        holdingBread = true;
        activeBread = newBread;
        cancelButton.gameObject.SetActive (true);
        DisableInventory ();
    }

    private void CancelBreadAction () {
        cancelButton.gameObject.SetActive (false);
        holdingBread = false;
        Destroy (activeBread);
        AddBread ();
    }
}