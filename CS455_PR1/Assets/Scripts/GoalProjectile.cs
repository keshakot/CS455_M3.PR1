using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class GoalProjectile : MonoBehaviour
{
    public Agent agent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter( Collider other ){
        //Debug.Log("HIT SOMETHING");
        if( other.TryGetComponent<Projectile_Tracker>(out Projectile_Tracker goal)){
            agent.AddReward( +10f );
            Debug.Log("HIT TARGET");
            agent.EndEpisode();
            Destroy(gameObject);
        }
        else{
            agent.AddReward( -0.01f );
            Destroy(gameObject);
        }
    }
}
