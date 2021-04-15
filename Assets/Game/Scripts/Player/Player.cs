using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
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

    public float shootTargetDistance = 0;
    public int maxAmmo;

    private Dictionary<string, bool> axisActive = new Dictionary<string, bool>();

    private bool switchingRelics = false;
    private bool paused = false;

    public Elements elementChanger = null;
    internal Dictionary<ElementTypes, int> Ammo = new Dictionary<ElementTypes, int>();

    public AudioMixerSnapshot unpausedSound, pausedSound;
    public AudioListener audioListener;
    public AudioSource audioSource;
    public string ammoChangeSound;
    public string jumpSound;
    public string landSound;
    public string damageSound;
    public string deathSound;
    public string enemyHitSound;

    public override void Awake()
    {
        base.Awake();
        if (elementChanger == null) elementChanger = weapon.GetComponent<Elements>();
        if (rotationScript == null) rotationScript = GetComponent<PlayerRotation>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (shootTargetDistance == 0) shootTargetDistance = shooter.range;

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

        if (audioListener) audioListener.enabled = SaveManager.GetBool("Muted");
        userInterface.HighlightSelectedAmmo();
        userInterface.ToggleSliderSelection(switchingRelics);
    }

    public override void Update()
    {
        bool landed = !isGrounded;
        base.Update();
        landed = landed && isGrounded;

        if (landed) sfxScript.PlaySFX3D(landSound, transform.position);
        Inputs();

        //If relic is in use
        if (currentRelic != null)
        {
            if (!currentRelic.readyToUse)
                userInterface.UpdateRelicTimer(currentRelic.cooldownTimer);
        }

        bool enemyScanned = false;
        if (Physics.Raycast(GetForwardRay(), out RaycastHit enemyScan, shootTargetDistance)) enemyScanned = enemyScan.transform.CompareTag("Enemy");

        userInterface.HighlightCrosshair(enemyScanned);
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
                bool newJump = !jumping;
                Jump();

                newJump = jumping && newJump;
                if (newJump) sfxScript.PlaySFX3D(jumpSound, transform.position);
            }


            if (Input.GetAxisRaw("Triggers") > 0)
            {
                if (!axisActive.ContainsKey("RightTrigger")) Shoot();
                else if (!axisActive["RightTrigger"]) Shoot();
                axisActive["RightTrigger"] = true;
            }

            else
            {
                axisActive["RightTrigger"] = false;
                if (Input.GetButtonDown("Fire1")) Shoot();
            }

            if (Input.GetButtonDown("Fire2"))
            {
                UseRelic();
            }

            if (Input.mouseScrollDelta.y != 0 || Input.GetButtonDown("ScrollUp") || Input.GetButtonDown("ScrollDown"))
            {
                int scrollAmount;
                if (Input.mouseScrollDelta.y != 0) scrollAmount = Mathf.FloorToInt(Input.mouseScrollDelta.y);
                else scrollAmount = Input.GetButtonDown("ScrollUp") ? 1 : -1;

                if (!switchingRelics)
                {
                    elementChanger.ChangeElement(scrollAmount);
                    userInterface.HighlightSelectedAmmo();
                    userInterface.UpdateAmmoCount(Ammo[elementChanger.m_CurElement]);
                }
                else ChangeRelic(scrollAmount);

                sfxScript.PlaySFX2D(ammoChangeSound);
            }

            if (Input.GetButtonDown("Toggle"))
            {
                switchingRelics = !switchingRelics;
                userInterface.ToggleSliderSelection(switchingRelics);
            }
        }

        if (Input.GetButtonDown("Pause") || Input.GetKeyDown(KeyCode.Escape)) TogglePause(false);
        if (Input.GetButtonDown("Select")) TogglePause(true);
        if (Input.GetKeyDown(KeyCode.M)) ToggleMute();
    }

    public void TogglePause(bool toolMenu)
    {
        bool checkUIOverlap;

        if (toolMenu)
            checkUIOverlap = userInterface.ShowToolBarMenu(elementChanger.m_CurElement, 
                (currentRelic != null) ? currentRelic.relicType : ElementTypes.ElementTypesSize);
        else
            checkUIOverlap = userInterface.PausePlay();

        //Prevents the player opening both panels at the same time.
        if (checkUIOverlap) ToggleInput();
    }

    public void ToggleInput(bool forcePause=false, bool newPause=false)
    {
        if (forcePause) paused = newPause;
        else paused = !paused;
        Time.timeScale = (paused) ? 0 : 1;
        rotationScript.SetCursorLock(!paused, paused);

        if (paused)
        {
            pausedSound.TransitionTo(0);
            sfxScript.PauseSFX();
        }
        else
        {
            unpausedSound.TransitionTo(0);
            sfxScript.UnPauseSFX();
        }
    }

    public void ToggleMute()
    {
        if (audioListener) audioListener.enabled = !audioListener.enabled;
        SaveManager.UpdateSavedBool("Muted", audioListener.enabled);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!movementLocked && !paused)
        {
            Vector3 newVelocity = (Input.GetAxisRaw("Horizontal") * transform.right + Input.GetAxisRaw("Vertical") * transform.forward).normalized * (runSpeed * speedMultiplier);

            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0 || !isGrounded) audioSource.Pause();
            else
            {
                audioSource.UnPause();
                audioSource.pitch = UnityEngine.Random.Range(0.8f, 1f); // play slightly different sounding footsteps
            }
            newVelocity.y = characterRigid.velocity.y;
            SetVelocity(newVelocity);
        }
    }

    /*_______________________________________________________________________________________________________________
    Relic use and shooting code
    _________________________________________________________________________________________________________________*/
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out RelicBase newRelic))
        {
            AddRelic(other.gameObject, true);
            userInterface.ShowRelicPopUp(newRelic.relicType.ToString());
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.gameObject.TryGetComponent(out ElementHazardAilments effectStats))
        {
            if (effectStats.team != team)
            {
                if (effectStats.damageType == ElementTypes.Air)
                    Shift((transform.position - effectStats.spawnPoint).normalized * effectStats.statusMagnitude, effectStats.statusEffectDuration, (1 - knockbackRecovery), 1, true);
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

    public override void IncreaseCombo()
    {
        base.IncreaseCombo();
        userInterface.ShowHit(hitCombo);
        sfxScript.PlaySFX2D(enemyHitSound);
    }

    public override void MissShot(float bulletDamage)
    {
        base.MissShot(bulletDamage);

        userInterface.EndCombo();
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

        if (!immortal)
        {
            sfxScript.PlaySFX2D(damageSound);
            userInterface.UpdateHealth(curHealth, true);
        }
    }

    public override void Immortalise()
    {
        base.Immortalise();
        userInterface.SetInvulnerable();
    }

    public override void Mortalise()
    {
        base.Mortalise();
        userInterface.EndInvulnerable();
    }

    public override void Die()
    {
        base.Die();

        paused = true;
        audioSource.Pause();
        rotationScript.SetCursorLock(false, true);
        sfxScript.PlaySFX2D(deathSound);
        userInterface.ShowEndGame(true);
    }

    public void CompleteGame()
    {
        paused = true;
        audioSource.Pause();
        rotationScript.SetCursorLock(false, true);
        userInterface.ShowEndGame(false);
    }

    public override bool AddHealth(float value, int cost=0, ElementTypes costType=ElementTypes.ElementTypesSize)
    {
        if ((costType == ElementTypes.ElementTypesSize) ? true : Ammo[costType] >= cost)
        {
            if (!base.AddHealth(value)) return false;
            if (costType != ElementTypes.ElementTypesSize) SubstractAmmo(cost, costType);
            userInterface.UpdateHealth(curHealth);
            return true;
        }
        
        return false;
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

    public bool AddAmmo(int value, ElementTypes type)
    {
        if (Ammo[type] >= maxAmmo) return false;
        Ammo[type] += value;

        if (Ammo[type] > maxAmmo)
        {
            Ammo[type] = maxAmmo;
        }

        if (elementChanger.m_CurElement == type) userInterface.UpdateAmmoCount(Ammo[type]);

        return true;
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
    public override void AddRelic(GameObject newRelic, bool playSound=false)
    {
        base.AddRelic(newRelic, playSound);
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
        SaveManager.UpdateSavedQuaternion(saveID + "PlayerRot", transform.localRotation);
        SaveManager.UpdateSavedQuaternion(saveID + "PlayerCameraRot", firstPersonCamera.transform.localRotation);

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
                transform.localRotation = SaveManager.GetQuaternion(loadTransform + "PlayerRot");
                transform.rotation = SaveManager.GetQuaternion(loadTransform + "PlayerRot");
                firstPersonCamera.transform.localRotation = SaveManager.GetQuaternion(loadTransform + "PlayerCameraRot");
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
        rotationScript.SetCursorLock(true);
        paused = false;
        characterRigid.velocity = Vector3.zero;
        if (shifting) EndShift();
    }
    #endregion
}