using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviourPunCallbacks
{
    #region Movement
    [Header("Movement")]
    [SerializeField] private Camera normalCam;
    [SerializeField] private GameObject cameraParent;

    private Rigidbody myRB;
    private float baseFOV;

    #region Walk
    [Header("Walk")]
    public float walkSpeed;
    #endregion

    #region Run
    [Header("Run")]
    public float runSpeed;

    [SerializeField] private float runFOVModifier = 1.5f;
    [SerializeField] private float runFOVDelay = 8f;

    private bool isSprinting;
    #endregion

    #region Jump
    [Header("Jump")]
    public float jumpForce;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.1f;

    [SerializeField] private bool isGrounded;
    private bool isJumping;
    #endregion
    #endregion

    #region Shooting
    [Header("Shooting")]
    [SerializeField] private Gun gun;

    #endregion

    #region Others
    [Header("Others")]
    public Transform weaponContainer;
    public bool isHeadBob;

    private float headbobCounter;
    private Vector3 weaponContainerOrigin;
    private Vector3 targetWeaponContainerPos;
    #endregion

    private void Start()
    {
        cameraParent.SetActive(photonView.IsMine);

        if (!photonView.IsMine) { gameObject.layer = 12; }

        baseFOV = normalCam.fieldOfView;
        if (Camera.main)
        {
            Camera.main.enabled = false;
        }
        myRB = GetComponent<Rigidbody>();

        weaponContainerOrigin = weaponContainer.localPosition;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        Shoot();
        if (isHeadBob)
        {
            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            {
                HeadBob(headbobCounter, 0.025f, 0.025f);
                headbobCounter += Time.deltaTime;
                weaponContainer.localPosition = Vector3.Lerp(weaponContainer.localPosition, targetWeaponContainerPos, Time.deltaTime * 2);
            }
            else if (!isSprinting)
            {
                HeadBob(headbobCounter, 0.035f, 0.035f);
                headbobCounter += Time.deltaTime * 3;
                weaponContainer.localPosition = Vector3.Lerp(weaponContainer.localPosition, targetWeaponContainerPos, Time.deltaTime * 6);
            }
            else
            {
                HeadBob(headbobCounter, 0.15f, 0.075f);
                headbobCounter += Time.deltaTime * 7;
                weaponContainer.localPosition = Vector3.Lerp(weaponContainer.localPosition, targetWeaponContainerPos, Time.deltaTime * 10);
            }
        }
        else
        {
            weaponContainer.localPosition = weaponContainerOrigin;
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        Move();
        Jump();
    }

    public void Move()
    {
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        movement.Normalize();
        movement = transform.TransformDirection(movement);

        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetAxisRaw("Vertical") > 0 && isGrounded)
        {
            isSprinting = true;
            movement *= runSpeed;
            normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * runFOVModifier, Time.deltaTime * 8f);
        }
        else
        {
            isSprinting = false;
            movement *= walkSpeed;
            normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV, Time.deltaTime * 8f);
        }
        movement *= Time.deltaTime;
        movement.y = myRB.velocity.y;

        myRB.velocity = movement;
    }

    public void Jump()
    {
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundCheckRadius, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isGrounded = false;
            isJumping = true;
            myRB.AddForce(Vector3.up * jumpForce);
        }
    }

    public void Shoot()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            gun.photonView.RPC("Shoot", RpcTarget.All);
        }
        else if (Input.GetButton("Fire1") && (gun.gunType == GunType.Semi || gun.gunType == GunType.Auto))
        {
            gun.photonView.RPC("ShootContinuous", RpcTarget.All);
        }
    }

    private void HeadBob(float z, float xIntensity, float yIntensity)
    {
        targetWeaponContainerPos = weaponContainerOrigin + new Vector3(Mathf.Cos(z) * xIntensity, Mathf.Sin(z * 2) * yIntensity, 0);
    }
}
