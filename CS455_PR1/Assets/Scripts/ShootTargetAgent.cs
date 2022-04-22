using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ShootTargetAgent : Agent
{
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject trainingProjectilePrefab;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    public override void OnEpisodeBegin(){
        /*float spread = 8f;
        float vRange = 1f;
        GameObject train_projectile = UnityEngine.Object.Instantiate(trainingProjectilePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject; 
        train_projectile.transform.position = new Vector3(Random.Range(-spread, +spread),Random.Range(-spread, +spread),-1);
        train_projectile.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-vRange, +vRange), Random.Range(-vRange, +vRange), 0);
        train_projectile.GetComponent<Projectile_Tracker>().agent = this;*/
    }

    public override void CollectObservations( VectorSensor sensor ){
        //sensor.AddObservation( 1 );
        //sensor.AddObservation( targetTransform.localPosition );
    }

    public override void OnActionReceived( ActionBuffers actions ){
        bool fire = actions.ContinuousActions[2] > 0 ? true : false;

        //if( !fire ) return;

        float fireX = (float)(Screen.width) / 2 * (1 + actions.ContinuousActions[0]);
        float fireY = (float)(Screen.height) / 2 * (1 + actions.ContinuousActions[1]);

        Vector3 firePosition = new Vector3(fireX, fireY, 25);
        firePosition = Camera.main.ScreenToWorldPoint(firePosition);

        this.fireProjectile(firePosition.x, firePosition.y);
        Debug.Log("("+actions.ContinuousActions[0]+"," + actions.ContinuousActions[1] + ") -> ("+firePosition.x+","+firePosition.y+")");
    }

    public override void Heuristic( in ActionBuffers actionsOut ){
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        Vector3 clickedPosition = Input.mousePosition;

        continuousActions[0] = clickedPosition.x * 2/(float)(Screen.width) - 1;
        continuousActions[1] = clickedPosition.y * 2/(float)(Screen.height) - 1;
        continuousActions[2] = 1; // Always fire
    }

    public bool fireProjectile(float xTrg, float yTrg)
    {   
        /* Set projectile position */
        Vector3 projPos = camera.transform.position + Vector3.down*1;

        /* Set projectile firing angle/speed */
        Vector3 projVec = new Vector3( xTrg, yTrg, -1) - projPos;

        /* Fire the projectile */
        GameObject projectile = UnityEngine.Object.Instantiate(projectilePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject; 
        projectile.transform.position = projPos;
        projectile.GetComponent<Rigidbody>().velocity = projVec * 10;
        projectile.GetComponent<GoalProjectile>().agent = this;

        return true;
    }
}
