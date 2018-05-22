using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

    public float OffsetRadius = 3.0f;
    public int OffsetCounter = 100;
    public int counter = 0;    
    public GameObject MainControl;    

    MainControl mainControl;
    Vector3 position = Vector3.zero;
    void Start () 
    {
	
	}
    void Awake()
    {
        mainControl = (MainControl)MainControl.GetComponent(typeof(MainControl));
    }
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (!mainControl.starting)
        {
            if (Vector3.Distance(transform.position, position) >= OffsetRadius)
            {
                position = transform.position;
                counter = 0;
            }
            else
            {
                counter++;
                if (counter > OffsetCounter)
                {
                    counter = 0;
                    mainControl.ResetToStartPosition();
                }
            }
        }
	}
    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "LeftPlayer")
        {
            mainControl.LeftControl();
        }
        if (other.transform.tag == "RightPlayer")
        {
            mainControl.RightControl();
        }
    }
}
