using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    public Transform player;
    private void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        newPosition.x = transform.position.x;
        if (newPosition.z < -5.7f) newPosition.z = -5.7f;
        else if (newPosition.z > 2.62f) newPosition.z = 2.62f;
        transform.position = newPosition;
    }
}
