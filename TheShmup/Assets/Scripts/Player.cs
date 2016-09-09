using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    public float speed=5f, timeBetweenShots=0.5f;
    public GameObject Bullet;
    public int bulletsToFire=1;
	
	// Update is called once per frame
	void Update ()
    {
        //Shoots a ray from the camera, through a point on the screen
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Stores the data if the ray his an object
        RaycastHit hit;
        //if the raycast hits, outputs the data to the hit variable and returns true
        if (Physics.Raycast(ray, out hit,100f,9))
        {
            //Slowly moves the player towards the hit position by the speed*delta time
            transform.position = Vector3.MoveTowards(transform.position, hit.point,speed*Time.deltaTime);
            //slightly tilts the player towards the hit position
            transform.rotation = Quaternion.RotateTowards(transform.rotation, GetTilt(transform.position, hit.point, 20f), 50f*Time.deltaTime);
        } else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, 50f * Time.deltaTime);
        }
    }

    //enumerators are complex but super useful, this one gets called every frame starting from the Start() func
    void Start()
    {
        StartCoroutine(FireLoop());
    }
    //this is where its declared
    IEnumerator FireLoop()
    {
        //while true is usually a massive red flag in programming design, but with enumerators they are totally safe
        while (true)
        {
            //first it does the fire bullet code
            FireBullet();
            //then it leaves the function until timeBetweenShots seconds has passed
            yield return new WaitForSeconds(timeBetweenShots);
            //once the time has passed, it returns here to recheck the loop and start all over again
        }
    }

    void FireBullet()
    {
        //creates a temp list
        List<GameObject> temp = new List<GameObject>();
        //foreach bullet to fire, creates a bullet
        for (int i = 0; i < bulletsToFire; i++)
        {
            temp.Add( (GameObject)Instantiate(Bullet, transform.position, Quaternion.identity));
        }

        //if there is more than one bullet
        if (temp.Count != 1)
        {
            //iterate over the list of bullets, shifting them apart based on the number of bullets
            for (int i = 0; i < temp.Count; i++)
            {
                float offset = Mathf.Lerp(-0.5f, 0.5f, (float)i / temp.Count) + (0.5f / temp.Count);
                //                                        //moves them between -0.5 and 0.5              //shifts them a little as i cant = 1
                temp[i].transform.position += new Vector3(offset, 0, -Mathf.Abs(offset));
            }
        }

    }

    public static Quaternion GetTilt(Vector3 planePosition, Vector3 targetPosition, float tiltAngle)
    {
        //Position difference between plane and target
        Vector3 positionDiff = targetPosition - planePosition;

        //The axis the plane will rotate on
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, positionDiff);

        //The quaternion to return
        return Quaternion.AngleAxis(tiltAngle, rotationAxis);
    }
}
