using System;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;
using static SaveManager;

public class Player : CharacterBase
{
    public GameObject canvas;
    public Vector3 crosshairPos;
    public Camera firstPersonCamera;
    public GUI_Manager userInterface = null;

    public PlayerRotation rotationScript = null;

    public float runSpeed;
    public float shootTargetDistance;
    public int maxAmmo;

    private bool switchingRelics = false;
    private bool paused = false;

    public Elements elementChanger = null;
    internal Dictionary<ElementTypes, int> Ammo = new Dictionary<ElementTypes, int>();

    public override void Awake()
    {
        base.Awake();
        if (elementChanger == null) elementChanger = weapon.GetComponent<Elements>();
        if (rotationScript == null) rotationScript = GetComponent<PlayerRotation>();
        /*______________________________________________________________________________________
        User Interface  initialisation
        ________________________________________________________________________________________*/

        if (userInterface == null) userInterface = canvas.GetComponent<GUI_Manager>();
        userInterface.SetMaxHealth(maxHealth);
        userInterface.SetMaxAmmo(maxAmmo);
        //______________________________________________________________________________________
        
        if (Ammo.Count < (int)ElementTypes.ElementTypesSize)
        {
            for (int ammo = Ammo.Count; ammo < (int)ElementTypes.ElementTypesSize; ammo++)
            {
                Ammo.Add((ElementTypes)0 + ammo, maxAmmo);
            }
        }
    }

    public override void Start()
    {
        base.Start();
        team = Teams.Player;

        userInterface.HighlightSelectedAmmo();
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
        if (!paused)
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
                    userInterface.HighlightSelectedAmmo();
                    userInterface.UpdateAmmoCount(Ammo[elementChanger.m_CurElement]);
                }
                else ChangeRelic(Mathf.FloorToInt(Input.mouseScrollDelta.y));
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                switchingRelics = !switchingRelics;
            }
        }

        if (Input.GetKeyDown(KeyCode.P)) TogglePause(false);
        if (Input.GetKeyDown(KeyCode.Q)) TogglePause(true);
    }

    public void TogglePause(bool toolMenu)
    {
        bool checkUIOverlap = false;

        if (toolMenu)
            checkUIOverlap = userInterface.ShowToolBarMenu();
        else
            checkUIOverlap = userInterface.PausePlay();

        //Prevents the player opening both panels at the same time.
        if (checkUIOverlap)
        {
            paused = !paused;

            Time.timeScale = (paused) ? 0 : 1;
            rotationScript.SetCursorLock(!paused, paused);
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

        if (collision.gameObject.TryGetComponent(out ElementHazardAilments effectStats))
        {
            if (effectStats.team != team)
            {
                if (effectStats.damageType == ElementTypes.Air)
                    Shift((effectStats.gameObject.transform.position - transform.position).normalized * effectStats.statusMagnitude, effectStats.statusEffectDuration, (1 - knockbackRecovery), 1, true);
            } 
        }
    }

    private void Shoot()
    {
        if (AmmoCheck())
        {
            shooter.Shoot(elementChanger.m_CurElement, (firstPersonCamera.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * shootTargetDistance)));
            SubstractAmmo(1, elementChanger.m_CurElement);
        }
    }

    public override Ray GetForwardRay()
    {
        return firstPersonCamera.ViewportPointToRay(crosshairPos);
    }

    /*_______________________________________________________________________________________________________________
    Player health code
    _________________________________________________________________________________________________________________*/

    public override void TakeDamage(float damage, ElementTypes damageType=ElementTypes.ElementTypesSize)
    {
        base.TakeDamage(damage);

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

        if (elementChanger.m_CurElement == type) userInterface.UpdateAmmoCount(Ammo[type]);
    }

    public void AddAmmo(int value, ElementTypes type)
    {
        Ammo[type] += value;

        if (Ammo[type] > maxAmmo)
        {
            Ammo[type] = maxAmmo;
        }

        if (elementChanger.m_CurElement == type) userInterface.UpdateAmmoCount(Ammo[type]);
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

    /*_______________________________________________________________________________________________________________
    Relic override code
    _________________________________________________________________________________________________________________*/
    public override void AddRelic(GameObject newRelic)
    {
        base.AddRelic(newRelic);
        //Resets the relic 
        userInterface.RefillRelicTimer(currentRelic.relicCooldownDuration);
    }

    public override void ChangeRelic(int cycleAmount = 1)
    {
        base.ChangeRelic(cycleAmount);
        //Reset timer max in UI
        if (currentRelic) userInterface.RefillRelicTimer(currentRelic.relicCooldownDuration);

        //call function for new relic icon
    }


    #region Saving
    public void SaveStats(string saveID)
    {
        SaveManager.UpdateSavedVector3(saveID + "PlayerPos", transform.position);
        SaveManager.UpdateSavedVector3(saveID + "PlayerRot", transform.localRotation.eulerAngles);
        SaveManager.UpdateSavedVector3(saveID + "PlayerCameraRot", firstPersonCamera.transform.localRotation.eulerAngles);

        foreach (KeyValuePair<ElementTypes, int> ammoPair in Ammo)
        {
            SaveManager.UpdateSavedInt(saveID + "Player" + ammoPair.Key.ToString() + "Ammo", ammoPair.Value);
        }

        SaveManager.UpdateSavedElementType(saveID + "PlayerElement", elementChanger.m_CurElement);
        SaveManager.UpdateSavedInt(saveID + "PlayerRelicIndex", relicIndex);

        SaveManager.UpdateSavedBool("PlayerSaved", true);
    }

    public void LoadStats(string loadID, string loadTransform="")
    {
        if (SaveManager.HasBool("PlayerSaved"))
        {
            if (loadTransform != "")
            {
                transform.position = SaveManager.GetVector3(loadTransform + "PlayerPos");
                transform.Rotate(SaveManager.GetVector3(loadTransform + "PlayerRot"), Space.Self);
                firstPersonCamera.transform.Rotate(SaveManager.GetVector3(loadTransform + "PlayerCameraRot"), Space.Self);
            }

            Ammo[ElementTypes.Fire] = SaveManager.GetInt(loadID + "Player" + ElementTypes.Fire.ToString() + "Ammo");
            Ammo[ElementTypes.Water] = SaveManager.GetInt(loadID + "Player" + ElementTypes.Water.ToString() + "Ammo");
            Ammo[ElementTypes.Air] = SaveManager.GetInt(loadID + "Player" + ElementTypes.Air.ToString() + "Ammo");
            Ammo[ElementTypes.Earth] = SaveManager.GetInt(loadID + "Player" + ElementTypes.Earth.ToString() + "Ammo");
            
            elementChanger.SetElement(SaveManager.GetElementType(loadID + "PlayerElement"));
            userInterface.HighlightSelectedAmmo();
            userInterface.UpdateAmmoCount(Ammo[elementChanger.m_CurElement]);

            relicIndex = Mathf.Clamp(SaveManager.GetInt(loadID + "PlayerRelicIndex"), 0, relicList.Count - 1);
            SetRelic(relicIndex);

            movementLocked = false;
        }

        curHealth = maxHealth;
        userInterface.UpdateHealth(curHealth);
        killed = false;
    }
    #endregion
}