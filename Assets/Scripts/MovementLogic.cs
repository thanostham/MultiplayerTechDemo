//Contribution - Both
//Stratos Salpas

using System;
using UnityEngine;
using PurrNet;
using PurrNet.Transports;
using System.Collections;

public class MovementLogic : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float baseSpeed = 5f;

    public Rigidbody2D rb;
    public Camera cam;

    Vector2 mousePos;

    [SerializeField] private float boostSpeed = 10f;
    [SerializeField] private float boostDuration = 2f;
    [SerializeField] private float boostCooldown = 5f;

    private bool boostOn;
    private bool canBoost = true;

    protected override void OnSpawned()
    {
        UpdateCamera(isController);
        baseSpeed = movementSpeed;
        canBoost = true;
    }

    public void UpdateCamera(bool status)
    {
        cam.enabled = status;
    }
    
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {    
        if (!isController) return;

        //if (boostOn) return;

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        cam.transform.position = rb.transform.position.WithNewZ(-10);

        SendMovementToServer(mousePos);   

        if (Input.GetKeyDown(KeyCode.Space) && canBoost)
        {
            StartCoroutine(SpeedBoost());
        }

        MainGameView mainView = InstanceHandler.GetInstance<MainGameView>();
        if (mainView != null)
        {
            mainView.UpdateBoostStatus(canBoost);
        }
    }

    private void SendMovementToServer(Vector2 targetMousePos)
    {
        Vector2 target = Vector2.Lerp(rb.position, targetMousePos, 0.1f);
        Vector2 dir = (target - rb.position).normalized;

        rb.position += dir * movementSpeed * Time.deltaTime;

        Vector2 lookDir = targetMousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    private IEnumerator SpeedBoost()
    {
        Debug.Log("BoostOn");
        canBoost = false;
        boostOn = true;
        movementSpeed = boostSpeed;
        yield return new WaitForSeconds(boostDuration);
        boostOn = false;
        movementSpeed = baseSpeed;
        Debug.Log("BoostOff");
        yield return new WaitForSeconds(boostCooldown);
        canBoost = true;
    }
}
