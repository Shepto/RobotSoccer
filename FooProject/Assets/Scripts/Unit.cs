using UnityEngine;
using System.Collections;
using Pathfinding;

public class Unit : MonoBehaviour 
{
    public bool watchBall = false;
    public Vector3 ballPosition;

    public bool selected = false;
    public float floorOffset = 1;
    public float speed = 20;
    public float stopDistanceOffset = 5.0f;
    public float angleOffset = 5.0f;
    public float rotationSpeed = 5.0f;
    
    private bool selectedByClick = false;
    
    private Quaternion startRotation;
    private Vector3 startPosition;
    
    private Path path;
    private int currentWaypoint = 0;

    public Collider thread;
    public bool isThreating;


    private Vector3 moveToDest = Vector3.zero;
    public bool reachDestination = false;
    private float angle = 0.0f;

    void Start()
    {

    }
    void Awake()
    {
        startRotation = transform.rotation;
        startPosition = transform.position;
    }

    public void Reset()
    {
        transform.rotation = startRotation;
        transform.position = startPosition;        
    }

    public void MoveTo(Vector3 destination)
    {  
        moveToDest = destination;
        
        reachDestination = true;        
    }
    public void MoveTo(Vector2 destination)
    {
        MoveTo(new Vector3(destination.x, transform.position.y, destination.y));
    }

    void Update()
    {
        Renderer tmpRender = transform.GetComponentsInChildren<MeshRenderer>()[0].GetComponent<Renderer>();
        if (tmpRender.isVisible && Input.GetMouseButton(0))
        {
            if (!selectedByClick)
            {
                Vector3 campPos = Camera.main.WorldToScreenPoint(transform.position);
                campPos.y = CameraOperator.InvertScreenY(campPos.y);
                selected = CameraOperator.selection.Contains(campPos);
            }
        }

        if (selected && Input.GetMouseButtonUp(1))
        {
            Vector3 destination = CameraOperator.GetDestination();
            if (destination != Vector3.zero)
            {                
                MoveTo(destination);
            }
        }
    }

    void FixedUpdate()
    {
        if (reachDestination)
        {
            Vector3 tmp = moveToDest;
            tmp.y = transform.position.y;
            
            float dist = Vector3.Distance(transform.position, tmp);
            
            //robot is in the desired destionation
            if (dist < 5.0f)
            {
                transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                reachDestination = false;

                if (watchBall)
                {
                    Vector3 bp = new Vector3(ballPosition.x, transform.position.y, ballPosition.y);
                    angle = FindAngle(transform.forward, bp - transform.position, transform.up);
                    if (Mathf.Abs(angle) > angleOffset)
                    {
                        transform.Rotate(0, Mathf.Sign(angle) * rotationSpeed, 0, Space.World);
                    }
                }
            }
            //move or rotate to destination
            else
            {
                angle = FindAngle(transform.forward, tmp - transform.position, transform.up);
                if (Mathf.Abs(angle) < angleOffset)
                {
                    transform.LookAt(tmp);
                    GetComponent<Rigidbody>().velocity = transform.forward * speed;
                }
                else
                {
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    transform.Rotate(0, Mathf.Sign(angle) * rotationSpeed, 0, Space.World);
                }
            }
        }
    }

    private float FindAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector)
    {
        if (toVector == Vector3.zero)
            return 0.0f;
        float angle = Vector3.Angle(fromVector, toVector);
        Vector3 normal = Vector3.Cross(fromVector, toVector);

        angle *= Mathf.Sign(Vector3.Dot(normal, upVector));

        return angle;
    }
    
    public void OnPathComplete(Path path)
    {
        if (!path.error)
        {
            currentWaypoint = 0;
            this.path = path;                     
        }
        else
            Debug.Log(path.error);
    }
    private void UpdateMove()
    {
        if (moveToDest != Vector3.zero && transform.position != moveToDest)
        {
            Vector3 direction = (moveToDest - transform.position).normalized;
            direction.y = 0;
            transform.GetComponent<Rigidbody>().velocity = direction * speed;

            if (Vector3.Distance(transform.position, moveToDest) < stopDistanceOffset)
                moveToDest = Vector3.zero;
        }
        else
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    private void OnMouseDown()
    {
        selectedByClick = true;
        selected = true;        
    }
    private void OnMouseUp()
    {
        if (selectedByClick)
            selected = true;
        selectedByClick = false;
    }    
}