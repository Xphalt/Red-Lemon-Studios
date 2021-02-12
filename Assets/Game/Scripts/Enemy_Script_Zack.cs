using UnityEngine;

public class Enemy_Script_Zack : MonoBehaviour
{
    public int HP = 100;
    private int damage = 10;
    private float moveSpeed = 2.2f;
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
            moving.velocity = Vector3.MoveTowards(gameObject.transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        }
        else if (!chasing)
        {
            moving.velocity = Vector3.zero;
        }
        
        if (HP == 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player_Weapon")
        {
            HP -= damage;
        }
        else if (collision.gameObject.tag == "Player")
        {
            //collision.gameObject.GetComponent<Player_Script>().HP -= damage;
        }
    }

    private void OnTriggerEnter2D(Collider2D bubble)
    {
        chasing = true;
    }

    private void OnTriggerExit2D(Collider2D bubble)
    {
        chasing = false;
    }
}
