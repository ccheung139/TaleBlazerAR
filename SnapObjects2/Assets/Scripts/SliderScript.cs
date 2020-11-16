using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour {
    public PlacementScript ps;
    public SelectObjectsScript selectObjectsScript;

    public Slider xSlider;
    public Slider ySlider;
    public Slider zSlider;
    public Slider xSliderScale;
    public Slider ySliderScale;
    public Slider zSliderScale;

    private float previousXValue;
    private float previousYValue;
    private float previousZValue;
    private float previousXValueScale;
    private float previousYValueScale;
    private float previousZValueScale;

    void Start () {
        xSlider.onValueChanged.AddListener (OnXSliderChanged);
        ySlider.onValueChanged.AddListener (OnYSliderChanged);
        zSlider.onValueChanged.AddListener (OnZSliderChanged);
        xSliderScale.onValueChanged.AddListener (OnXSliderChangedScale);
        ySliderScale.onValueChanged.AddListener (OnYSliderChangedScale);
        zSliderScale.onValueChanged.AddListener (OnZSliderChangedScale);
    }

    private void SliderChanger (float previousValue, Vector3 newAngle, float value) {
        if (ps.preliminaryObject == null) {
            return;
        }
        float delta = value - previousValue;
        if (ps.preliminaryObject.name == "Cylinder") {
            GameObject parent = ps.preliminaryObject.transform.parent.gameObject;
            if (ps.allSelects.Count == 0) {
                parent.transform.RotateAround (parent.transform.position, newAngle, delta * 360);
            } else {
                Transform topOfCylinder = ps.preliminaryObject.transform.Find ("TopOfCylinder");
                parent.transform.RotateAround (topOfCylinder.position, newAngle, delta * 360);
            }
        } else {
            ps.preliminaryObject.transform.RotateAround (ps.preliminaryObject.transform.position, newAngle, delta * 360);
        }
    }

    void OnXSliderChanged (float value) {
        Vector3 forwardVector = ps.preliminaryObject.transform.forward;
        SliderChanger (previousXValue, forwardVector, value);
        previousXValue = value;
    }

    void OnYSliderChanged (float value) {
        Vector3 upVector = ps.preliminaryObject.transform.up;
        SliderChanger (previousYValue, upVector, value);
        previousYValue = value;
    }

    void OnZSliderChanged (float value) {
        Vector3 rightVector = ps.preliminaryObject.transform.right;
        SliderChanger (previousZValue, rightVector, value);
        previousZValue = value;
    }

    private void SizeSliderChanger (float delta, Vector3 scaleChange) {
        GameObject objectToChange;
        if (ps.preliminaryObject != null) {
            objectToChange = ps.preliminaryObject;
        } else if (selectObjectsScript.selectedShape != null) {
            objectToChange = selectObjectsScript.selectedShape;
        } else {
            return;
        }

        if (objectToChange.name == "Cylinder") {
            GameObject parent = objectToChange.transform.parent.gameObject;
            parent.transform.localScale += scaleChange;
        } else {
            objectToChange.transform.localScale += scaleChange;
        }
    }

    void OnXSliderChangedScale (float value) {
        float delta = value - previousXValueScale;
        Vector3 scaleChange = new Vector3 (delta, 0, 0);
        SizeSliderChanger (delta, scaleChange);
        previousXValueScale = value;
    }
    void OnYSliderChangedScale (float value) {
        float delta = value - previousYValueScale;
        Vector3 scaleChange = new Vector3 (0, delta, 0);
        SizeSliderChanger (delta, scaleChange);
        previousYValueScale = value;
    }
    void OnZSliderChangedScale (float value) {
        float delta = value - previousZValueScale;
        Vector3 scaleChange = new Vector3 (0, 0, delta);
        SizeSliderChanger (delta, scaleChange);
        previousZValueScale = value;
    }
}