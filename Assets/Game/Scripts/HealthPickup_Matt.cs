/*
    Mateusz Szymanski
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup_Matt : MonoBehaviour
{
    public float RestoreValue;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<PlayerAttributes_Matt>().AddHealth(RestoreValue);
            gameObject.SetActive(false);
        }
    }
}
