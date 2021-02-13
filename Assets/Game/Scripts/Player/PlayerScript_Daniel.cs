/*
 * DANIEL BIBBY
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript_Daniel : MonoBehaviour
{
    public enum Tools
    {
        noTool,
        crystalBall
    }


    public GameObject BulletFire;
    public GameObject BulletWater;
    public GameObject BulletEarth;
    public GameObject BulletAir;
    public GameObject GunPos;
    private Rigidbody rb;
    private CharacterController characterController;

    public float ShootForce;
    public float TargetDistance;

    private GameObject ChosenBullet;

    public Tools Tool;
    public float DashSpeed;

    private Vector3 dashDist;
    private float dashTimer = 0;
    public float dashDuration;
    private bool isDashing;

    private float toolTimer = 0;
    public float toolDuration;
    private bool isToolAvailable;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckElement();
            GameObject newBullet = Instantiate(ChosenBullet, GunPos.transform);
            newBullet.GetComponent<Rigidbody>().AddForce((Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward*TargetDistance) - GunPos.transform.position).normalized * ShootForce);
            newBullet.transform.SetParent(null);
            Destroy(newBullet, 2);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UseTool();
        }

        if (isDashing)
        {
            dashTimer += Time.deltaTime;

            characterController.Move(dashDist * Time.deltaTime);

            if (dashTimer > dashDuration)
            {
                isDashing = false;
                dashTimer = 0;
            }
        }

        if (!isToolAvailable)
        {
            toolTimer += Time.deltaTime;

            if (toolTimer > toolDuration)
            {
                isToolAvailable = true;
                toolTimer = 0;
            }
        }
    }

    void CheckElement()
    {
        switch(GetComponent<ElementsScript_MattNDaniel>().m_CurElement)
        {
            case ElementsScript_MattNDaniel.Elements.Fire:
                ChosenBullet = BulletFire;
                break;
            case ElementsScript_MattNDaniel.Elements.Water:
                ChosenBullet = BulletWater;
                break;
            case ElementsScript_MattNDaniel.Elements.Air:
                ChosenBullet = BulletAir;
                break;
            case ElementsScript_MattNDaniel.Elements.Earth:
                ChosenBullet = BulletEarth;
                break;
        }
    }

    private void UseTool()
    {
        if (isToolAvailable)
        {
            if (Tool == Tools.crystalBall)
            {
                CrystalBallTool();
            }

            isToolAvailable = false;
        }
    }

    private void CrystalBallTool()
    {
        isDashing = true;

        dashDist = characterController.velocity.normalized * DashSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == ("Tool"))
        {
            Tool = Tools.crystalBall;
            other.gameObject.SetActive(false);
        }
    }
}
