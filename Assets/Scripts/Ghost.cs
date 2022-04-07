using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ghost : MonoBehaviour
{
    // Animations for scattering
    Animator anim;

    // Identity
    string ghostName;

    // Reference variables
    public GameObject player;
    public GameObject scatterPoint;
    public GameManager gameManager;
    //public GameObject testBlock;

    // Movement variables
    //float range = 10f;
    //Vector3 nextTilePos;
    //bool canChangeDirection = false;
    GameObject nextBlock;
    bool canMove;
    bool hasNextBlock = false;
    float speed;
    float initialSpeed = 1f;
    float rayCastLength = 1f;

    private GameObject target;
    private Vector3 targetPos;

    // Mode variables
    private bool isChasemode = true;

    // UI
    //public TMP_Text toggleText;
    // Constants
    enum Direction
    {
        up,
        down,
        left,
        right
    }
    //Direction direction = Direction.right;
    // Rotations
    Quaternion upDirection = Quaternion.Euler(0, 0, 0);
    Quaternion downDirection = Quaternion.Euler(0, -180, 0);
    Quaternion leftDirection = Quaternion.Euler(0, -90, 0);
    Quaternion rightDirection = Quaternion.Euler(0, 90, 0);
    // Types of block
    private enum BlockType
    {
        none,
        upRight,
        upLeft,
        downRight,
        downLeft,
        leftUpDown,
        rightUpDown,
        leftRightDown,
        leftRightUp,
        cross,
        reverse,
        leftRight
    }
    BlockType nextBlockType;

    // Start is called before the first frame update
    void Start()
    {
        ghostName = transform.name;
        anim = GetComponent<Animator>();
        if (anim == null) print("ERROR: missing animator");
    }

    // Update is called once per frame
    void Update()
    {
        // isChaseMode = true ... chase ... change target to player
        // isChaseMode = false .... scatter ... change target to scatter point
        if (isChasemode)
        {
            speed = initialSpeed;
            switch (ghostName)
            {
                case "Pinky":
                    targetPos = gameManager.GetPinkyTarget();
                    if (targetPos == null) target = player;
                    Debug.DrawLine(transform.position, targetPos, Color.magenta, 0.1f);
                    break;
                case "Blinky":
                    targetPos = player.transform.position;
                    Debug.DrawLine(transform.position, targetPos, Color.red, 0.1f);
                    break;
                case "Inky":
                    targetPos = gameManager.GetInkyTarget();
                    if (targetPos == null) target = player;
                    Debug.DrawLine(transform.position, targetPos, Color.cyan, 0.1f);
                    break;
                case "Clyde":
                    float distanceToPlayer = Vector3.Distance(
                        transform.position, player.transform.position);
                    //Debug.Log("Clyde distance = " + distanceToPlayer);
                    if (Mathf.Abs(distanceToPlayer) <= 8f)
                    {
                        targetPos = scatterPoint.transform.position;
                        Debug.DrawLine(transform.position, targetPos,
                            Color.yellow, 0.1f);
                    }
                    else
                    {
                        targetPos = player.transform.position;
                        Debug.DrawLine(transform.position, targetPos,
                            Color.yellow, 0.1f);
                    }
                    break;
                case null:
                    print("ERROR; no ghost found");
                    break;
            }
        }
        else
        {
            targetPos = scatterPoint.transform.position;
            speed = initialSpeed * 3f;
            Debug.DrawLine(transform.position, targetPos, Color.red, 0.1f);
        }

        if (hasNextBlock == false)
        {
            nextBlock = GetForwardBlock();
            if (nextBlock == null)
            {
                MoveForward();
            }
            else
            {
                hasNextBlock = true;
                canMove = true;
            }
        }
        else
        {
            if (canMove)
            // MOVE
            {
                Move(nextBlock);
            }
            else
            // TURN
            {
                HandleRoatationsBasedOnPlayerPosition(nextBlock);

                hasNextBlock = false;
                canMove = false;
            }
        }


    }

    public void HandleRoatationsBasedOnPlayerPosition(GameObject nextBlock)
    {
        // Rotates gameobject based on 
        nextBlockType = GetNextBlockType(nextBlock);

        // LEFT RIGHT ONLY
        if (nextBlockType == BlockType.leftRight)
        {
            //print("Coming right");
            // Get next block in a direction
            float tile1DistToTarget = Vector3.Distance(GetLeftBlockPos(), targetPos);
            float tile2DistToTarget = Vector3.Distance(GetRightBlockPos(), targetPos);

            if (tile1DistToTarget < tile2DistToTarget)
            {
                transform.eulerAngles = leftDirection.eulerAngles;
            }
            else
            {
                transform.eulerAngles = rightDirection.eulerAngles;
            }
        }

        // REVERSE BLOCK
        if (nextBlockType ==BlockType.reverse)
        {
            if (transform.eulerAngles == leftDirection.eulerAngles)
            {
                transform.eulerAngles = rightDirection.eulerAngles;
            }
            else if (transform.eulerAngles == rightDirection.eulerAngles)
            {
                transform.eulerAngles = leftDirection.eulerAngles;
            }
        }

        // STANDARD BLOCKS
        if (nextBlockType == BlockType.upRight)
        {
            if (transform.eulerAngles == leftDirection.eulerAngles)
            {
                transform.eulerAngles = upDirection.eulerAngles;
            }
            else if (transform.eulerAngles == downDirection.eulerAngles)
            {
                transform.eulerAngles = rightDirection.eulerAngles;
            }
        }

        if (nextBlockType == BlockType.upLeft)
        {
            if (transform.eulerAngles == rightDirection.eulerAngles)
            {
                transform.eulerAngles = upDirection.eulerAngles;
            }
            else if (transform.eulerAngles == downDirection.eulerAngles)
            {
                transform.eulerAngles = leftDirection.eulerAngles;
            }
        }

        if (nextBlockType == BlockType.downLeft)
        {

            if (transform.eulerAngles == rightDirection.eulerAngles)
            {
                transform.eulerAngles = downDirection.eulerAngles;
            }
            if (transform.eulerAngles == upDirection.eulerAngles)
            {
                transform.eulerAngles = leftDirection.eulerAngles;
            }
        }

        if (nextBlockType == BlockType.downRight)
        {
            if (transform.eulerAngles == leftDirection.eulerAngles)
            {
                transform.eulerAngles = downDirection.eulerAngles;
            }
            if (transform.eulerAngles == upDirection.eulerAngles)
            {
                transform.eulerAngles = rightDirection.eulerAngles;
            }
        }

        // DECISION BLOCKs
        // ┤ done
        // ╞ done
        // ╤ done
        // ╨  done
        // ╬ done

        // DECISION BLOCK - LEFT UP DOWN ┤
        if (nextBlockType == BlockType.leftUpDown)
        {
            //print("GHOST - next block = " + nextBlock.tag);

            // COMING RIGHT ┤
            if (transform.eulerAngles == rightDirection.eulerAngles)
            {

                // Get neighbour blocks 
                float tile1DistToTarget = Vector3.Distance(GetLeftBlockPos(), targetPos);
                float tile2DistToTarget = Vector3.Distance(GetRightBlockPos(), targetPos);

                if (tile1DistToTarget < tile2DistToTarget)
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
            }
            // COMING UP ┤
            else if (transform.eulerAngles == upDirection.eulerAngles)
            {
                // Get next block in a direction
                float forwardDistToTarget = Vector3.Distance(GetForwardBlockPos(), targetPos);
                float westDistToTarget = Vector3.Distance(GetLeftBlockPos(), targetPos);

                if (forwardDistToTarget < westDistToTarget)
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
            }
            // COMING DOWN ┤
            //else if (transform.eulerAngles == downDirection.eulerAngles)
            else if (transform.eulerAngles == downDirection.eulerAngles)
            {
                //print("Coming down    ");
                // Get next block in a direction
                float tile1DistToTarget = Vector3.Distance(GetForwardBlockPos(), targetPos);
                float tile2DistToTarget = Vector3.Distance(GetRightBlockPos(), targetPos);

                if (tile1DistToTarget < tile2DistToTarget)
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
            }
        }

        // DECISION BLOCK - RIGHT LEFT DOWN ╤
        else if (nextBlockType == BlockType.leftRightDown)
        {
            // COMING RIGHT ╤
            if (transform.eulerAngles == rightDirection.eulerAngles)
            {

                // Get neighbour blocks 
                float tile1DistToTarget = Vector3.Distance(GetForwardBlockPos(), targetPos);
                float tile2DistToTarget = Vector3.Distance(GetRightBlockPos(), targetPos);

                if (tile1DistToTarget < tile2DistToTarget)
                {
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
            }
            // COMING UP ╤
            else if (transform.eulerAngles == upDirection.eulerAngles)
            {
                // print("Coming up");
                // Get next block in a direction
                float tile1DistToTarget = Vector3.Distance(GetRightBlockPos(), targetPos);
                float tile2DistToTarget = Vector3.Distance(GetLeftBlockPos(), targetPos);

                if (tile1DistToTarget < tile2DistToTarget)
                {
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
            }
            // COMING LEFT ╤
            else if (transform.eulerAngles == leftDirection.eulerAngles)
            {
                //print("Coming left");
                // Get next block in a direction
                float tile1DistToTarget = Vector3.Distance(GetForwardBlockPos(), targetPos);
                float tile2DistToTarget = Vector3.Distance(GetLeftBlockPos(), targetPos);

                if (tile1DistToTarget < tile2DistToTarget)
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
            }
        }

        // DECISION BLOCK - RIGHT UP DOWN ╞ 
        else if (nextBlockType == BlockType.rightUpDown)
        {
            // Coming Up ╞ 
            if (transform.eulerAngles == upDirection.eulerAngles)
            {
                //print("Coming up");
                // Get next block in a direction
                float tile1DistToTarget = Vector3.Distance(GetRightBlockPos(), targetPos);
                float tile2DistToTarget = Vector3.Distance(GetForwardBlockPos(), targetPos);

                if (tile1DistToTarget < tile2DistToTarget)
                {
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
            }
            // Coming down ╞
            else if (transform.eulerAngles == downDirection.eulerAngles)
            {
                //print("Coming down");
                // Get next block in a direction
                float tile1DistToTarget = Vector3.Distance(GetLeftBlockPos(), targetPos);
                float tile2DistToTarget = Vector3.Distance(GetForwardBlockPos(), targetPos);
                //print("Coming down");

                if (tile1DistToTarget < tile2DistToTarget)
                {
                    //print("Coming down");
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
            }
            // Coming left ╞
            else if (transform.eulerAngles == leftDirection.eulerAngles)
            {
                // Get next block
                //print("Coming left");
                float tile1DistToTarget = Vector3.Distance(GetLeftBlockPos(), targetPos);
                float tile2DistToTarget = Vector3.Distance(GetRightBlockPos(), targetPos);

                if (tile1DistToTarget < tile2DistToTarget)
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
            }
        }

        // DECISION BLOCK - LEFT RIGHT UP ╨
        else if (nextBlockType == BlockType.leftRightUp)
        {
            // Coming down ╨
            if (transform.eulerAngles == downDirection.eulerAngles)
            {
                //print("Coming down");
                // Get next block in a direction
                float tile1DistToTarget = Vector3.Distance(GetLeftBlockPos(), targetPos);
                float tile2DistToTarget = Vector3.Distance(GetRightBlockPos(), targetPos);

                if (tile1DistToTarget < tile2DistToTarget)
                {
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
            }
            // Coming down ╨
            else if (transform.eulerAngles == leftDirection.eulerAngles)
            {
                //print("Coming left");
                // Get next block in a direction
                float tile1DistToTarget = Vector3.Distance(GetRightBlockPos(), targetPos);
                float tile2DistToTarget = Vector3.Distance(GetForwardBlockPos(), targetPos);

                if (tile1DistToTarget < tile2DistToTarget)
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
            }
            // Coming right ╨
            else if (transform.eulerAngles == rightDirection.eulerAngles)
            {
                //print("Coming right");
                // Get next block in a direction
                float tile1DistToTarget = Vector3.Distance(GetLeftBlockPos(), targetPos);
                float tile2DistToTarget = Vector3.Distance(GetForwardBlockPos(), targetPos);

                if (tile1DistToTarget < tile2DistToTarget)
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
                else
                {
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
            }

        }

        // DECISION BLOCK - CROSS ╬
        else if (nextBlockType == BlockType.cross)
        {
            //print("=>>>>>> CROSS ");
            // Coming down ╬
            if (transform.eulerAngles == downDirection.eulerAngles)
            {

                // Get next block in a direction
                float tile1DistToTarget = Vector3.Distance(GetLeftBlockPos(), targetPos);
                float tile2DistToTarget = Vector3.Distance(GetRightBlockPos(), targetPos);
                float tile3DistToTarget = Vector3.Distance(GetForwardBlockPos(), targetPos);

                float min = Mathf.Min(Mathf.Min(tile1DistToTarget, tile2DistToTarget), tile3DistToTarget);

                if (tile1DistToTarget == min)
                {
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
                else if (tile2DistToTarget == min)
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
                else if (tile3DistToTarget == min)
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
            }
            // Coming up ╬
            else if (transform.eulerAngles == upDirection.eulerAngles)
            {
                //print("Coming up");
                // Get next block in a direction
                float tile1DistToTarget = Vector3.Distance(GetRightBlockPos(), targetPos);
                float tile2DistToTarget = Vector3.Distance(GetForwardBlockPos(), targetPos);
                float tile3DistToTarget = Vector3.Distance(GetLeftBlockPos(), targetPos);

                float min = Mathf.Min(Mathf.Min(tile1DistToTarget, tile2DistToTarget), tile3DistToTarget);

                if (tile1DistToTarget == min)
                {
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
                else if (tile2DistToTarget == min)
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
                else if (tile3DistToTarget == min)
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
            }
            // Coming left ╬
            else if (transform.eulerAngles == leftDirection.eulerAngles)
            {
                // print("Coming left");
                // Get next block in a direction
                float tile1DistToTarget = Vector3.Distance(GetRightBlockPos(), targetPos);
                float tile2DistToTarget = Vector3.Distance(GetForwardBlockPos(), targetPos);
                float tile3DistToTarget = Vector3.Distance(GetLeftBlockPos(), targetPos);

                float min = Mathf.Min(Mathf.Min(tile1DistToTarget, tile2DistToTarget), tile3DistToTarget);

                if (tile1DistToTarget == min)
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
                else if (tile2DistToTarget == min)
                {
                    transform.eulerAngles = leftDirection.eulerAngles;
                }
                else if (tile3DistToTarget == min)
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
            }
            // Coming right ╬
            else if (transform.eulerAngles == rightDirection.eulerAngles)
            {
                //print("Coming right");
                // Get next block in a direction
                float tile1DistToTarget = Vector3.Distance(GetRightBlockPos(), targetPos);
                float tile2DistToTarget = Vector3.Distance(GetForwardBlockPos(), targetPos);
                float tile3DistToTarget = Vector3.Distance(GetLeftBlockPos(), targetPos);

                float min = Mathf.Min(Mathf.Min(tile1DistToTarget, tile2DistToTarget), tile3DistToTarget);

                if (tile1DistToTarget == min)
                {
                    transform.eulerAngles = downDirection.eulerAngles;
                }
                else if (tile2DistToTarget == min)
                {
                    transform.eulerAngles = rightDirection.eulerAngles;
                }
                else if (tile3DistToTarget == min)
                {
                    transform.eulerAngles = upDirection.eulerAngles;
                }
            }

        }
    }

    BlockType GetNextBlockType(GameObject tile)
    {
        //string blockName = tile.transform.name;
        string blockName = tile.transform.tag;

        switch (blockName)
        {
            // DECISION BLOCKS ///////////////////////////////////////
            case "LeftUpDown":
                return BlockType.leftUpDown;
                break;
            case "RightUpDown":
                return BlockType.rightUpDown;
                break;
            case "LeftRightDown":
                return BlockType.leftRightDown;
                break;
            case "LeftRightUp":
                return BlockType.leftRightUp;
                break;
            case "Cross":
                return BlockType.cross;
                break;
            // DECISION BLOCKS ///////////////////////////////////////
            case "DownLeft":
                return BlockType.downLeft;
                break;
            case "UpRight":
                return BlockType.upRight;
                break;
            case "UpLeft":
                return BlockType.upLeft;
                break;
            case "DownRight":
                return BlockType.downRight;
                break;
            case "Reverse":
                return BlockType.reverse;
                break;
            case "LeftRight":
                return BlockType.leftRight;
                break;
            case "NA":
                return BlockType.none;
                break;
            default:
                // print("ERROR GetTileType");
                return BlockType.none;
                break;
        }

    }

    private void MoveForward()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private void Move(GameObject nextTile)
    {
        if (Mathf.Abs(Vector3.Distance(transform.position, nextTile.transform.position)) >= 0.001f)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(
                transform.position,
                nextTile.transform.position,
                step);
        }
        else
        {
            canMove = false;

        }
    }
    GameObject GetForwardBlock()
    {
        RaycastHit hit;
        Vector3 offsetStart = transform.position + transform.up / 2;
        Ray ray = new Ray(offsetStart, transform.forward);
        if (Physics.Raycast(ray, out hit, rayCastLength))
        {
            return (hit.transform.gameObject);
        }
        return null;
    }
    private Vector3 GetForwardBlockPos()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.yellow, 3f);
        return (transform.position + transform.forward);

    }
    private Vector3 GetLeftBlockPos()
    {
        Debug.DrawRay(transform.position, -transform.right, Color.cyan, 3f);

        return (transform.position + -transform.right);
    }
    private Vector3 GetRightBlockPos()
    {
        Debug.DrawRay(transform.position, transform.right, Color.red, 3f);
        return (transform.position + transform.right);

    }
    public void ToggleGhostMode()
    {
        isChasemode = !isChasemode;
        anim.SetBool("isChaseMode", isChasemode);
        if (isChasemode)
        {

        }
        else
        {

        }
        //print("Toggle Chase Mode = " + isChasemode + " for " + transform.name);
    }

    private void OnDrawGizmos()
    {
        Vector3 offsetStart = transform.position + transform.up / 2;
        Gizmos.DrawRay(offsetStart, transform.forward);
    }
}
