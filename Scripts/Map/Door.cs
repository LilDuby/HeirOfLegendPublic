using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector2Int position;
    public Room connectedRoom;

    public Door(Vector2Int position, Room connectedRoom)
    {
        this.position = position;
        this.connectedRoom = connectedRoom;
    }
}
