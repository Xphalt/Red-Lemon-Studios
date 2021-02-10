using UnityEngine;

public class Enemy_Script : MonoBehaviour
{
    public int HP = 100;
    private int damage = 10;
    private float moveSpeed = 2.2f;

    public GameObject target;

    void Update()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target.transform.position, moveSpeed * Time.deltaTime);

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
}
