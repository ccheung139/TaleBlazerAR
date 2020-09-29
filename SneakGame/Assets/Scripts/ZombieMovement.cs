using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieMovement : MonoBehaviour {
    public System.Random rand;
    public Camera arCamera;
    public GameObject zombieBulb;
    public GameObject orangeBulb;
    public GameObject yellowBulb;
    public Camera zombieCamera;
    public GameObject gameOverPanel;
    public GameObject gameWonPanel;
    public Text healthText;
    public int dangerLevel = 0;
    public GameObject yellowCanvas;
    public GameObject orangeCanvas;
    public GameObject redCanvas;
    public int currentDangerLevel = 0;
    public bool leftArrowOn = false;
    public bool rightArrowOn = false;

    private float moveAfterSeconds = 5.0f;
    private float stillTimer = 0;
    private bool isMoving = false;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Ray ray;
    private RaycastHit hit;
    private float suspisciousTimerTotal = 5.0f;
    private float suspisciousTimer = 0;
    private bool suspisciousBlocked = false;
    private float healthTimerTotal = 0.3f;
    private float healthTimer = 0;
    private float bulbTimer = 0;
    private float bulbTimerTotal = 1.0f;
    private float angryTimer = 0;
    private float angryTimerTotal = 1.0f;

    private Queue<float> lastFiveSpeeds = new Queue<float> ();
    private int sizeOfQueue = 0;
    private float lastDistance = 0;
    private float cumulativeSpeed = 0;

    private bool suspiscious = false;

    void Update () {
        float distance = Vector3.Distance (arCamera.transform.position, new Vector3 (transform.position.x, arCamera.transform.position.y, transform.position.z));
        if (suspiscious) {
            HandleSuspisciousZombie (distance);
        } else {
            HandleNormalMovements ();
            float distanceTraveled = lastDistance - distance;
            lastDistance = distance;
            HandleSpeedQueue (distanceTraveled);

            if (sizeOfQueue < 5) {
                return;
            }
            float avgSpeed = cumulativeSpeed / sizeOfQueue;
            AlertZombie (avgSpeed, distance);
        }
        CheckDamage (distance);
        CheckOffScreenIndicator (distance);
    }

    private void HandleSuspisciousZombie (float distance) {
        Renderer userRenderer = arCamera.gameObject.GetComponent<Renderer> ();

        if (distance > 7f) {
            if (angryTimer >= angryTimerTotal) {
                angryTimer = 0;
                dangerLevel = 0;
                suspiscious = false;
                zombieBulb.SetActive (false);
                healthTimer = 0;
                currentDangerLevel = 0;
            } else {
                angryTimer += Time.deltaTime * 1.0f;
            }
        } else {
            Vector3 zombieEyePosition = transform.position + new Vector3 (0, 1.6f, 0);
            var n = arCamera.transform.position - zombieEyePosition;
            ray.origin = zombieEyePosition;
            ray.direction = n;

            if (Physics.Raycast (ray, out hit, 30f)) {
                if (hit.collider.gameObject != arCamera.gameObject) {
                    if (suspisciousTimer >= suspisciousTimerTotal) {
                        suspisciousTimer = 0;
                        dangerLevel = 0;
                        suspiscious = false;
                        angryTimer = 0;
                        zombieBulb.SetActive (false);
                        healthTimer = 0;
                        currentDangerLevel = 0;
                    } else {
                        suspisciousTimer += Time.deltaTime * 1.0f;
                    }
                    return;
                } else {
                    suspisciousTimer = 0;
                }
            }

            n.y = 0;
            if (n.x == 0 && n.z == 0) {
                return;
            }
            var newRotation = Quaternion.LookRotation (n) * Quaternion.Euler (0, 0, 0);

            transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, Time.deltaTime * 1f);
            MoveZombie (new Vector3 (arCamera.transform.position.x, transform.position.y, arCamera.transform.position.z), 1.3f);
        }
    }

    private void CheckDamage (float distance) {
        if (distance < 1f) {
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
        if (healthPoints == 0) {
            return;
        } else if (healthPoints <= 5 && !gameWonPanel.activeSelf) {
            gameOverPanel.SetActive (true);
        }
        int newHealth = Math.Max (healthPoints - 5, 0);
        healthText.text = "Health: " + (newHealth);
    }

    private int GetHealthPoints () {
        int index = healthText.text.LastIndexOf (' ');
        string healthAmount = healthText.text.Substring (index + 1);
        return Int32.Parse (healthAmount);
    }

    private void HandleSpeedQueue (float distanceTraveled) {
        float speed = distanceTraveled / (Time.deltaTime);
        if (sizeOfQueue == 5) {
            float dequeuedSpeed = lastFiveSpeeds.Dequeue ();
            sizeOfQueue -= 1;
            cumulativeSpeed -= dequeuedSpeed;
        } else {
            lastFiveSpeeds.Enqueue (speed);
            sizeOfQueue += 1;
            cumulativeSpeed = speed;
        }
    }

    private void AlertZombie (float avgSpeed, float distance) {
        if (distance > 6f) {
            currentDangerLevel = 0;
            return;
        }
        if (avgSpeed >.2f) {
            suspiscious = true;
            zombieBulb.SetActive (true);
            orangeBulb.SetActive (false);
            yellowBulb.SetActive (false);
            angryTimer = 0;
            bulbTimer = 0;
            currentDangerLevel = 3;

            Vector3 direction = arCamera.transform.position - transform.position;
            ray = new Ray (transform.position, direction);
        } else {
            if (avgSpeed >.13f) {
                orangeBulb.SetActive (true);
                yellowBulb.SetActive (false);
                bulbTimer = 0;
                currentDangerLevel = 2;
            } else if (avgSpeed >.07f) {
                if (!orangeBulb.activeSelf || bulbTimer >= bulbTimerTotal) {
                    yellowBulb.SetActive (true);
                    orangeBulb.SetActive (false);
                    bulbTimer = 0;
                    currentDangerLevel = 1;
                } else {
                    bulbTimer += Time.deltaTime * 1.0f;
                }
            } else {
                if (bulbTimer >= bulbTimerTotal) {
                    yellowBulb.SetActive (false);
                    orangeBulb.SetActive (false);
                    bulbTimer = 0;
                    currentDangerLevel = 0;
                } else {
                    bulbTimer += Time.deltaTime * 1.0f;
                }

            }
        }
    }

    private void HandleNormalMovements () {
        if (isMoving) {
            MoveZombie (targetPosition, .5f);
            RotateZombie ();
        } else {
            if (stillTimer >= moveAfterSeconds) {
                isMoving = true;
                targetPosition = GenerateTargetPosition ();
                stillTimer = 0;
                moveAfterSeconds = (float) ((rand.NextDouble () * 8.0) + 3.0);

                Vector3 direction = targetPosition - transform.position;
                targetRotation = Quaternion.LookRotation (targetPosition - transform.position);

            } else {
                stillTimer += Time.deltaTime * 1.0f;
            }
        }
    }

    private void CheckOffScreenIndicator (float distance) {
        if (distance <= 1.0f || !suspiscious) {
            leftArrowOn = false;
            rightArrowOn = false;
            return;
        }
        if (!GetComponent<Renderer> ().IsVisibleFrom (arCamera)) {
            Vector3 direction = transform.position - arCamera.transform.position;
            float angle = Vector3.SignedAngle (direction, arCamera.transform.forward, Vector3.up);
            Debug.Log (angle);
            if (angle < -5.0f) {
                Debug.Log ("turn right");
                rightArrowOn = true;
            } else if (angle > 5.0f) {
                Debug.Log ("turn left");
                leftArrowOn = true;
            }
        } else {
            leftArrowOn = false;
            rightArrowOn = false;
        }
    }

    private Vector3 GenerateTargetPosition () {
        Vector3 currentPosition = transform.position;
        float randX = (float) ((rand.NextDouble () * 10.0) - 5.0);
        float randZ = (float) ((rand.NextDouble () * 10.0) - 5.0);
        Vector3 newPosition = currentPosition + new Vector3 ((rand.Next (1) == 0) ? randX : -randX, 0, (rand.Next (1) == 0) ? randZ : -randZ);
        return newPosition;
    }

    private void MoveZombie (Vector3 targetPosition, float movementSpeed) {
        if (transform.position == targetPosition) {
            isMoving = false;
            return;
        }

        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards (transform.position, targetPosition, step);
    }

    private void RotateZombie () {
        float movementSpeed = 200f;
        var step = movementSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, step);
    }
}