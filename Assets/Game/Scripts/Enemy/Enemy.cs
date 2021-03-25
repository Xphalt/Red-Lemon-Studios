/// <summary>
/// 
/// Script made by Zack
/// 
/// Linden added enemy death
/// when their health reaches 0
/// 
/// Daniel added enemies being
/// effected by ailments from
/// elemental ammo
/// 
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class Enemy : CharacterBase
{
    public enum EnemyStates { Idle, Chasing, Patrolling, Fleeing, EnemyStatesSize };

    public Animator myAnim = null;
    public GameObject target = null;
    public ElementTypes elementType;
    protected ElementTypes weakAgainst;
    protected ElementTypes strongAgainst;

    public GameObject ammoDrop;
    private PickUpBase dropScript;
    public float dropChance;

    protected Player playerScript;

    public float strongAgainstResist = 0.7f;
    public float weakAgainstIncrease = 1.5f;

    public float defaultColourChangeDuration = 0.1f;
    private float colourChangeDuration;
    private float colourChangeTimer = 0;
    private bool colourChanged = false;

    public List<SkinnedMeshRenderer> skins;
    public List<MeshRenderer> meshes;
    protected List<List<Color>> originalSkinColours = new List<List<Color>>();
    protected List<List<Color>> originalMeshColours = new List<List<Color>>();

    public float attackInterval;
    protected float attackTimer = 0;

    protected EnemyStates movementState;
    protected bool runTooFar;
    public bool sentryMode = false;

    internal bool spawned = false;

    public float chaseSpeed = 5;
    public float patrolSpeed = 2;
    public float playerDetectionRadius = 50;
    public float wallDetectionRadius = 5;

    protected float statusDuration;
    protected float statusTimer;
    protected bool statusEffectActive;
    protected float statusMagnitude;

    protected float DOTTimer;
    protected float DOTInterval = 1; //Placeholder. Not sure how it will be implemented long-term 

    public override void Awake()
    {
        base.Awake();
        if (target == null) target = GameObject.FindGameObjectWithTag("Player");
        playerScript = target.GetComponent<Player>();
        dropScript = ammoDrop.GetComponent<PickUpBase>();
        dropChance = Mathf.Clamp(dropChance, 0, 1);

        if (myAnim == null) myAnim = gameObject.GetComponent<Animator>();

        if (skins.Count == 0)
        {
            foreach (SkinnedMeshRenderer newSkin in gameObject.transform.GetComponentsInChildren<SkinnedMeshRenderer>()) skins.Add(newSkin);
        }

        for (int s = 0; s < skins.Count; s++)
        {
            originalSkinColours.Add(new List<Color>());
            foreach (Material skinmat in skins[s].materials) originalSkinColours[s].Add(skinmat.color);
        }

        for (int mr = 0; mr < meshes.Count; mr++)
        {
            originalMeshColours.Add(new List<Color>());
            foreach (Material meshmat in meshes[mr].materials) originalMeshColours[mr].Add(meshmat.color);
        }
    }

    public override void Start()
    {
        base.Start();
        team = Teams.Enemy;

        if (sentryMode) movementState = EnemyStates.Idle;
        else characterRigid.velocity = transform.forward * chaseSpeed;

        gameObject.SetActive(spawned && !killed);
    }

    public override void Update()
    {
        base.Update();

        attackTimer += Time.deltaTime;

        if (statusEffectActive)
        {
            statusTimer += Time.deltaTime;
            if (statusTimer > statusDuration)
            {
                EndSatusEffect();
            }
        }

        if (colourChanged)
        {
            colourChangeTimer += Time.deltaTime;
            if (colourChangeTimer > colourChangeDuration)
            {
                for (int s = 0; s < skins.Count; s++)
                {
                    for (int m = 0; m < skins[s].materials.Length; m++) skins[s].materials[m].color = originalSkinColours[s][m];
                }
                for (int mr = 0; mr < meshes.Count; mr++)
                {
                    for (int m = 0; m < meshes[mr].materials.Length; m++) meshes[mr].materials[m].color = originalMeshColours[mr][m];
                }
                colourChanged = false;
                colourChangeTimer = 0;
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        bool directionSet = false;
        if (!movementLocked)
        {
            Vector3 newVelocity = characterRigid.velocity;
            switch (movementState)
            {
                case EnemyStates.Chasing:
                    runTooFar = false;
                    newVelocity = (target.transform.position - transform.position).normalized * chaseSpeed;
                    break;

                case EnemyStates.Fleeing:
                    runTooFar = false;
                    newVelocity = (transform.position - target.transform.position).normalized * chaseSpeed;
                    if (isGrounded && !canFly)
                    {
                        if (CheckObstruction(newVelocity))
                        {
                            runTooFar = true;
                            newVelocity = Vector3.zero;
                            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
                            directionSet = true;
                        }
                    }
                    break;

                case EnemyStates.Patrolling:
                    runTooFar = false;
                    Vector3 patrolDirection = characterRigid.velocity;
                    patrolDirection.y = 0;

                    patrolDirection.Normalize();

                    if (patrolDirection == Vector3.zero) patrolDirection = transform.forward;

                    if (CheckObstruction(patrolDirection))
                    {
                        newVelocity = Vector3.Cross(patrolDirection, Vector3.up).normalized * patrolSpeed;
                        if (Random.Range(0, 2) == 1) newVelocity *= -1;
                    }
                    else newVelocity = patrolDirection * patrolSpeed;

                    break;

                case EnemyStates.Idle:
                    newVelocity = Vector3.zero;
                    break;
            }

            if (!canFly) newVelocity.y = characterRigid.velocity.y;

            SetVelocity(newVelocity);
        }

        if (!directionSet)
        {
            Vector3 lookDirection = characterRigid.velocity - Vector3.up * characterRigid.velocity.y; //Set Y component to 0
            if (lookDirection != Vector3.zero) transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

    public bool CheckObstruction(Vector3 patrolDirection)
    {
        bool obstruction = Physics.Raycast(transform.position, patrolDirection, wallDetectionRadius);
        if (!obstruction && isGrounded && !canFly)
            obstruction = !Physics.Raycast(transform.position + patrolDirection * wallDetectionRadius, Vector3.down, floorDistance); // Fire enemy is spesh but oh well
        
        return obstruction;
    }

    public bool CanSeePlayer()
    {
        if (!Physics.Raycast(transform.position, (target.transform.position - transform.position), out RaycastHit castHit, playerDetectionRadius)) return false;

        if (castHit.transform.gameObject == target)
        {
            sentryMode = false;
            return true;
        }

        return false;
    }

    public float GetDistance()
    {
        return (target.transform.position - transform.position).magnitude;
    }

    public virtual bool Attack()
    {
        if (attackTimer < attackInterval || !CanSeePlayer()) return false;

        attackTimer = 0;
        return true;
    }

    public virtual void Animate()
    {
        //currently used as a placeholder
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.gameObject.TryGetComponent(out ElementHazardAilments effectStats))
        {
            ChangeColour(effectStats);
            if (effectStats.hasEffect && effectStats.damageType == weakAgainst && effectStats.team != team)
            {
                TriggerStatusEffect(effectStats);
            }
        }
    }

    public void ChangeColour(ElementHazardAilments effectStats)
    {
        if (effectStats.changesColour)
        {
            Color newColour;
            colourChanged = true;
            colourChangeTimer = 0;

            if (effectStats.damageType == weakAgainst)
            {
                newColour = effectStats.weakHitColour;
                colourChangeDuration = (effectStats.hasEffect && effectStats.statusEffectDuration != 0) ? effectStats.statusEffectDuration : defaultColourChangeDuration;
            }
            else
            {
                colourChangeDuration = defaultColourChangeDuration;
                newColour = (effectStats.damageType == strongAgainst) ? effectStats.strongHitColour : effectStats.normalHitColour;
            }

            foreach (SkinnedMeshRenderer skin in skins)
            {
                for (int m = 0; m < skin.materials.Length; m++) skin.materials[m].color = newColour;
            }
            foreach (MeshRenderer mesh in meshes)
            {
                for (int m = 0; m < mesh.materials.Length; m++) mesh.materials[m].color = newColour;
            }
        }
    }

    public override void TakeDamage(float damage, ElementTypes damageType = ElementTypes.ElementTypesSize)
    {

        if (damageType == weakAgainst) damage *= weakAgainstIncrease;
        else if (damageType == strongAgainst) damage *= strongAgainstResist;

        base.TakeDamage(damage, damageType);
    }

    public override void Die()
    {
        base.Die();

        if (Random.value < dropChance) dropScript.Spawn();
        gameObject.SetActive(false);
    }

    public virtual void TriggerStatusEffect(ElementHazardAilments effectStats) 
    {
        statusEffectActive = true;
        statusTimer = 0;
        statusDuration = effectStats.statusEffectDuration;
        statusMagnitude = effectStats.statusMagnitude;
    }

    public virtual void EndSatusEffect()
    {
        statusEffectActive = false;
        statusTimer = 0;
        statusMagnitude = 0;
        statusDuration = 0;
    }

    public virtual void SaveEnemy(string saveID)
    {
        saveID = "Enemy" + saveID;

        SaveManager.UpdateSavedVector3(saveID + "Pos", transform.position);
        SaveManager.UpdateSavedVector3(saveID + "Rot", transform.rotation.eulerAngles);
        SaveManager.UpdateSavedBool(saveID + "Spawned", spawned);
        SaveManager.UpdateSavedBool(saveID + "Killed", killed);
        SaveManager.UpdateSavedFloat(saveID + "AttackTimer", attackTimer);
    }

    public virtual void LoadEnemy(string loadID)
    {
        loadID = "Enemy" + loadID;

        transform.position = SaveManager.GetVector3(loadID + "Pos");
        transform.rotation = Quaternion.Euler(SaveManager.GetVector3(loadID + "Rot"));
        spawned = SaveManager.GetBool(loadID + "Spawned");
        killed = SaveManager.GetBool(loadID + "Killed");
        attackTimer = SaveManager.GetFloat(loadID + "AttackTimer");

        gameObject.SetActive(spawned && !killed);
    }
}
