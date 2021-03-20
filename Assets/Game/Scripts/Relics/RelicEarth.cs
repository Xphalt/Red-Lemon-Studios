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
    }

    public override void Start()
    {
        base.Start();
        
        userMomentumResidue = Mathf.Clamp(userMomentumResidue, 0, 1);
        hostileMomentumResidue = Mathf.Clamp(hostileMomentumResidue, 0, 1);

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

    public override void SetUser(GameObject newUser)
    {
        base.SetUser(newUser);
        pillarScript.Initialise(pillarDamage, sizePerSecond, pillarLifeTime, characterScript.team, userMomentumResidue, hostileMomentumResidue);
    }

    public override bool Activate()
    {
        if (!base.Activate()) return false;

        RaycastHit[] floorHits = Physics.RaycastAll(new Ray(user.transform.position, -Vector3.up));
        foreach (RaycastHit floorHit in floorHits)
        {
            if (floorHit.transform.CompareTag("Floor"))
            {
                pillar.SetActive(true);

                float startScale = 0;

                pillarScript.Activate(startScale, floorHit.point + Vector3.up * startScale);
                characterScript.immortal = true;

                pillarTimer = 0;
                readyToUse = false;
                inUse = true;

                return true;
            }
        }
        return false;
    }

    public override void EndAbility()
    {
        base.EndAbility();

        pillar.SetActive(false);

        characterScript.immortal = false;
        characterScript.EndShift();
    }
}
