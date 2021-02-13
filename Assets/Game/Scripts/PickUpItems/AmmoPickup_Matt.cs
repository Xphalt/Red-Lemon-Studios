/*
    Mateusz Szymanski
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup_Matt : MonoBehaviour
{
    public int AmmoValue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerScript_Daniel>().AddAmmo(AmmoValue);
            gameObject.SetActive(false);
        }
    }
}
