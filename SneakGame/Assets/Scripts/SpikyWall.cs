using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpikyWall : MonoBehaviour {
    public Camera arCamera;
    public GameObject gameOverPanel;
    public GameObject gameWonPanel;
    public Text healthText;

    private float healthTimerTotal = 0.5f;
    private float healthTimer = 0;

    void Update () {
        float distance = Vector3.Distance (arCamera.transform.position, transform.position);
        CheckDamage (distance);
    }

    private void CheckDamage (float distance) {
        if (distance < 2f) {
            if (healthTimer >= healthTimerTotal) {
                DamageHealth ();
                healthTimer = 0;
            } else {
                healthTimer += Time.deltaTime * 1.0f;
            }
        }
    }

    private void DamageHealth () {
        int healthPoints = GetHealthPoints ();
        if (healthPoints == 1 && !gameWonPanel.activeSelf) {
            gameOverPanel.SetActive (true);
        } else if (healthPoints == 0) {
            return;
        }
        healthText.text = "Health: " + (healthPoints - 1);
    }

    private int GetHealthPoints () {
        int index = healthText.text.LastIndexOf (' ');
        string healthAmount = healthText.text.Substring (index + 1);
        return Int32.Parse (healthAmount);
    }
}