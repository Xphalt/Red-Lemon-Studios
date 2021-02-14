using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoAmmoBox : MonoBehaviour
{
    public int AmmoTaken;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Player>().SubstractAmmo(AmmoTaken);
        }
    }
}
