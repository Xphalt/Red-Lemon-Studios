using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Script : MonoBehaviour
{
<<<<<<< Updated upstream
    // Start is called before the first frame update
    void Start()
=======
    public int HP;
    private int damage;
    private float moveSpeed;

    public GameObject target;

    void Start()
    {
        HP = 100;
        damage = 10;
        moveSpeed = 2.2f;
    }
    void Update()
>>>>>>> Stashed changes
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
