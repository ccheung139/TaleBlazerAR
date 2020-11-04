using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastThrownScript : MonoBehaviour {
    public InventoryScript invScript;
    public List<GameObject> allToasts;
    public GameObject toastPrefab;
    private GameObject spawnedToast;
    private float throwSpeed;
    private Vector3 direction;

    private float timer = 0;
    private float timerTotal = 0.2f;

    void Update () {
        if (spawnedToast == null) {
            return;
        }
        if (spawnedToast.transform.position.y >= -.7f) {
            float step = throwSpeed * Time.deltaTime;
            spawnedToast.transform.position += (direction) * step;
            HandleVertical ();
        } else {
            allToasts.Add (spawnedToast);
        }
    }

    private void HandleVertical () {
        if (timer >= timerTotal) {
            spawnedToast.transform.position -= new Vector3 (0, .03f, 0);
        } else {
            spawnedToast.transform.position -= new Vector3 (0, .008f, 0);
            timer += Time.deltaTime * 1f;
        }
    }

    public void StartThrow (Vector3 newDirection, Vector3 startPosition, Quaternion rotation) {
        timer = 0;
        throwSpeed = 4f;
        direction = newDirection;
        GameObject newToast = Instantiate (toastPrefab, startPosition, toastPrefab.transform.rotation);
        spawnedToast = newToast;
    }

    public void RemoveAllToasts () {
        foreach (GameObject toastToClear in allToasts) {
            Destroy (toastToClear);
        }
        allToasts.Clear ();
    }
}