using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class EarthPillarScript : MonoBehaviour
{
    private float damage;
    private float sizePerSecond;
    private float lifeTime;
    private float userMomentumResidue;
    private float hostileMomentumResidue;
    internal Teams team;

    public void Initialise(float pillarDamage, float growthRate, float pillarLifeTime, Teams userTeam, float userMomentum, float hostileMomentum)
    {
        damage = pillarDamage;
        sizePerSecond = growthRate;
        lifeTime = pillarLifeTime;
        team = userTeam;
        userMomentumResidue = userMomentum;
        hostileMomentumResidue = hostileMomentum;
    }

    void Update()
    {
        transform.Translate(Vector3.up * (sizePerSecond * Time.deltaTime));
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + sizePerSecond * Time.deltaTime, transform.localScale.z);
    }

    public void Activate(float startScale, Vector3 startPos)
    {
        transform.localScale = new Vector3(transform.localScale.x, startScale, transform.localScale.z);
        transform.position = startPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out CharacterBase collisionCharacter))
        {
            if (collisionCharacter.team != team) collisionCharacter.TakeDamage(damage);

            float momentumResidue = (collisionCharacter.team == team) ? userMomentumResidue : hostileMomentumResidue;
            collisionCharacter.Shift(Vector3.up * sizePerSecond * 2, lifeTime, momentumResidue);
        }
    }
}
