/*
 * BULLET
 *      Apply to an object and it will move along its local forwad axis.
 *      Will ignore other bullets and enemies if tagged "Enemy" (set in ENEMY).
 *      Bullets destroy themselves anytime they collide with:
 *          - An object tagged "Enemy" or "Player" sending a Hurt message.
 *          - Another bullet that isn't tagged when this is tagged "Enemy".
 *          - Any object that isn't tagged with "Enemy" when this is tagged "Enemy"
 * 
 */

using UnityEngine;

public class Bullet : MonoBehaviour {

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
        // Checks if this is tagged as "Enemy"
        if (gameObject.tag == "Enemy")
        {
            // Checks if it is colliding with the player
            if (col.gameObject.tag == "Player")
                col.gameObject.SendMessage("Hurt");
            // Checks if it is colliding with something that isn't an
            if (col.gameObject.tag != "Enemy")
                Destroy(gameObject);
        }
        else
        {
            // Checks if it is colliding with an enemy
            if (col.gameObject.tag == "Enemy")
                col.gameObject.SendMessage("Hurt");
            // Checks if it is colliding with a player
            if (col.gameObject.tag != "Player")
                Destroy(gameObject);
        }
    }
}
