using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {


    public GameObject player;
    public float m_Health;
    public GameObject m_bullet;
    private float shotTimer = 0;
    private float nextShot;
    public Vector3 t_left;
    public Vector3 t_right;

    public Vector3 target;


    public float minShotTime = 1.5f;
    public float maxShotTime = 3.5f;


    // Use this for initialization
    void Start () {
	
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");



        // These Values can be manually set in the inspector if the raycast's don't fuggin work

        if(t_left == new Vector3(0,0,0)) // get the position of the left side of the screen
        {
            RaycastHit hit;
            Ray ray = camera.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width/5, Screen.width/2, 0));
            if(Physics.Raycast(ray, out hit))
            {
                t_left = hit.point;
                Debug.Log(hit.point);

            }
        }

        if (t_right == new Vector3(0, 0, 0)) // get the position of the right side of the screen
        {
            RaycastHit hit;
            Ray ray = camera.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width - (Screen.width / 5), Screen.width / 2, 0));
            if (Physics.Raycast(ray, out hit))
            {
                t_right = hit.point;
                Debug.Log(hit.point);
            }
        }


        int chance = Random.Range(0, 1);
        if(chance == 0)
        {
            target = t_left;
        }
        else if(chance == 1)
        {
            target = t_right;
        }

    }

    // Kill the boss and increase score by 100
    public void Hurt()
    {
        GameManager.refer.IncreaseScore(100);
        Destroy(gameObject);
    }

    // Desroy the object if colliding with something that isn't tagged as enemy
    void OnTriggerEnter(Collider col)
    {
        // Checks for a player, if so sends Hurt to the player as well
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.SendMessage("Hurt");
            Destroy(gameObject);
        }
        // Destroys itself if colliding with something that isn't tagged Enemy
        else if (col.gameObject.tag != "Enemy")
            Destroy(gameObject);
    }



    Vector3 velocity;

	// Update is called once per frame
	void Update () {


        shotTimer += Time.deltaTime;
        if (shotTimer > nextShot && m_bullet != null)
        {
            GameObject reff = Instantiate(m_bullet, transform.position + (transform.forward * 0.8f), transform.rotation) as GameObject;
            reff.tag = "Enemy";
            shotTimer = 0;
            nextShot = Random.Range(minShotTime, maxShotTime);
        }




        // move around boss whale
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, Random.Range(0.2f, 0.5f));

        if(Vector3.Distance(target, transform.position) < 0.5f)
        {   // switch target position
            if(target == t_left) { target = t_right; }
            else if (target == t_right) { target = t_left; }
        }


	}
}
