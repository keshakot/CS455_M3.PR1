using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class ProjectileLaunch
{
    public Kinematic character;
    public GameObject target;
    public GameObject projectilePrefab;
    public Agent mAgent;

    float muzzleV = 26.0f;
    Vector3 offset = new Vector3(0, 2, 0);
    Vector3 gravity;

    private float xOffset = 15f;
    bool left = false;
    float maxSpeed = 8f;
    

    public ProjectileLaunch( GameObject dtpf ){
        projectilePrefab = dtpf;
        character = character;
    	Physics.gravity = new Vector3(0, -9.8f, 0);
        gravity = Physics.gravity;
        Physics.gravity = Physics.gravity / 2;
    }

    public bool fireProjectile()
    {   
        /* Set projectile position */
        Vector3 projPos = new Vector3( (left ? -xOffset : xOffset),
                                       Random.Range(-10f,10f),
                                       -1 );

        /* Set projectile firing angle/speed */
        //float projAngle = Random.Range(0f, Mathf.Deg2Rad*50f);
        float projAngle = 0f;
        float projSpeed = Random.Range(maxSpeed/2, maxSpeed);
        Vector3 projVec = new Vector3( (left ? 1 : -1)*projSpeed*Mathf.Cos(projAngle),
                                       projSpeed*Mathf.Sin(projAngle),
                                       0 );

    	/* Fire the projectile */
		GameObject projectile = UnityEngine.Object.Instantiate(projectilePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject; 
		projectile.transform.position = projPos;
		projectile.GetComponent<Rigidbody>().velocity = projVec;
        projectile.GetComponent<Projectile_Tracker>().agent = mAgent;

    	left = !left;

        return true;
    }
}
