using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumHelper;

public class NoAmmoBox : MonoBehaviour
{
    public int AmmoTaken;
    public ElementTypes Type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Player>().SubstractAmmo(AmmoTaken, Type);
        }
    }
}
