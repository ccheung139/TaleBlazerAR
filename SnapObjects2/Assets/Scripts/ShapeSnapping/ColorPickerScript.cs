using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerScript : MonoBehaviour {
    public Text redText;
    public Text greenText;
    public Text blueText;
    public GameObject paintRoller;
    public GameObject paintRollerObj;
    public Button colorButton;
    public Button pickColorButton;
    public Button finishPickColorButton;
    public Button exitPickColorButton;
    public Canvas colorCanvas;

    private float redValue = 0;
    private float greenValue = 0;
    private float blueValue = 0;

    private Color color;

    void Start () {
        color = new Color (0, 0, 0);
        colorButton.onClick.AddListener (ColorPressed);
        pickColorButton.onClick.AddListener (PickColorPressed);
        finishPickColorButton.onClick.AddListener (FinishPickColorPressed);
        exitPickColorButton.onClick.AddListener (ExitPickColorPressed);
    }

    public void RedChanged (float newRed) {
        redValue = newRed;
        redText.text = ConvertTo255 (newRed);
        UpdateColor ();
    }

    public void GreenChanged (float newGreen) {
        greenValue = newGreen;
        greenText.text = ConvertTo255 (newGreen);
        UpdateColor ();
    }

    public void BlueChanged (float newBlue) {
        blueValue = newBlue;
        blueText.text = ConvertTo255 (newBlue);
        UpdateColor ();
    }

    private string ConvertTo255 (float colorValue) {
        float scaled = colorValue * 255;
        return ((int) scaled).ToString ();
    }

    private void UpdateColor () {
        color = new Color (redValue, greenValue, blueValue);
        Material mat = paintRollerObj.GetComponent<Renderer> ().material;
        mat.color = color;
    }

    private void ColorPressed () {
        paintRoller.gameObject.SetActive (true);

    }

    private void PickColorPressed () {
        colorCanvas.gameObject.SetActive (true);
        pickColorButton.gameObject.SetActive (false);
    }

    private void FinishPickColorPressed () {
        colorCanvas.gameObject.SetActive (false);
    }

    private void ExitPickColorPressed () {
        paintRoller.gameObject.SetActive (false);
    }
}