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
    public Vector3 crosshairPos;
    public Camera firstPersonCamera;
    public GUI_Manager userInterface;

    public float runSpeed;
    public float shootTargetDistance;
    public int maxAmmo;

    private bool switchingRelics = false;
    private UIManager UIScript;

    internal Elements elementChanger;
    internal Dictionary<ElementTypes, int> Ammo = new Dictionary<ElementTypes, int>();

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
        // UIScript.UpdateElementText(elementChanger.m_CurElement, Ammo[elementChanger.m_CurElement], true);

        /*______________________________________________________________________________________
        User Interface  initialisation
        ________________________________________________________________________________________*/
        userInterface.SetMaxHealth(maxHealth);
        userInterface.SetMaxAmmo(maxAmmo);
        //______________________________________________________________________________________
    }

    public override void Update()
    {
        base.Update();

        Inputs();

        //If relic is in use
        if (currentRelic != null)
        {
            if (!currentRelic.readyToUse)
                userInterface.UpdateRelicTimer(currentRelic.cooldownTimer);
        }
    }
    /*_______________________________________________________________________________________________________________
    Player movement code
    _________________________________________________________________________________________________________________*/

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
            if (!switchingRelics)
            {
                elementChanger.ChangeElement(Mathf.FloorToInt(Input.mouseScrollDelta.y));
                userInterface.UpdateAmmoCount(Ammo[elementChanger.m_CurElement]);
            }
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

    /*_______________________________________________________________________________________________________________
    Relic use and shooting code
    _________________________________________________________________________________________________________________*/
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

    /*_______________________________________________________________________________________________________________
    Player health code
    _________________________________________________________________________________________________________________*/

    public override void TakeDamage(float damage, ElementTypes damageType=ElementTypes.ElementTypesSize)
    {
        base.TakeDamage(damage);

        if (curHealth <= 0)
        {
            curHealth = 0;
            Respawn();
        }

        //UIScript.UpdateHealthText((int)curHealth);

        userInterface.UpdateHealth(curHealth);
    }

    public override void AddHealth(float value, int cost=0, ElementTypes costType=ElementTypes.ElementTypesSize)
    {
        if (cost == 0) base.AddHealth(value);
        else if (Ammo[costType] >= cost)
        {
            base.AddHealth(value);
            SubstractAmmo(cost, costType);
        }

        //UIScript.UpdateHealthText((int)curHealth);
        userInterface.UpdateHealth(curHealth);
    }

    /*_______________________________________________________________________________________________________________
    Ammo code
    _________________________________________________________________________________________________________________*/
    public void SubstractAmmo(int value, ElementTypes type)
    {
        Ammo[type] -= value;

        if (Ammo[type] < 0)
        {
            Ammo[type] = 0;
        }

        userInterface.UpdateAmmoCount(Ammo[type]);
        //if (elementChanger.m_CurElement == type) UIScript.UpdateElementText(elementChanger.m_CurElement, Ammo[type]);
    }

    public void AddAmmo(int value, ElementTypes type)
    {
        Ammo[type] += value;

        if (Ammo[type] > maxAmmo)
        {
            Ammo[type] = maxAmmo;
        }

        userInterface.UpdateAmmoCount(Ammo[type]);
        //if (elementChanger.m_CurElement == type) UIScript.UpdateElementText(elementChanger.m_CurElement, Ammo[type]);
    }

    public bool AmmoCheck(int requiredAmount = 1)
    {
        return Ammo[elementChanger.m_CurElement] >= requiredAmount;
    }

    //_______________________________________________________________________________________________________________

    public override bool IsPlayer()
    {
        return true;
    }

    public void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /*_______________________________________________________________________________________________________________
    Relic override code
    _________________________________________________________________________________________________________________*/
    public override void AddRelic(GameObject newRelic)
    {
        base.AddRelic(newRelic);
        //Resets the relic 
        userInterface.RefillRelicTimer(currentRelic.relicCooldownDuration);
    }

    //public override void ChangeRelic(int cycleAmount = 1)
    //{
    //    base.ChangeRelic(cycleAmount);
    //    //call function for new relic icon
    //}
}