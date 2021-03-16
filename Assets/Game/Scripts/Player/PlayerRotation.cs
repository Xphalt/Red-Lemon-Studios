﻿//THIS IS A MODIFIED VERSION OF THE FIRST PERSON CHARACTER 'MOUSE LOOK' SCRIPT

using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerRotation : MonoBehaviour
{
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;
    public bool startLocked = true;

    public Camera firstPersonCamera;
    private Transform cameraTransform;
    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    private bool m_cursorIsLocked = true;
    private bool m_inputIsLocked = false;

    private void Start()
    {
        SetCursorLock(startLocked);

        m_CharacterTargetRot = transform.localRotation;
        m_CameraTargetRot = firstPersonCamera.transform.localRotation;
        cameraTransform = firstPersonCamera.transform;
    }


    public void Update()
    {
        InternalLockUpdate();
        

        float yRot = (m_cursorIsLocked) ? CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity : 0;
        float xRot = (m_cursorIsLocked) ? CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity : 0;

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (clampVerticalRotation)
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

        if (smooth)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, m_CameraTargetRot,
                smoothTime * Time.deltaTime);
        }
        else
        {
            transform.localRotation = m_CharacterTargetRot;
            cameraTransform.localRotation = m_CameraTargetRot;
        }
    }

    public void SetCursorLock(bool cursorLock, bool inputLock=false)
    {
        if (cursorLock) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;

        Cursor.visible = !cursorLock;

        m_cursorIsLocked = cursorLock;
        m_inputIsLocked = inputLock;
    }

    private void InternalLockUpdate()
    {
        if (!m_inputIsLocked)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                SetCursorLock(false);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                SetCursorLock(true);
            }
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}
