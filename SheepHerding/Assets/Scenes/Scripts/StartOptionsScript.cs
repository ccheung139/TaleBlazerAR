using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartOptionsScript : MonoBehaviour {
    public Button drawSpaceButton;
    public Button createSpaceButton;
    public Button loadSpaceButton;
    public DrawSpaceScript drawSpaceScript;
    public Image logo;
    public Image placeGameBounds;

    void Start () {
        createSpaceButton.onClick.AddListener (CreateSpacePressed);
        loadSpaceButton.onClick.AddListener (LoadSpace);
    }

    private void CreateSpacePressed () {
        drawSpaceButton.gameObject.SetActive (true);
        StartSpace ();
    }

    private void LoadSpace () {
        StartSpace ();
        drawSpaceScript.LoadSpace ();
        placeGameBounds.gameObject.SetActive (true);
        
    }

    private void StartSpace () {
        createSpaceButton.gameObject.SetActive (false);
        loadSpaceButton.gameObject.SetActive (false);
        logo.gameObject.SetActive (false);
    }
}