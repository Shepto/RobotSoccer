using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	// Use this for initialization
    Unit unit;
    void Start () {
        unit = transform.parent.GetComponent<Unit>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerStay(Collider other)
    {       
        unit.isThreating = true;
        if(unit.thread == null)
            unit.thread = other;
    }
    void OnTriggerExit(Collider other)
    {
        unit.isThreating = false;
        unit.thread = null;
    }
}
