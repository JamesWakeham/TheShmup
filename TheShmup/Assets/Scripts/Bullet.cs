using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{

    // Speed of the Bullet
    public float speed = 20.0f;

    // Reference to own transform
    private Transform myTransform;

    void Awake()
    {
        // Gets reference to own transform
        myTransform = gameObject.transform;
    }

    void Update()
    {
        // Moves the Bullet
        myTransform.position += myTransform.forward * speed * Time.deltaTime;
    }

    // Destruction of object when an enemy bullet and player bullet collide
    public void Hurt()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider col)
    {
        // Checks for an object with either a player tag or an enemy tag while this isn't tagged enemy
        if (col.gameObject.tag == "Player" || (gameObject.tag != "Enemy" && col.gameObject.tag == "Enemy"))
            col.gameObject.SendMessage("Hurt");
        // Destroy itself if it isn't and the colliding object isn't tagged Enemy
        if (gameObject.tag != "Enemy" && col.gameObject.tag != "Enemy")
            Destroy(gameObject);
    }
}