/// <summary>
/// 
/// Script made by Daniel and Daniel
/// 
/// Originally we had two scripts, this
/// one for player inputs, shooting and
/// using tools which was made primarily
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

public class Player : MonoBehaviour
{
    public GameObject canvas;
    private UIManager UIScript;

    public Camera firstPersonCamera;

    private Rigidbody playerRigid;

    public float runSpeed;
    public float jumpForce;
    public float gravityMult;
    public float floorDistance;

    private bool hasJumpedTwice;
    internal bool isGrounded = false;
    internal bool movementLocked = false;
    public float airControl;

    internal List<ToolBase> toolList = new List<ToolBase>();
    internal ToolBase currentTool = null;
    private int toolIndex = 0;

    public GameObject weapon;
    internal int hitCombo = 0;

    private bool switchingTools = false;

    internal ElementShooting shooter;
    internal Elements elementChanger;

    internal Dictionary<ElementTypes, int> Ammo = new Dictionary<ElementTypes, int>();

    public int m_MaxAmmo;

    private const float m_MaxHealth = 100.0f;
    private float m_CurHealth;

    private float toolTimer = 0;
    public float toolCooldownDuration;
    internal bool isToolAvailable;

    private int maxCombo = 1;
    private float percentIncreasePerHit = 0;
    private float damagePercentRecievedOnMiss = 0;

    private bool doubleJumpEnabled = false;
    private float knockBackMultiplier = 1;

    private float damageRecievedMultiplier = 1;
    private float speedMultiplier = 1;

    private void Start()
    {
        playerRigid = GetComponent<Rigidbody>();

        airControl = Mathf.Clamp(airControl, 0, 1);

        isToolAvailable = false;
        shooter = weapon.GetComponent<ElementShooting>();
        elementChanger = weapon.GetComponent<Elements>();

        m_CurHealth = m_MaxHealth;
        for (int ammo = 0; ammo < (int)ElementTypes.ElementTypesSize; ammo++)
        {
            Ammo.Add((ElementTypes)0 + ammo, m_MaxAmmo);
        }


        UIScript = canvas.GetComponent<UIManager>();

        UIScript.UpdateElementText(elementChanger.m_CurElement, Ammo[elementChanger.m_CurElement], true);
    }

    void Update()
    {
        Inputs();
        CheckGround();
        ToolCooldown();
    }

    private void Inputs()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            shooter.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            UseTool();
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            if (!switchingTools) elementChanger.ChangeElement(Mathf.FloorToInt(Input.mouseScrollDelta.y));
            else ChangeTool(Mathf.FloorToInt(Input.mouseScrollDelta.y));
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            switchingTools = !switchingTools;
        }
    }

    #region Movement

    private void FixedUpdate()
    {
        if (!movementLocked)
        {
            Vector3 newVelocity = Vector3.zero;
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            float currentSpeed = runSpeed * speedMultiplier;

            if (horizontal != 0)
            {
                newVelocity += ((horizontal > 0) ? transform.right : -transform.right) * currentSpeed;
            }

            if (vertical != 0)
            {
                newVelocity += ((vertical > 0) ? transform.forward : -transform.forward) * currentSpeed;
            }
            newVelocity.y = playerRigid.velocity.y;

            if (!isGrounded) newVelocity = Vector3.Lerp(playerRigid.velocity, newVelocity, airControl);

            playerRigid.velocity = newVelocity;

            playerRigid.AddForce(Physics.gravity * (gravityMult - 1), ForceMode.Acceleration);
        }
    }

    private void CheckGround()
    {
        RaycastHit[] floorHits = Physics.RaycastAll(new Ray(transform.position, -Vector3.up), floorDistance);
        isGrounded = false;
        foreach (RaycastHit floorHit in floorHits)
        {
            if (floorHit.transform.CompareTag("Floor"))
            {
                isGrounded = true;
                break;
            }
        }
    }

    private void Jump()
    {
        if (isGrounded || doubleJumpEnabled && !hasJumpedTwice)
        {
            playerRigid.AddForce(Vector3.up * jumpForce);

            hasJumpedTwice = !isGrounded;
        }
    }

    public void KnockBack(Vector3 force)
    {
        playerRigid.AddForce(force * knockBackMultiplier);
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (currentTool != null)
        {
            if (currentTool.toolType == ElementTypes.Air && currentTool.inUse) currentTool.EndAbility();
        }
    }

    #region StatManagement
    public void TakeDamage(float value)
    {
        m_CurHealth -= value * damageRecievedMultiplier;

        if (m_CurHealth <= 0)
        {
            m_CurHealth = 0;
            Respawn();
        }

        UIScript.UpdateHealthText((int)m_CurHealth);
    }

    public void AddHealth(float value)
    {
        m_CurHealth += value;

        if (m_CurHealth > m_MaxHealth)
        {
            m_CurHealth = m_MaxHealth;
        }

        UIScript.UpdateHealthText((int)m_CurHealth);
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

        if (Ammo[type] > m_MaxAmmo)
        {
            Ammo[type] = m_MaxAmmo;
        }
        
        if (elementChanger.m_CurElement == type) UIScript.UpdateElementText(elementChanger.m_CurElement, Ammo[type]);
    }

    public float CalculateDamageMult()
    {
        return 1 + Mathf.Clamp(hitCombo, 1, maxCombo) * percentIncreasePerHit / 100;
    }

    public void IncreaseCombo()
    {
        if (hitCombo < maxCombo) hitCombo++;
    }

    public void MissShot(float bulletDamage)
    {
        TakeDamage(bulletDamage * (1 + damagePercentRecievedOnMiss / 100));
        hitCombo = 0;
    }

    public bool AmmoCheck()
    {
        return Ammo[elementChanger.m_CurElement] > 0;
    }

    public void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    #region Tool
    private void UseTool()
    {
        //if (currentTool.inUse) currentTool.EndAbility();

        if (isToolAvailable)
        {
            if (currentTool.Activate())
            {
                isToolAvailable = false;
            }
        }
    }

    private void ToolCooldown()
    {
        if (!isToolAvailable && currentTool != null)
        {
            toolTimer += Time.deltaTime;

            if (toolTimer > toolCooldownDuration)
            {
                isToolAvailable = true;
                toolTimer = 0;
            }
        }
    }

    private void ChangeTool(int mouseScroll)
    {
        if (mouseScroll != toolList.Count && toolList.Count > 0)
        {
            currentTool.EndAbility();
            currentTool.gameObject.SetActive(false);
            toolIndex += mouseScroll;

            if (toolIndex >= toolList.Count)
            {
                toolIndex -= toolList.Count;
            }
            else if (toolIndex < 0)
            {
                toolIndex += toolList.Count;
            }

            currentTool = toolList[toolIndex];
            currentTool.gameObject.SetActive(true);

            ActivatePassives();
        }
    }

    public void ActivatePassives()
    {
        maxCombo = currentTool.maxCombo;
        percentIncreasePerHit = currentTool.percentIncreasePerHit;
        damagePercentRecievedOnMiss = currentTool.damagePercentRecievedOnMiss;

        doubleJumpEnabled = currentTool.doubleJumpEnabled;
        knockBackMultiplier = currentTool.knockBackMultiplier;

        damageRecievedMultiplier = currentTool.damageRecievedMultiplier;
        speedMultiplier = currentTool.speedMultiplier;

        hitCombo = 0;
}
    #endregion
}