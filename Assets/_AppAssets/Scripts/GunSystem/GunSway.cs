using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class GunSway : MonoBehaviourPunCallbacks
{
    [SerializeField] private float intensity;
    [SerializeField] private float smooth;

    private Quaternion originRotation;

    private void Start()
    {
        originRotation = transform.localRotation;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        UpdateSway();
    }

    private void UpdateSway()
    {
        float xMouse = Input.GetAxis("Mouse X");
        float yMouse = Input.GetAxis("Mouse Y");

        Quaternion xAdj = Quaternion.AngleAxis(-intensity * xMouse, Vector3.up);
        Quaternion yAdj = Quaternion.AngleAxis(intensity * yMouse, Vector3.right);
        Quaternion targetRotation = originRotation * xAdj * yAdj;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
    }
}
