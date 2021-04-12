using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicEarth : RelicBase
{
    public GameObject pillar;
    private Transform pillarTransform;
    private EarthPillarScript pillarScript = null;
    public float maxPillarSize;
    public float pillarDamage;
    public float userMomentumResidue = 0.1f;
    public float hostileMomentumResidue = 1;

    public float pillarLifeTime;
    private float pillarTimer = 0;
    private float sizePerSecond;

    public override void Awake()
    {
        base.Awake();

        pillarScript = pillar.GetComponent<EarthPillarScript>();
        pillarTransform = pillar.GetComponent<Transform>();

        pillarTransform.localScale = new Vector3(pillarTransform.localScale.x, 0, pillarTransform.localScale.z);

        sizePerSecond = maxPillarSize / pillarLifeTime;

        pillar.SetActive(false);
    }

    public override void Update()
    {
        base.Update();

        if (inUse)
        {
            pillarTimer += Time.deltaTime;
            if (pillarTimer > pillarLifeTime)
            {
                EndAbility();
            }
        }
    }

    public override void SetUser(GameObject newUser, bool playSound=false)
    {
        base.SetUser(newUser, playSound);
        pillarScript.Initialise(pillarDamage, sizePerSecond, pillarLifeTime, characterScript.team, userMomentumResidue, hostileMomentumResidue);
    }

    public override bool Activate()
    {
        if (!base.Activate()) return false;

        if (Physics.Raycast(user.transform.position, Vector3.down, out RaycastHit hit))
        {
            pillar.SetActive(true);

            // use start scale if pillar is to appear at players feet regardless of vertical position

            pillarScript.Activate(0, hit.point);
            characterScript.Immortalise();

            pillarTimer = 0;
            readyToUse = false;
            inUse = true;

            sfxScript.PlaySFX2D(activateSound);
            if (myAnim) myAnim.SetTrigger("Activate");
                
            return true;
        }
        return false;
    }

    public override void EndAbility()
    {
        base.EndAbility();

        pillar.SetActive(false);

        characterScript.Mortalise();
        characterScript.EndShift();
    }
}
