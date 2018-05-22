using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraOperator : MonoBehaviour {

    public Texture2D selectionHighlight = null;
    public static Rect selection = new Rect(0, 0, 0, 0);   
    private Vector3 startClick = -Vector3.one;

    private static Vector3 moveToDestination = Vector3.zero;
    private static List<string> passables = new List<string>() { "Floor" };
    
	void Update () 
    {
        CheckCamera();
        MoveCleanUp();          
	}

    private void CheckCamera()
    {
        if (Input.GetMouseButtonDown(0))
            startClick = Input.mousePosition;
        else if (Input.GetMouseButtonUp(0))       
            startClick = -Vector3.one;

        if (Input.GetMouseButton(0))
        {
            selection = new Rect(startClick.x, InvertScreenY(startClick.y), Input.mousePosition.x - startClick.x, InvertScreenY(Input.mousePosition.y) - InvertScreenY(startClick.y));
            
            if (selection.width < 0)
            {
                selection.x += selection.width;
                selection.width = -selection.width;
            }
            if (selection.height < 0)
            {
                selection.y += selection.height;
                selection.height = -selection.height;
            }
        }
    }

    public void OnGUI()
    {
        if (startClick != -Vector3.one)
        {
            GUI.color = new Color(1, 1, 1, 0.5f);
            GUI.DrawTexture(selection, selectionHighlight);
        }
    }

    public static float InvertScreenY(float y)
    {
        return Screen.height - y;
    }
    private void MoveCleanUp()
    {
        if (Input.GetMouseButtonUp(1))
        {
            moveToDestination = Vector3.zero;
        }
    }
    public static Vector3 GetDestination()
    {
        Debug.Log(GameObject.FindGameObjectsWithTag("Player").Length); 
       
            RaycastHit hit;
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(r, out hit))
            {
                while (!passables.Contains(hit.transform.gameObject.name))
                {
                    if (!Physics.Raycast(hit.point + r.direction * 0.1f, r.direction, out hit))
                        break;
                }
            }
            if (hit.transform != null)
                moveToDestination = hit.point;

        return moveToDestination;
    }
}
