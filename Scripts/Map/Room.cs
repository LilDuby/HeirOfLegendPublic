using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public RectInt roomRect;
    public List<Door> doors = new List<Door>();

    public Room(RectInt roomRect)
    {
        this.roomRect = roomRect;
    }

    public void AddDoor(Door door)
    {
        doors.Add(door);
    }
}
