using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerController : MonoBehaviour
{
    // mouse look
    public float sensitivity = 2;
    public float smoothing = 1.5f;
    public Transform cameraTransform;

    bool iAmMoving = false;

    public LayerMask layerMask;

    float rayCastLength = 1.2f;
    float rayBlockCheckLength = 20f;

    Vector3 targetPosition;
    GameObject nextBlock;

    float speed = 2f;
    float forwardMultiplier = 1f;

    // Rotations
    Quaternion upDirection = Quaternion.Euler(0, 0, 0);
    Quaternion downDirection = Quaternion.Euler(0, -180, 0);
    Quaternion leftDirection = Quaternion.Euler(0, -90, 0);
    Quaternion rightDirection = Quaternion.Euler(0, 90, 0);
    Quaternion angle;
    Quaternion currentRotation;

    public PinkyForecastPosition pinkyForecastPosition;
    public Transform pinkyForecastMarker;
    public InkyForecastPosition inkyForecastPosition;
    public Transform inkyForecastMarker;

    public GameManager gameManager;

    private float startTime;
    private float journeyLength;
    private Vector3 startPosition;

    public Transform portalLeft;
    public Transform portalRight;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        HandleMovement();
    }
    private void FixedUpdate()
    {
        MovePlayer();
        //RotateMe();
    }
    void HandleMovement()
    {
        bool forward = Input.GetAxisRaw("Vertical") > 0;
       
        if (forward)
        {
            if (!iAmMoving)
            {
                if (CheckIfCanGoForward())
                {
                    startTime = Time.time;
                    startPosition = transform.position;
                    targetPosition = transform.position + (transform.forward * forwardMultiplier);
                    journeyLength = Vector3.Distance(startPosition, targetPosition);
                    nextBlock = GetForwardBlock();
                    iAmMoving = true;
                }
            }
        }
    }
    void TeleportMe()
    {
        if (transform.position.x > 0)
        {
            transform.position = portalLeft.position;
        }
        else
        {
            transform.position = portalRight.position;
        }
    }
    public bool isPlayerMoving()
    {
        return iAmMoving;
    }
    public bool CheckIfCanTurnLeft()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -transform.right);
        Debug.DrawRay(transform.position, -transform.right * 10f, Color.white, 1f);
        if (Physics.Raycast(ray, out hit, 10f, layerMask))
        {
            if (hit.transform.name == "Block")
            {
                print("Can turn left "+hit.transform.tag);
                return true;
            }
        }
        return false;
    }
    public bool CheckIfCanTurnRound()
    {
        print("check if can turn round");
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -transform.forward);
        Debug.DrawRay(transform.position, -transform.forward * 10f, Color.white, 1f);
        if (Physics.Raycast(ray, out hit, 10f, layerMask))
        {
            if (hit.transform.name == "Block")
            {
                //print("Can turn round " + hit.transform.tag);
                return true;
            }
        }
        return false;
    }
    public bool CheckIfCanTurnRight()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.right);
        Debug.DrawRay(transform.position, transform.right * 10f, Color.black, 2f);
        if (Physics.Raycast(ray, out hit, 10f, layerMask))
        {
            if (hit.transform.name == "Block")
            {
                //print("Can turn right");
                return true;
            }
        }
        return false;
    }
    bool CheckIfCanGoForward()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.red, 1f);
        if (Physics.Raycast(ray, out hit, 10f, layerMask))
        {
            if (hit.transform.name == "Block")
            {
                return true;
            }
        }
        return false;
    }

    private void LateUpdate()
    {
        float height = transform.position.y;
        Vector3 newPos = transform.position;
        newPos.y = height;
        cameraTransform.position = newPos;
    }
    void ForecastPositions()
    {
        if (true)
        {
            //Pinky
            pinkyForecastMarker.position = transform.position;
            pinkyForecastMarker.rotation = transform.rotation;
            gameManager.SetPinkyTarget(pinkyForecastPosition.GetPinkyTargetBlock(nextBlock));
            //Inky
            inkyForecastMarker.position = transform.position;
            inkyForecastMarker.rotation = transform.rotation;
            gameManager.SetInkyTarget(inkyForecastPosition.GetInkyTargetBlock(nextBlock));
        }
    }
    void MovePlayer()
    {
        if (iAmMoving)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
            float d = Mathf.Abs(Vector3.Distance(transform.position, targetPosition));
            //print(d);
            if (d == 0)
            {
                //print("STOP moving forward " + transform.position);
                if (nextBlock != null)
                {
                    if (nextBlock.CompareTag("Portal"))
                    {
                        print("teleport me");
                        TeleportMe();
                    }
                }

                ForecastPositions();


                iAmMoving = false;
            }
        }
    }
    GameObject GetForwardBlock()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out hit, 1))
        {
            if (hit.transform.name == "Block")
                return (hit.transform.gameObject);
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        //Gizmos.DrawRay(transform.position, transform.forward * 2);
        Vector3 newPos = transform.position + transform.forward;
        Gizmos.DrawWireSphere(newPos, 0.5f);
    }
}
