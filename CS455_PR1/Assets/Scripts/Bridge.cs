using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge: Kinematic
{
    /* 0 = broken; 1 = fixed */
    public int bridgeState;

    public void bridgeStateBroken(){ bridgeState = 0; }
    public void bridgeStateFixed(){ bridgeState = 1; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        if( bridgeState == 0 ){
            //this.enabled = false;
            this.transform.position = new Vector3(7,0,0);
        }
        else{
            //this.enabled = true;
            this.transform.position = new Vector3(7,0,10.25f);
        }

    }
}
