using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    public override void OnEpisodeBegin(){
        transform.localPosition = new Vector3(Random.Range(-6.5f, +1f), 1, Random.Range(-6.5f, +6.5f));
        targetTransform.localPosition = new Vector3(Random.Range(+2f, +6.5f), 1, Random.Range(-6.5f, +6.5f));
    }

    public override void CollectObservations( VectorSensor sensor ){
        sensor.AddObservation( transform.localPosition );
        sensor.AddObservation( targetTransform.localPosition );
    }

    public override void OnActionReceived( ActionBuffers actions ){
        //Debug.Log(actions.DiscreteActions[0]);

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 2.0f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic( in ActionBuffers actionsOut ){
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter( Collider other ){
        if( other.TryGetComponent<Goal0>(out Goal0 goal)){
            SetReward( +1f );
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
        if( other.TryGetComponent<Wall>(out Wall wall)){
            SetReward( -1f );
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
    }
}
