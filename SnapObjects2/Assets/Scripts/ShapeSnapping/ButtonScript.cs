using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour {
    public Button cubeButton;
    public Button sphereButton;
    public Button cylinderButton;
    public Button cancelButton;
    public Button placeButton;
    public Button wholeButton;
    public Button deleteButton;
    public Button moveButton;
    public Button finishMoveButton;
    public Button selectButton;
    public Button finishSelectButton;
    public Button rotateButton;
    public Button scaleButton;
    public Button copyButton;
    public Button pasteButton;
    public Button colorButton;
    public Button pickColorButton;
    public Button finishPickColorButton;
    public Button exitPickColorButton;

    public Canvas canvas;
    public Canvas colorCanvas;

    void Start () {
        cubeButton.onClick.AddListener (CubePressed);
        sphereButton.onClick.AddListener (SpherePressed);
        cylinderButton.onClick.AddListener (CylinderPressed);
        cancelButton.onClick.AddListener (CancelPressed);
        placeButton.onClick.AddListener (PlacePressed);
        wholeButton.onClick.AddListener (WholePressed);
        deleteButton.onClick.AddListener (DeletePressed);
        moveButton.onClick.AddListener (MovePressed);
        finishMoveButton.onClick.AddListener (FinishMovePressed);
        selectButton.onClick.AddListener (SelectPressed);
        finishSelectButton.onClick.AddListener (FinishSelectPressed);
        rotateButton.onClick.AddListener (RotatePressed);
        scaleButton.onClick.AddListener (ScalePressed);
        copyButton.onClick.AddListener (CopyPressed);
        pasteButton.onClick.AddListener (PastePressed);
        colorButton.onClick.AddListener (ColorPressed);
        pickColorButton.onClick.AddListener (PickColorPressed);
        finishPickColorButton.onClick.AddListener (FinishPickColorPressed);
        exitPickColorButton.onClick.AddListener (ExitPickColorPressed);

        HideAll ();
        ShowNeutralChoices ();
    }

    private void CubePressed () {
        HideAll ();
        placeButton.gameObject.SetActive (true);
        cancelButton.gameObject.SetActive (true);
    }
    private void SpherePressed () {
        HideAll ();
        placeButton.gameObject.SetActive (true);
        cancelButton.gameObject.SetActive (true);
    }
    private void CylinderPressed () {
        HideAll ();
        placeButton.gameObject.SetActive (true);
        cancelButton.gameObject.SetActive (true);
    }
    public void CancelPressed () {
        HideAll ();
        ShowNeutralChoices ();
    }
    private void PlacePressed () {
        HideAll ();
        ShowNeutralChoices ();
    }
    private void WholePressed () {
        wholeButton.gameObject.SetActive (false);
    }
    private void DeletePressed () {
        HideAll ();
        ShowNeutralChoices ();
    }
    private void MovePressed () {
        moveButton.gameObject.SetActive (false);
        HideAll ();
        cancelButton.gameObject.SetActive (true);
        finishMoveButton.gameObject.SetActive (true);
    }
    private void FinishMovePressed () {
        HideAll ();
        ShowNeutralChoices ();
    }
    private void SelectPressed () {
        selectButton.gameObject.SetActive (false);
        HideAll ();
        cancelButton.gameObject.SetActive (true);
        finishSelectButton.gameObject.SetActive (true);
    }
    private void FinishSelectPressed () {
        HideAll ();
        ShowSelectChoices ();
    }
    private void RotatePressed () {
        HideAll ();
        cancelButton.gameObject.SetActive (true);
        rotateButton.gameObject.SetActive (true);
    }
    private void ScalePressed () {
        HideAll ();
        cancelButton.gameObject.SetActive (true);
        scaleButton.gameObject.SetActive (true);
    }
    private void CopyPressed () {
        HideAll ();
        cancelButton.gameObject.SetActive (true);
        pasteButton.gameObject.SetActive (true);
    }
    private void PastePressed () {
        HideAll ();
        ShowSelectChoices ();
    }

    private void ColorPressed () {
        canvas.gameObject.SetActive (false);
        cancelButton.gameObject.SetActive (true);
        pickColorButton.gameObject.SetActive (true);
        exitPickColorButton.gameObject.SetActive (true);
    }

    private void PickColorPressed () {
        pickColorButton.gameObject.SetActive (false);
        finishPickColorButton.gameObject.SetActive (true);
    }

    private void FinishPickColorPressed () {
        pickColorButton.gameObject.SetActive (true);
        finishPickColorButton.gameObject.SetActive (false);
    }

    private void ExitPickColorPressed () {
        canvas.gameObject.SetActive (true);
        HideAll ();
        ShowNeutralChoices ();
    }

    private void HideAll () {
        cubeButton.gameObject.SetActive (false);
        sphereButton.gameObject.SetActive (false);
        cylinderButton.gameObject.SetActive (false);
        cancelButton.gameObject.SetActive (false);
        placeButton.gameObject.SetActive (false);
        wholeButton.gameObject.SetActive (false);
        deleteButton.gameObject.SetActive (false);
        moveButton.gameObject.SetActive (false);
        finishMoveButton.gameObject.SetActive (false);
        selectButton.gameObject.SetActive (false);
        finishSelectButton.gameObject.SetActive (false);
        rotateButton.gameObject.SetActive (false);
        scaleButton.gameObject.SetActive (false);
        copyButton.gameObject.SetActive (false);
        pasteButton.gameObject.SetActive (false);
        colorButton.gameObject.SetActive (false);

        colorCanvas.gameObject.SetActive (false);
        pickColorButton.gameObject.SetActive (false);
        finishPickColorButton.gameObject.SetActive (false);
        exitPickColorButton.gameObject.SetActive (false);
    }

    private void ShowNeutralChoices () {
        cubeButton.gameObject.SetActive (true);
        sphereButton.gameObject.SetActive (true);
        cylinderButton.gameObject.SetActive (true);
        selectButton.gameObject.SetActive (true);
        colorButton.gameObject.SetActive (true);
    }

    public void HideNeutralChoices () {
        cubeButton.gameObject.SetActive (false);
        sphereButton.gameObject.SetActive (false);
        cylinderButton.gameObject.SetActive (false);
        selectButton.gameObject.SetActive (false);
        colorButton.gameObject.SetActive (false);
    }

    public void ShowSelectChoices () {
        cancelButton.gameObject.SetActive (true);
        wholeButton.gameObject.SetActive (true);
        deleteButton.gameObject.SetActive (true);
        moveButton.gameObject.SetActive (true);
        rotateButton.gameObject.SetActive (true);
        scaleButton.gameObject.SetActive (true);
        copyButton.gameObject.SetActive (true);
    }
}