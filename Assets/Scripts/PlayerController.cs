using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Vector3 lastPos;
    private Vector3 targetPos;
    private GameController gameController;
    private void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gameController.SetCountText();
        lastPos = transform.position;
    }

    public Vector3 GetTagetPos()
    {
        return targetPos;
    }
    void FixedUpdate()
    {
        float horAxis = Input.GetAxis("Horizontal");
        float verAxis = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horAxis, 0.0f, verAxis);

        GetComponent<Rigidbody>().AddForce(movement * speed * Time.deltaTime);
        movement = transform.position;

        if (lastPos != movement)
        {
            targetPos = movement - lastPos;
            targetPos /= Time.deltaTime;
            
        }
        lastPos = transform.position;

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            gameController.UpdateCount();
            gameController.SetCountText();
        }
    }

   
}
