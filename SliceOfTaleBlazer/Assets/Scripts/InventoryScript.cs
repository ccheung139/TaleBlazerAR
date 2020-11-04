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
    public Text toastText;
    public Text resultText;
    public GameObject breadInHand;
    public Camera arCamera;
    public Button cancelButton;
    public Text actionText;
    public GameObject magnet;
    public Button magnetButton;
    public Button toastButton;
    public GameObject toastInHand;
    public Button holdButton;
    public Button houseButton;
    public GameObject houseInHand;
    public HouseScript houseScript;

    public Button exitInventoryButton;
    public int toastsCollected = 0;
    public int breadsCollected = 0;

    private float timer = 0;
    private float timerTotal = 3.0f;
    private float resultTimer = 0;
    private float resultTimerTotal = 3.0f;

    void Start () {
        cancelButton.onClick.AddListener (DisableActions);
        inventoryButton.onClick.AddListener (EnableInventory);
        exitInventoryButton.onClick.AddListener (DisableInventory);
        breadButtonText.text = breadsCollected.ToString ();
        toastText.text = toastsCollected.ToString ();
        breadButton.onClick.AddListener (SpawnBread);
        magnetButton.onClick.AddListener (EnableMagnet);
        toastButton.onClick.AddListener (EnableToast);
        houseButton.onClick.AddListener (EnableHouse);
    }

    void Update () {
        if (actionText.text != "") {
            if (timer >= timerTotal) {
                actionText.text = "";
            } else {
                timer += Time.deltaTime * 1.0f;
            }
        }
        if (resultText.text != "") {
            if (resultTimer >= resultTimerTotal) {
                resultText.text = "";
                resultTimer = 0;
            } else {
                resultTimer += Time.deltaTime * 1.0f;
            }
        }
        HandleHousePlacement ();
    }

    private void EnableInventory () {
        gameCanvas.enabled = false;
        inventoryCanvas.enabled = true;
    }

    private void DisableInventory () {
        gameCanvas.enabled = true;
        inventoryCanvas.enabled = false;
    }

    public void AddToast () {
        toastsCollected += 1;
        ToastHelper ();
    }

    public void SubtractToast () {
        toastsCollected -= 1;
        ToastHelper ();
    }

    private void ToastHelper () {
        toastText.text = toastsCollected.ToString ();

        breadsCollected = toastsCollected / 3;
        breadButtonText.text = breadsCollected.ToString ();
    }

    public void AddBread () {
        toastsCollected += 3;
        toastText.text = toastsCollected.ToString ();

        breadsCollected += 1;
        breadButtonText.text = breadsCollected.ToString ();
    }

    public void SubtractBread () {
        toastsCollected -= 3;
        toastText.text = toastsCollected.ToString ();

        breadsCollected -= 1;
        breadButtonText.text = breadsCollected.ToString ();
    }

    public void DisableActions () {
        cancelButton.gameObject.SetActive (false);
        magnet.SetActive (false);
        holdButton.gameObject.SetActive (false);
        if (breadInHand.activeSelf) {
            breadInHand.SetActive (false);
            AddBread ();
        } else if (toastInHand.activeSelf) {
            toastInHand.SetActive (false);
            AddToast ();
        } else if (houseInHand != null && houseInHand.activeSelf) {
            houseInHand.SetActive (false);
        }

    }

    private void EnableMagnet () {
        DisableActions ();
        magnet.SetActive (true);
        DisableInventory ();
        cancelButton.gameObject.SetActive (true);
    }

    private void EnableToast () {
        if (toastsCollected == 0) {
            actionText.text = "Collect toast first!";
            DisableInventory ();
            return;
        }

        SubtractToast ();
        DisableActions ();
        toastInHand.SetActive (true);
        DisableInventory ();
        cancelButton.gameObject.SetActive (true);
        holdButton.gameObject.SetActive (true);
    }

    private void SpawnBread () {
        if (breadsCollected == 0) {
            actionText.text = "Collect bread first!";
            DisableInventory ();
            return;
        }
        
        timer = 0;
        SubtractBread ();
        breadInHand.SetActive (true);
        cancelButton.gameObject.SetActive (true);
        DisableInventory ();
    }

    private void EnableHouse () {
        DisableActions ();
        houseInHand.SetActive (true);
        DisableInventory ();
        cancelButton.gameObject.SetActive (true);
    }

    private void HandleHousePlacement () {
        if (houseInHand != null && houseInHand.activeSelf) {
            if (arCamera.transform.position.y <= -.7f) {
                houseScript.PlaceHouse (houseInHand.transform.position, houseInHand.transform.rotation);
                Destroy (houseInHand);
                cancelButton.gameObject.SetActive (false);
            }
        }
    }
}