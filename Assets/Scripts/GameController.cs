using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    private int count; // how many pick-ups are collected
    private int numPickups;
    private LineRenderer lineRenderer;
    public Text scoreText;
    public Text winText;
    public Text positionText;
    public Text velocityText;
    public Text distanceText;
    public PlayerController playerController;
    private GameObject[] pickUpObjs;
    private GameObject closestPickUpObj;
    private GameObject PU;
    private float distance;

    private enum ChangeState
    {
        Normal,
        Distance,
        Vision
    };

    private ChangeState state;


    // Start is called before the first frame update
    public void Start()
    {
        count = 0;
        winText.text = "";
        positionText.text = "";
        velocityText.text = "";
        distanceText.text = "";
        pickUpObjs =
            GameObject.FindGameObjectsWithTag("PickUp"); // Tim tat ca cac pick up co tag la PickUp de cho vao 1 array
        numPickups = pickUpObjs.Length;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        state = ChangeState.Normal;
        SetCountText();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (state)
            {
                case ChangeState.Normal:
                    state = ChangeState.Distance;
                    break;
                case ChangeState.Distance:
                    state = ChangeState.Vision;
                    break;
                case ChangeState.Vision:
                    state = ChangeState.Normal;
                    break;
            }
        }

        if (state == ChangeState.Normal)
        {
            lineRenderer.enabled = false;
            foreach (var pickup in pickUpObjs)
            {
                pickup.GetComponent<Renderer>().material.color = Color.white;
            }
            positionText.text = "";
            velocityText.text = "";
            distanceText.text = "";
        } else if (state == ChangeState.Distance)
        {
            lineRenderer.enabled = true;
            positionText.text = "Position " + playerController.transform.position;
            velocityText.text = "Velocity " + playerController.GetTagetPos().magnitude.ToString("0.0");
            ClosestPickup();
            if (count < numPickups)
            {
                distanceText.text = "Distance " + distance.ToString("0.0");
                closestPickUpObj.GetComponent<Renderer>().material.color = Color.blue;

                // 0 for the start point , position vector ’ startPosition ’
                lineRenderer.SetPosition(0, playerController.transform.position);
                // 1 for the end point , position vector ’endPosition ’
                lineRenderer.SetPosition(1, closestPickUpObj.transform.position);
                // Width of 0.1 f both at origin and end of the line
                lineRenderer.startWidth = 0.1f;
            }
            else
            {
                distanceText.text = "Distance 0";
                lineRenderer.enabled = false;
            }
        }
        else
        {
            var temp = playerController.GetTagetPos();
            lineRenderer.enabled = true;
            var playerPosition = playerController.transform.position;
            lineRenderer.SetPosition(0, playerPosition);
            lineRenderer.SetPosition(1, playerPosition + temp * 0.5f);
            lineRenderer.startWidth = 0.1f;
            positionText.text = "";
            velocityText.text = "";
            distanceText.text = "";

            if (numPickups > count)
            {
                var smallestAngle = 180.0f;
                foreach (var pickUp in pickUpObjs)
                {
                    if (!pickUp.activeSelf)
                    {
                        continue;
                    }
                    pickUp.GetComponent<Renderer>().material.color = Color.white;
                
                    var angle = Vector3.Angle(temp, pickUp.transform.position - playerPosition); // cong thuc tinh angle (vector player, vector noi tu player -> pickup)
               
                    if (angle < smallestAngle)
                    {
                        smallestAngle = angle;
                        PU= pickUp;
                    }
                }
                PU.GetComponent<Renderer>().material.color = Color.green;
                PU.transform.LookAt(playerController.transform);
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }
        
    }


    public void SetCountText()
    {
        scoreText.text = "Score: " + count.ToString();
        if (count >= numPickups)
        {
            winText.text = "You Win!";
        }
        
    }

    public void UpdateCount()
    {
        count++;
    }

    private void ClosestPickup()
    {
        distance = float.MaxValue;
        foreach (var pickUp in pickUpObjs)
        {
            if (!pickUp.activeSelf)
            {
                continue;
            }

            pickUp.GetComponent<Renderer>().material.color = Color.white;
            var temp = Vector3.Distance(playerController.transform.position,
                pickUp.transform.position); // khoang cach giua player va pick up
            if (distance > temp)
            {
                distance = temp;
                closestPickUpObj = pickUp;
            }
        }
    }
}