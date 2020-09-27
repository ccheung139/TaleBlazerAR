using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophyScript : MonoBehaviour {
    public Camera arCamera;
    public GameObject gameOverPanel;
    public GameObject gameWonPanel;

    void Update () {
        float distance = Vector3.Distance (arCamera.transform.position, new Vector3 (transform.position.x, arCamera.transform.position.y, transform.position.z));
        if (distance < 2f) {
            if (!gameOverPanel.activeSelf) {
                gameWonPanel.SetActive(true);
            }
        }
    }
}