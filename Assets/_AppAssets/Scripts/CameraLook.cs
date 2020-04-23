using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraLook : MonoBehaviourPunCallbacks
{
    public static bool cursorLocked = true;

    public Transform player;
    public Transform cams;
    public float xSensitivity;
    public float ySensitivity;

    [SerializeField] private float maxAngle;

    private Quaternion camCenter;

    private void Start()
    {
        camCenter = cams.localRotation;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        SetY();
        SetX();

        UpdateCursorLock();
    }

    private void SetY()
    {
        float t_Input = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
        Quaternion t_Adj = Quaternion.AngleAxis(t_Input, Vector3.left);
        Quaternion t_Delta = cams.localRotation * t_Adj;

        if (Quaternion.Angle(camCenter, t_Delta) < maxAngle)
        {
            cams.localRotation = t_Delta;
        }

        if (GetComponent<PlayerController>().weaponContainer)
        {
            GetComponent<PlayerController>().weaponContainer.localRotation = t_Delta;
        }
    }

    private void SetX()
    {
        float t_Input = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
        Quaternion t_Adj = Quaternion.AngleAxis(t_Input, Vector3.up);
        Quaternion t_Delta = player.localRotation * t_Adj;
        player.localRotation = t_Delta;
    }

    private void UpdateCursorLock()
    {
        if (cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cursorLocked = false;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cursorLocked = true;
            }
        }
    }
}
