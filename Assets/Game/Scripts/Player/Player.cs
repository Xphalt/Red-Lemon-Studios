/// <summary>
/// 
/// Script made by Daniel and Daniel
/// 
/// Originally we had two scripts, this
/// one for player inputs, shooting and
/// using relics which was made primarily
/// by Daniel with Linden making a few
/// changes.
/// 
/// The other script was called 
/// "PlayerAttributes" and was created 
/// by Matt which contained the health
/// and ammo along with ways to increase
/// and decrease health, lose and gain ammo.
/// 
/// Me and Daniel kept Matts code and added
/// and refactored it into this script to
/// keep everything related to the player
/// together and to help with reusablillty.
/// 
/// </summary> 

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using static EnumHelper;
using static SaveManager;

public class Player : CharacterBase
{
    public GameObject canvas;
    private UIManager UIScript;

    public Vector3 crosshairPos;

    public Camera firstPersonCamera;
    public float runSpeed;
    public float shootTargetDistance;

    private bool switchingRelics = false;

    internal Elements elementChanger;

    internal Dictionary<ElementTypes, int> Ammo = new Dictionary<ElementTypes, int>();

    public int maxAmmo;


    public override void Start()
    {
        base.Start();

        team = Teams.Player;
        elementChanger = weapon.GetComponent<Elements>();

        for (int ammo = 0; ammo < (int)ElementTypes.ElementTypesSize; ammo++)
        {
            Ammo.Add((ElementTypes)0 + ammo, maxAmmo);
        }

        UIScript = canvas.GetComponent<UIManager>();
        UIScript.UpdateElementText(elementChanger.m_CurElement, Ammo[elementChanger.m_CurElement], true);
    }

    public override void Update()
    {
        base.Update();

        Inputs();
    }

    private void Inputs()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            UseRelic();
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            if (!switchingRelics) elementChanger.ChangeElement(Mathf.FloorToInt(Input.mouseScrollDelta.y));
            else ChangeRelic(Mathf.FloorToInt(Input.mouseScrollDelta.y));
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            switchingRelics = !switchingRelics;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!movementLocked)
        {
            Vector3 newVelocity = (Input.GetAxisRaw("Horizontal") * transform.right + Input.GetAxisRaw("Vertical") * transform.forward).normalized * (runSpeed * speedMultiplier);

            newVelocity.y = characterRigid.velocity.y;
            SetVelocity(newVelocity);

            characterRigid.AddForce(Physics.gravity * (gravityMult - 1), ForceMode.Acceleration);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Relic")) AddRelic(other.gameObject);
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.gameObject.CompareTag("Hazard"))
        {
            ElementHazardAilments effectStats = collision.gameObject.GetComponent<ElementHazardAilments>();

            if (effectStats.damageType == ElementTypes.Air)
                Shift((effectStats.gameObject.transform.position - transform.position).normalized * effectStats.statusMagnitude, effectStats.statusEffectDuration, (1 - knockbackRecovery), 1, true);
        }
    }

    private void Shoot()
    {
        if (AmmoCheck())
        {
            shooter.Shoot(elementChanger.m_CurElement, (Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * shootTargetDistance)));
            SubstractAmmo(1, elementChanger.m_CurElement);
        }
    }

    public override Ray GetForwardRay()
    {
        return Camera.main.ViewportPointToRay(crosshairPos);
    }

    public override void TakeDamage(float damage, ElementTypes damageType=ElementTypes.ElementTypesSize)
    {
        base.TakeDamage(damage);

        if (curHealth <= 0)
        {
            curHealth = 0;
            Respawn();
        }

        UIScript.UpdateHealthText((int)curHealth);
    }

    public override void AddHealth(float value, int cost=0, ElementTypes costType=ElementTypes.ElementTypesSize)
    {
        if (cost == 0) base.AddHealth(value);
        else if (Ammo[costType] >= cost)
        {
            base.AddHealth(value);
            SubstractAmmo(cost, costType);
        }

        UIScript.UpdateHealthText((int)curHealth);
    }

    public void SubstractAmmo(int value, ElementTypes type)
    {
        Ammo[type] -= value;

        if (Ammo[type] < 0)
        {
            Ammo[type] = 0;
        }

        if (elementChanger.m_CurElement == type) UIScript.UpdateElementText(elementChanger.m_CurElement, Ammo[type]);
    }

    public void AddAmmo(int value, ElementTypes type)
    {
        Ammo[type] += value;

        if (Ammo[type] > maxAmmo)
        {
            Ammo[type] = maxAmmo;
        }
        
        if (elementChanger.m_CurElement == type) UIScript.UpdateElementText(elementChanger.m_CurElement, Ammo[type]);
    }

    public bool AmmoCheck(int requiredAmount = 1)
    {
        return Ammo[elementChanger.m_CurElement] >= requiredAmount;
    }

    public override bool IsPlayer()
    {
        return true;
    }

    public void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}