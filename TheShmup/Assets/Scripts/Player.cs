using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    public float speed=5f, timeBetweenShots=0.5f;
    public GameObject Bullet;
    public int bulletsToFire=1;
	// Use this for initialization
	void Start () {
        StartCoroutine(FireLoop());
	}
	
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

    IEnumerator FireLoop()
    {
        while (true)
        {
            FireBullet();
            yield return new WaitForSeconds(timeBetweenShots);
            yield return null;
        }
    }

    void FireBullet()
    {
        List<GameObject> temp = new List<GameObject>();
        for (int i = 0; i < bulletsToFire; i++)
        {
            temp.Add( (GameObject)Instantiate(Bullet, transform.position + (Vector3.forward*0), Quaternion.identity));
        }
        
        for (int i = 0; i < temp.Count; i++)
        {
            temp[i].transform.position += new Vector3(Random.Range(-0.5f, 0.5f), 0);
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
