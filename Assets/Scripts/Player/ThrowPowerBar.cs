﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ThrowPowerBar : MonoBehaviour
{
    [SerializeField] private Image indicator;
    private bool isFillingUp = true;
    private float fillSpeed;
    [SerializeField] private PlayerArm playerArm;
    [SerializeField] private Vector2 offset = new Vector2(-5f, 5f);
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        fillSpeed = 1f / playerArm.TimeTo100Percent;
    }

    private void Update()
    {
        FillIndicator();
        FollowMouse();
        CheckIfMouseIsStillHeld();
    }

    private void FillIndicator()
    {
        int sign = isFillingUp ? 1 : -1;
        indicator.fillAmount = Mathf.Clamp(indicator.fillAmount + sign * fillSpeed * Time.deltaTime, 0f, 1f);
        if (indicator.fillAmount == 0f || indicator.fillAmount == 1f)
            isFillingUp = !isFillingUp;
    }

    private void FollowMouse()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 position = mainCamera.ScreenToWorldPoint(mousePosition);
        transform.position = position + offset;
    }

    private void CheckIfMouseIsStillHeld()
    {
        if (!Mouse.current.leftButton.isPressed)
            Destroy(gameObject);
    }
}
