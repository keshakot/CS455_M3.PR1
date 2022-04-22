using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal {
    public string name;
    public float value;

    public Goal( string n, float v ){
        name = n;
        value = v;
    }
}

public abstract class Action {
    public string action;
    public abstract float getGoalChange( Goal goal );
}

public class GOBMovement: Kinematic
{
    Sequence root;
    private List<Action> queue;
    public Action[] gActions;
    public Goal[] gGoals;
    public int timesteps = 0;

    private int delayFlag = 0;
    private int time;
    private Vector3 loc;

    /* Implementation-specific tasks */
    class UseLoo: Action {
        public string action = "UseLoo";
        public UseLoo(){ base.action = action; }
        public override float getGoalChange( Goal goal ){
            switch (goal.name){
                case "Loo":
                    return -4f;
                case "Eat":
                    return 1f;
                case "Sleep":
                    return 1f;
                case "Work":
                    return 0;
                default:
                    return 0f;
            }
        }
    }

    class EatMeal: Action {
        public string action = "EatMeal";
        public EatMeal(){ base.action = action; }
        public override float getGoalChange( Goal goal ){
            switch (goal.name){
                case "Loo":
                    return 2f;
                case "Eat":
                    return -3f;
                case "Sleep":
                    return 1f;
                case "Work":
                    return 1f;
                default:
                    return 0f;
            }
        }
    }

    class EatSnack: Action {
        public string action = "EatSnack";
        public EatSnack(){ base.action = action; }
        public override float getGoalChange( Goal goal ){
            switch (goal.name){
                case "Loo":
                    return 1f;
                case "Eat":
                    return -2f;
                case "Sleep":
                    return 1f;
                case "Work":
                    return 0;
                default:
                    return 0f;
            }
        }
    }

    class Sleep: Action {
        public string action = "Sleep";
        public Sleep(){ base.action = action; }
        public override float getGoalChange( Goal goal ){
            switch (goal.name){
                case "Loo":
                    return 1f;
                case "Eat":
                    return 2f;
                case "Sleep":
                    return -4f;
                case "Work":
                    return 2f;
                default:
                    return 0f;
            }
        }
    }

    class Work: Action {
        public string action = "Work";
        public Work(){ base.action = action; }
        public override float getGoalChange( Goal goal ){
            switch (goal.name){
                case "Loo":
                    return 1f;
                case "Eat":
                    return 2f;
                case "Sleep":
                    return 3f;
                case "Work":
                    return -4f;
                default:
                    return 0f;
            }
        }
    }
 
    /* ----------------------------- */

    Action chooseActionSimple( Action[] actions, Goal[] goals ){
        Goal topGoal = goals[0];

        // Find the most 'valuable' goal
        for(int i=1; i<goals.Length; i++)
            if( goals[i].value > topGoal.value )
                topGoal = goals[i];

        // Find the best action to take
        Action bestAction = actions[0];
        float bestUtility = -actions[0].getGoalChange( topGoal );

        for(int i=1; i<actions.Length; i++){
            float utility = -actions[i].getGoalChange( topGoal );
            if( utility > bestUtility ){
                bestUtility = utility;
                bestAction = actions[i];
            }
        }

        return bestAction;
    }

    float discontentment( Action action, Goal[] goals ){
        float discontentment = 0;

        foreach( Goal goal in goals){
            float i = goal.value + action.getGoalChange( goal );
            discontentment += i*i;
        }

        return discontentment;
    }

    Action chooseActionUtility( Action[] actions, Goal[] goals ){
        Goal topGoal = goals[0];

        // Find the most 'valuable' goal
        for(int i=1; i<goals.Length; i++)
            if( goals[i].value > topGoal.value )
                topGoal = goals[i];

        // Find the action leading to the lowest discontentment
        Action bestAction = actions[0];
        float bestDsc = discontentment( bestAction, goals );

        for(int i=1; i<actions.Length; i++){
            float dsc = discontentment( actions[i], goals );
            if( dsc < bestDsc ){
                bestDsc = dsc;
                bestAction = actions[i];
            }
        }

        return bestAction;
    }

    void printWeights( Action a ){
        if( gGoals == null ) return; 
        
        string txt = "[T=" + timesteps + "] Weights: ";

        foreach( Goal g in gGoals )
            txt += string.Format(" [{0}:{1}]", g.name, g.value);

        txt += " -> " + a.action;

        Debug.Log( txt );
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(1);

        // Define goals and actions
        gActions = new Action[]{};
        gGoals = new Goal[]{ new Goal("Eat", 4), new Goal("Sleep", 3), new Goal("Loo", 5), new Goal("Work", 5) };
        gActions = new Action[]{ new UseLoo(), new EatMeal(), new EatSnack(), new Sleep(), new Work() };

        queue = new List<Action>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if( queue == null ) return; 
        else if( delayFlag > 0 ){ delayFlag--; return; }


        // If no action to perform, choose action //
        if( queue.Count == 0 ){
            Action a = this.chooseActionUtility( this.gActions, this.gGoals );
            this.printWeights( a );
            queue.Add( a );
            timesteps++;
            switch (queue[0].action){
                case "UseLoo":
                    loc = new Vector3(10.25f, 1, 0);
                    time = 50;
                    break;
                case "Sleep":
                    loc = new Vector3(-12.25f, 1, 0);
                    time = 400;
                    break;
                case "EatSnack":
                    loc = new Vector3(-3, 1, -4);
                    time = 100;
                    break;
                case "EatMeal":
                    loc = new Vector3(4, 1, -4);
                    time = 200;
                    break;
                case "Work":
                    loc = new Vector3(0, 1, 4.25f);
                    time = 350;
                    break;
                default:
                    loc = new Vector3(0,0,0);
                    time = 0;
                    break;
            }
        }
        // If an action exists in the queue, perform that action //
        else {
            if( Vector3.Distance(this.transform.position, loc) > 0.1f )
                this.transform.position = Vector3.MoveTowards(this.transform.position, loc, 5*Time.deltaTime);
            else if (time > 0 )
                time--;
            else{
                // Adjust the weights in accordance with the performed action //
                foreach( Goal g in gGoals ) {
                    g.value += queue[0].getGoalChange( g );
                    g.value = Mathf.Max( g.value, 0f ); // to avoid negative weights
                }
                queue.RemoveAt(0);
            }
        }
    }

    IEnumerator delay( float s ){
        yield return new WaitForSeconds( s );
        delayFlag = 0;
    }
}
