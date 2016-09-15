/*
 * ENEMY
 *     Moves the object on its local forward axis, shooting bullets.
 *     The amount of time it takes between shots is different each time ranging between minShotTime and maxShotTime.
 *     The speed of the object is different on spawn to add diversity to each enemy, raging between minSpeed and maxSpeed.
 *      
 */

using UnityEngine;

public class Enemy : MonoBehaviour {

    // Reference to bullet
    public GameObject bullet, deathPrefab;
    
    // Private variables
    private float shotTimer = 0;
    private float speed;
    private float nextShot;

    // Speed and Shot time
    public float minShotTime = 1.5f;
    public float maxShotTime = 3.5f;
    public float minSpeed = 2.0f;
    public float maxSpeed = 5.0f;

    // Reference to own Transform
    private Transform myTransform;

	// Use this for initialization
	void Awake () {
        // Sets reference to transform
        myTransform = gameObject.transform;
        // Set speed to a random range to add diversity
        speed = Random.Range(minSpeed, maxSpeed);
        // Set the first instance of nextShot
        nextShot = Random.Range(minShotTime, maxShotTime);
    }
	
	// Update is called once per frame
	void Update () {
        // Shooting Logic
        shotTimer += Time.deltaTime;
        if (shotTimer > nextShot)
        {
            GameObject reff = Instantiate(bullet, myTransform.position + (myTransform.forward * 0.8f), myTransform.rotation) as GameObject;
            reff.tag = "Enemy";
            shotTimer = 0;
            nextShot = Random.Range(minShotTime, maxShotTime);
        }

        // Movement
        myTransform.position += myTransform.forward * speed * Time.deltaTime;
    }

    // Kill the object and increase score by 10
    public void Hurt()
    {
        Die();
    }

    void Die ()
    {
        GameManager.refer.IncreaseScore(10);
        GameObject explo = (GameObject)Instantiate(deathPrefab, transform.position, Quaternion.identity);
        Destroy(explo, 3);
        Destroy(gameObject);
    }

    // Desroy the object if colliding with something that isn't tagged as enemy
    void OnTriggerEnter(Collider col)
    {
        // Checks for a player, if so sends Hurt to the player as well
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.SendMessage("Hurt");
            Die();
        }
        // Destroys itself if colliding with something that isn't tagged Enemy
        else if (col.gameObject.tag != "Enemy")
            Die();
    }
}
