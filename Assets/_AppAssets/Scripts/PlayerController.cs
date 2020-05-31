using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviourPunCallbacks
{
    #region Movement Variables
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

    #region Shooting Variables
    [Header("Shooting")]
    [SerializeField] private Gun gun;

    #endregion

    #region PlayerUI Variables
    public int maxHealth;
    [HideInInspector] public bool isActive = false;

    [SerializeField] private Canvas playerCanvas;
    [SerializeField] private Slider otherHealthSlider;
    [SerializeField] private TextMeshProUGUI playerName;

    private int currHealth;
    #endregion

    #region Others Variables
    [Header("Others")]
    public Transform weaponContainer;
    public bool isHeadBob;

    private float headbobCounter;
    private Vector3 weaponContainerOrigin;
    private Vector3 targetWeaponContainerPos;

    #endregion

    #region Photon Variables
    [SerializeField] private int teamIndex = 0;
    #endregion

    [PunRPC]
    public void ActivatePlayer()
    {
        currHealth = maxHealth;
        playerCanvas.enabled = !photonView.IsMine;
        if (photonView.IsMine)
        {
            if (Camera.main)
            {
                Camera.main.enabled = false;
            }
            LevelUIManager.Instance.InitPlayerHealth(maxHealth, currHealth);
        }
        else
        {
            otherHealthSlider.maxValue = maxHealth;
            otherHealthSlider.value = maxHealth;

            ImportantThings.SetLayerRecursively(gameObject, 12);
        }

        cameraParent.SetActive(photonView.IsMine);

        baseFOV = normalCam.fieldOfView;

        myRB = GetComponent<Rigidbody>();

        weaponContainerOrigin = weaponContainer.localPosition;

        isActive = true;
    }

    private void Update()
    {
        if (isActive)
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
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            if (!photonView.IsMine) return;

            Move();
            Jump();
        }
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

    [PunRPC]
    public void TakeDamage(int damageValue)
    {
        currHealth -= damageValue;
        if (photonView.IsMine)
        {
            LevelUIManager.Instance.SetPlayerHealthValue(currHealth);
        }
        else
        {
            otherHealthSlider.value = currHealth;
        }


        if (currHealth <= 0)
        {
            print("you died");
        }
    }

    private void HeadBob(float z, float xIntensity, float yIntensity)
    {
        targetWeaponContainerPos = weaponContainerOrigin + new Vector3(Mathf.Cos(z) * xIntensity, Mathf.Sin(z * 2) * yIntensity, 0);
    }

    [PunRPC]
    public void SetTeamIndex(int mteamIndex)
    {
        teamIndex = mteamIndex;
    }

    public int GetTeamIndex()
    {
        return teamIndex;
    }

    [PunRPC]
    public void SetPlayerName(string name, int teamCanSeeIndex)
    {
        if (teamIndex != 0)
        {
            if (teamIndex != teamCanSeeIndex)
            {
                TogglePlayerName(false);
            }
            else if (teamIndex == teamCanSeeIndex)
            {
                TogglePlayerName(true);
                playerName.text = name;
            }
        }
        else
        {
            TogglePlayerName(false);
        }
    }

    public void TogglePlayerName(bool enabled)
    {
        playerName.gameObject.SetActive(enabled);
    }
}
