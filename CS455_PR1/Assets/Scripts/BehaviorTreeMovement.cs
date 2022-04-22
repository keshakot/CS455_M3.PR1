using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract class Task{

    public abstract bool run();

}

class Selector: Task {

    public Task[] children; 

    public override bool run(){
        foreach( Task c in children )
            if( c.run() )
                return true;
        return false;
    }
}

class Sequence: Task {

    public Task[] children; 

    public override bool run(){
        foreach( Task c in children )
            if( !c.run() )
                return false;
        return true;
    }
}



public class BehaviorTreeMovement: Kinematic
{
    public Door door;
    public Bridge bridge;
    Sequence root;
    private List<int> queue;

    /* Implementation-specific tasks */

    class DoorOpen: Task {
        private BehaviorTreeMovement btm;
        public DoorOpen( BehaviorTreeMovement b ){ btm = b; }
        public override bool run(){ return (btm.door.doorState == 2); }
    }

    class DoorOpenL: Task {
        private BehaviorTreeMovement btm;
        public DoorOpenL( BehaviorTreeMovement b ){ btm = b; }
        public override bool run(){ return !(btm.door.doorState == 2); }
    }

    class DoorLocked: Task {
        private BehaviorTreeMovement btm;
        public DoorLocked( BehaviorTreeMovement b ){ btm = b; }
        public override bool run(){ return !(btm.door.doorState == 1); }
    }

    class BridgeBroken: Task {
        private BehaviorTreeMovement btm;
        public BridgeBroken( BehaviorTreeMovement b ){ btm = b; }
        public override bool run(){ return (btm.bridge.bridgeState == 0); }
    }

    class BuildBridge: Task {
        private BehaviorTreeMovement btm;
        public BuildBridge( BehaviorTreeMovement b ){ btm = b; }
        public override bool run(){ this.btm.queue.Add(5); return true; }
    }

    class OpenDoor: Task {
        private BehaviorTreeMovement btm;
        public OpenDoor( BehaviorTreeMovement b ){ btm = b; }
        public override bool run(){ btm.door.doorState = 2; return true; }
    }

    class BargeDoor: Task {
        private BehaviorTreeMovement btm;
        public BargeDoor( BehaviorTreeMovement b ){ btm = b; }
        public override bool run(){ 
            this.btm.queue.Add(2);
           return true;
        }
    }

    class MoveIntoRoom: Task{
        private BehaviorTreeMovement btm;
        public MoveIntoRoom( BehaviorTreeMovement b ){ btm = b; }
        public override bool run(){
            this.btm.queue.Add(1);
            return true;
        }
    }

    class MoveToDoor: Task{
        private BehaviorTreeMovement btm;
        public MoveToDoor( BehaviorTreeMovement b ){ btm = b; }
        public override bool run(){
            this.btm.queue.Add(0);
            Debug.Log("Moving to door..."); /* Wait for completion of movement. */
            return true;
        }
    }

    class MoveToBridge: Task{
        private BehaviorTreeMovement btm;
        public MoveToBridge( BehaviorTreeMovement b ){ btm = b; }
        public override bool run(){
            this.btm.queue.Add(3);
            Debug.Log("Moving to bridge..."); /* Wait for completion of movement. */
            return true;
        }
    }

    class CrossBridge: Task{
        private BehaviorTreeMovement btm;
        public CrossBridge( BehaviorTreeMovement b ){ btm = b; }
        public override bool run(){
            this.btm.queue.Add(4);
            Debug.Log("Crossing bridge..."); /* Wait for completion of movement. */
            return true;
        }
    }
    /* ----------------------------- */

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(5);

        queue = new List<int>();

        // Create the behavior tree
        root = new Sequence();
        Selector l0_1 = new Selector();
        Sequence l1_1 = new Sequence();
        Sequence l1_2 = new Sequence();
        Sequence l1_3 = new Sequence();
        root.children = new Task[]{ l0_1, l1_3 };
        l0_1.children = new Task[]{ l1_1, l1_2 };

        l1_1.children = new Task[]{ new DoorOpen(this), new MoveIntoRoom(this) };

        Selector l2_2_2 = new Selector();
        Sequence l3_1 = new Sequence();
        Sequence l3_2 = new Sequence();
        l2_2_2.children = new Task[]{ l3_1, l3_2 };

        l3_1.children = new Task[]{ new DoorLocked(this), new OpenDoor(this) };
        l3_2.children = new Task[]{ new DoorOpenL(this), new BargeDoor(this) };

        l1_2.children = new Task[]{ new MoveToDoor(this), l2_2_2, new MoveIntoRoom(this) };

        Selector l2_3_2 = new Selector();
        Sequence l3_3_2 = new Sequence();
        l2_3_2.children = new Task[]{ l3_3_2, new CrossBridge(this) };
        l3_3_2.children = new Task[]{ new BridgeBroken(this), new BuildBridge(this) };
        l1_3.children = new Task[]{ new MoveToBridge(this), l2_3_2, new CrossBridge(this) };

        Debug.Log("Running tree...");
        root.run();
        Debug.Log("Ran tree.");
    }

    // Update is called once per frame
    protected override void Update()
    {
        if( this.queue != null && this.queue.Count > 0 ){
            int action = this.queue[0];
            switch ( action ){
                case 0: /* Move to door */
                    Vector3 trg = new Vector3(-3,1,0);
                    if( Vector3.Distance(this.transform.position, trg) > 0.1f )
                        this.transform.position = Vector3.MoveTowards(this.transform.position, trg, 5*Time.deltaTime);
                    else
                        this.queue.RemoveAt(0);
                    break;
                case 1: /* Move into room */
                    trg = new Vector3(7,1,0);
                    if( Vector3.Distance(this.transform.position, trg) > 0.1f )
                        this.transform.position = Vector3.MoveTowards(this.transform.position, trg, 5*Time.deltaTime);
                    else
                        this.queue.RemoveAt(0);
                    break;
                case 2: /* Barge door */
                    this.transform.localScale = new Vector3(3,3,3);
                    trg = new Vector3(7,1,0);
                    if( this.door.doorState != 3){
                        this.door.transform.position = new Vector3(0,2,0);
                        this.door.GetComponent<Rigidbody>().velocity = new Vector3(10,20,15);
                    }
                    this.door.doorState = 3;
                    if( Vector3.Distance(this.transform.position, trg) < 3.5f )
                        this.transform.localScale = new Vector3(1,1,1);
                    if( Vector3.Distance(this.transform.position, trg) > 0.1f )
                        this.transform.position = Vector3.MoveTowards(this.transform.position, trg, 10*Time.deltaTime);
                    else
                        this.queue.RemoveAt(0);
                    break;
                case 3: /* Move to bridge */
                    trg = new Vector3(7,1,6);
                    if( Vector3.Distance(this.transform.position, trg) > 0.1f )
                        this.transform.position = Vector3.MoveTowards(this.transform.position, trg, 5*Time.deltaTime);
                    else
                        this.queue.RemoveAt(0);
                    break;
                case 4: /* Cross bridge */
                    trg = new Vector3(7,1,16);
                    if( Vector3.Distance(this.transform.position, trg) > 0.1f )
                        this.transform.position = Vector3.MoveTowards(this.transform.position, trg, 5*Time.deltaTime);
                    else
                        this.queue.RemoveAt(0);
                    break;
                case 5: /* Build bridge */
                    this.bridge.bridgeState = 1;
                    this.queue.RemoveAt(0);
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator delay( float s ){
        yield return new WaitForSecondsRealtime( s );
        int i = 1; i++;
    }
}
