using UnityEngine;
using System.Collections;

public class GoalCheck : MonoBehaviour {

    public GameObject MainControl;
    public string GoalSide = "Left";

    MainControl mainControl;
	void Start () {
	    
	}
    void Awake()
    {
        mainControl = (MainControl)MainControl.GetComponent(typeof(MainControl));
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Ball")
        {
            if (this.GoalSide == "Left")
            {
                mainControl.RightGoal();
            }
            if (this.GoalSide == "Right")
            {
                mainControl.LeftGoal();
            }
        }
    }
}
