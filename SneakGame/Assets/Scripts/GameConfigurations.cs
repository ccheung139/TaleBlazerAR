using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GameConfigurations : MonoBehaviour {
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private Canvas startCanvas;
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private ARSessionOrigin origin;
    [SerializeField]
    private Slider numZombiesSlider;
    [SerializeField]
    private Slider zombieDamageSlider;
    [SerializeField]
    private Slider zombieChaseSpeedSlider;
    [SerializeField]
    private Slider numWallsSlider;

    void Start () {
        canvas.gameObject.SetActive (false);
        startButton.onClick.AddListener (StartWithConfigurations);
    }

    private void StartWithConfigurations () {
        SpawnZombies script = origin.GetComponent<SpawnZombies> ();
        script.totalZombies = (int) numZombiesSlider.value;
        script.zombieDamage = (int) zombieDamageSlider.value;
        script.chaseSpeed = zombieChaseSpeedSlider.value;
        script.numWalls = (int) numWallsSlider.value;
        script.BeginSpawning ();
        startCanvas.gameObject.SetActive (false);
        canvas.gameObject.SetActive (true);
        origin.transform.position = new Vector3 (0, 0, 0);
        origin.transform.rotation = Quaternion.Euler (0, 0, 0);
    }
}