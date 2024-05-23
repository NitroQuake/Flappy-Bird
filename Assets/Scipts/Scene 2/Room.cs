using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Vector2> doorPositions;

    public void updateRooms(Vector2 pos)
    {
        for(int i = 0; i < doorPositions.Count; i++)
        {
            doorPositions[i] = new Vector2(pos.x + doorPositions[i].x, pos.y + doorPositions[i].y);
        }
    }
}
