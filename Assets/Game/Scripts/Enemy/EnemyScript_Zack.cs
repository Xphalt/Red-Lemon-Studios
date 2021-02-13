using UnityEngine;

//Zack Pilgrim

public class EnemyScript_Zack : MonoBehaviour
{
    public int HP = 100;
    private int damage = 10;
    public float moveSpeed = 3.0f;
    private bool chasing = false;

    public GameObject target;
    public Rigidbody moving;

    void Start()
    {
        moving = GetComponent<Rigidbody>();
    }

    void Update()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        if (chasing)
        {
            moving.velocity = (target.transform.position - gameObject.transform.position).normalized * moveSpeed;
        }
        else if (!chasing)
        {
            moving.velocity = Vector3.zero;
        }
        
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player_Weapon")
        {
            HP -= damage;
        }
        else if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerScript_Daniel>().TakeDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider bubble)
    {
        if (bubble.tag == "Player")
        {
            chasing = true;
        }
        
    }

    private void OnTriggerExit(Collider bubble)
    {
        if (bubble.tag == "Player")
        {
            chasing = false;
        }
    }
}
