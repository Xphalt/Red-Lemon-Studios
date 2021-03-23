//Script created by Zack

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Items : MonoBehaviour
{
    public float DropPercentage; //chance of dropping a pickup

    private void Start()
    {
        DropPercentage = Random.Range(0, 100); 
    }

    public void OnCollisionEnter(Collision collision)
    { //check if a bullet has collided
        if (collision.gameObject.tag == "Bullet")
        {
            Destroy(gameObject); //destroys the crate
            Drop(DropPercentage);
        }
    }

    public void Drop(float value)
    {
        Vector2 tempPos = gameObject.transform.position; //position at time of destruction
        if (value > 60 && value <= 80)// drop ammo pickup
        {
            GameObject[] boxes = GameObject.FindGameObjectsWithTag("PickUp");//grabs all ammo boxes
            int ChosenType = Random.Range(0, 3);//picks one to drop
            Instantiate(boxes[ChosenType], tempPos, Quaternion.identity);
        }
        else if (value > 80) // drop health pickup
        {
            Instantiate(GameObject.FindGameObjectWithTag("HealthUp"), tempPos, Quaternion.identity);
        }
        
    }

}
