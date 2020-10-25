using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonScript : MonoBehaviour {
    public Button holdButton;
    public Button startButton;
    public Text resultText;

    void Start () {
        startButton.onClick.AddListener (EnableHoldButton);
        holdButton.gameObject.SetActive (false);
    }

    private void EnableHoldButton () {
        resultText.text = "";
        holdButton.gameObject.SetActive (true);
        startButton.gameObject.SetActive (false);
    }
}