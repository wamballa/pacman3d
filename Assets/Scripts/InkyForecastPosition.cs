using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InkyForecastPosition : ForecastPosition
{
    bool hasNextBlock = false;
    bool canMove = false;
    GameObject nextBlock;

    // Start is called before the first frame update
    void Start()
    {
        direction = Direction.right;
    }

    public Vector3 GetInkyTargetBlock(GameObject currentBlock)
    {
        if (currentBlock != null)
        {
            //print("FC: immediately rotate");
            HandleRotationsBasedOnPlayerPosition(currentBlock);
        }


        Vector3 targetPos = new Vector3();

        for (int i = 0; i < 2; i++)
        {

            // Get Next Block
            nextBlock = GetForwardBlock();

            if (nextBlock != null)
            {
                //transform.position = nextBlock.transform.position;
                //print(i + " " + nextBlock.tag);
            }
                

            targetPos = MoveAlongPath();

            if (nextBlock != null)
            {
                // Handle Rotation Based On Random
                HandleRotationsBasedOnPlayerPosition(nextBlock);
                //currentBlock = nextBlock;
            }

        }

        return targetPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward);

    }

}
