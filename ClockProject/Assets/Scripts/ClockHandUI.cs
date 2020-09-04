using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClockHandUI : MonoBehaviour
{
    private float SECONDS_IN_MINUTE = 60f;
    private float MINUTES_IN_HOUR = 60f;
    private float SECONDS_IN_HOUR = 3600f;
    private float HOURS_IN_DAY = 24f;
    private float DEGREES = 360f;

    public GameObject secondHand;
    public GameObject minuteHand;
    public GameObject hourHand;

    private float seconds;
    private float minutes;
    private float hours;

    private void Start() {
        DateTime localDate = DateTime.Now;
        float startingSecond = (float)localDate.Second;
        float startingMinute = (float)localDate.Minute;
        float startingHour = (float)localDate.Hour;

        seconds = startingSecond/SECONDS_IN_MINUTE;
        minutes = (startingMinute + seconds)/MINUTES_IN_HOUR;
        hours = (startingHour + minutes)/(HOURS_IN_DAY/2);
    }

    private void Update()
    {
        seconds += (Time.deltaTime / SECONDS_IN_MINUTE);
        minutes += (Time.deltaTime / SECONDS_IN_HOUR);
        hours += (Time.deltaTime / (SECONDS_IN_HOUR*SECONDS_IN_MINUTE));

        float secondsNorm = (seconds*DEGREES) % DEGREES;
        float minutesNorm = (minutes*DEGREES) % DEGREES;
        float hoursNorm = (hours*DEGREES) % DEGREES;

        secondHand.transform.eulerAngles = new Vector3(0, 0, -secondsNorm);
        minuteHand.transform.eulerAngles = new Vector3(0, 0, -minutesNorm);
        hourHand.transform.eulerAngles = new Vector3(0, 0, -hoursNorm);
    }
}
