using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class RoomGrid : MonoBehaviour
{
    public Vector2[] doorDir;
    public int width;
    public int length;
    private DungeonGeneratorGrid generator;
    private SetEmpty[] setEmpty;
    int[,] grid;

    private void Start()
    {
        generator = GameObject.FindGameObjectWithTag("DungeonGenerator").GetComponent<DungeonGeneratorGrid>();

        // if the room is a 2x2 it sets 0 for the slots as when a 2x2 spawns, it takes up a 3x3 space
        setEmpty = gameObject.GetComponentsInChildren<SetEmpty>();
        for(int i = 0; i < setEmpty.Length; i++)
        {
            generator.setEmpty(setEmpty[i].gameObject.transform.position);
        }

        grid = generator.grid;
        List<Vector2> avilableSlots;
        avilableSlots = findAdjSpaces(grid, (int)transform.position.x / generator.offset, (int)transform.position.y / generator.offset);

        // for all available slots add a room
        for(int i = 0; i < avilableSlots.Count; i++)
        {
            if (generator.rooms.Count < generator.roomLimit) // checks the roomlimit 
            {
                // left -1 0 right 1 0 up 0 1 down 0 -1
                if (avilableSlots[i].x / generator.offset - (int)transform.position.x / generator.offset == -1)  // left
                {
                    spawnRoom(avilableSlots, generator.leftRooms, 0, i, new Vector2(-1, 0));
                }
                else if (avilableSlots[i].x / generator.offset - (int)transform.position.x / generator.offset == 1) // right
                {
                    spawnRoom(avilableSlots, generator.rightRooms, 0, i, new Vector2(1, 0));
                }
                else if (avilableSlots[i].y / generator.offset - (int)transform.position.y / generator.offset == 1) // top
                {
                    spawnRoom(avilableSlots, generator.topRooms, 2, i, new Vector2(0, 1));
                }
                else if (avilableSlots[i].y / generator.offset - (int)transform.position.y / generator.offset == -1) // bottom
                {
                    spawnRoom(avilableSlots, generator.bottomRooms, 0, i, new Vector2(0, -1));
                }
            } else {
                Instantiate(generator.closedRoom, avilableSlots[i], Quaternion.identity); // a closed room
            }
        }
    }

    // Spawns a room
    public void spawnRoom(List<Vector2> avilableSlots, GameObject[] rooms, int amountOf2x2, int index, Vector2 direction)
    {
        // chooses a random room
        int random = Random.Range(0, rooms.Length);
        if (random >= rooms.Length - amountOf2x2) // checks if its a 2x2 room
        {
            List<Vector2> avilableSlots_ = findAdjSpacesForLarge3x3(grid, (int)transform.position.x / generator.offset, (int)transform.position.y / generator.offset, direction); // final available slots
            if (avilableSlots_.Count != 0) // if there are slots
            {
                generator.updateGrid(new Vector2(avilableSlots_[0].x / generator.offset, avilableSlots_[0].y / generator.offset));
                generator.updateGrid(avilableSlots_[1]);
                generator.updateGrid(avilableSlots_[2]);
                generator.updateGrid(avilableSlots_[3]);
                generator.updateGrid(avilableSlots_[4]);
                generator.updateGrid(avilableSlots_[5]);
                generator.updateGrid(avilableSlots_[6]);
                generator.updateGrid(avilableSlots_[7]);
                generator.updateGrid(avilableSlots_[8]);

                GameObject room = Instantiate(rooms[random], avilableSlots_[0], Quaternion.identity);
                generator.rooms.Add(room);
            }
            else // add 1x1 room
            {
                GameObject roomBig = Instantiate(rooms[Random.Range(0, rooms.Length - amountOf2x2)], avilableSlots[index], Quaternion.identity);
                generator.updateGrid(new Vector2(avilableSlots[index].x / generator.offset, avilableSlots[index].y / generator.offset));
                generator.rooms.Add(roomBig);
            }
        }
        else // add 1x1 room
        {
            GameObject roomBig = Instantiate(rooms[Random.Range(0, rooms.Length - amountOf2x2)], avilableSlots[index], Quaternion.identity);
            generator.updateGrid(new Vector2(avilableSlots[index].x / generator.offset, avilableSlots[index].y / generator.offset));
            generator.rooms.Add(roomBig);
        }
    }

    // Find slots for 1x1 rooms
    public List<Vector2> findAdjSpaces(int[,] grid, int indexX, int indexY) 
    {
        List<Vector2> list = new List<Vector2>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if ((i == -1 && j == -1) || (i == -1 && j == 1) || (i == 0 && j == 0) || (i == 1 && j == -1) || (i == 1 && j == 1)) // skips center value and corner value
                {
                    continue;
                }

                int x = indexX + i; // calculate x index
                int y = indexY + j; // calculate y index

                if (x < 0 || x >= generator.width || y < 0 || y >= generator.height) // check for out-of-bounds
                {
                    continue;
                }

                if (grid[x, y] == 1) // checks if there is a room
                {
                    continue;
                }

                for(int k = 0; k < doorDir.Length; k++)
                {
                    if (doorDir[k] == new Vector2(i, j)) // get values for door position
                    {
                        list.Add(new Vector2(x * generator.offset, y * generator.offset));
                    }
                }
            }
        }
        return list;
    }

    // Find slots for a 3x3 
    public List<Vector2> findAdjSpacesForLarge3x3(int[,] grid, int indexX, int indexY, Vector2 direction)
    {
        List<Vector2> list = new List<Vector2>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if ((i == -1 && j == -1) || (i == -1 && j == 1) || (i == 0 && j == 0) || (i == 1 && j == -1) || (i == 1 && j == 1)) // skips center value and corner value
                {
                    continue;
                }

                int x = indexX + i; // calculate x index
                int y = indexY + j; // calculate y index

                if (x < 0 || x >= generator.width || y < 0 || y >= generator.height) // check for out-of-bounds
                {
                    continue;
                }

                Vector2[] spaces = new Vector2[9];
                if((i == 0 && j == -1) || (i == 0 && j == 1)) // get the positions of the space that could possible fit a 2x2 room in a 3x3 space
                {
                    spaces[0] = new Vector2(x, y);
                    spaces[1] = new Vector2(x + j, y);
                    spaces[2] = new Vector2(x - j, y);
                    spaces[3] = new Vector2(x, y + j);
                    spaces[4] = new Vector2(x + j, y + j);
                    spaces[5] = new Vector2(x - j, y + j);
                    spaces[6] = new Vector2(x, y + j + j);
                    spaces[7] = new Vector2(x + j, y + j + j);
                    spaces[8] = new Vector2(x - j, y + j + j);

                    // check if out-of-bounds
                    if (((int)spaces[0].x < 0 || (int)spaces[0].x >= generator.width || (int)spaces[0].y < 0 || (int)spaces[0].y >= generator.height) ||
                        ((int)spaces[1].x < 0 || (int)spaces[1].x >= generator.width || (int)spaces[1].y < 0 || (int)spaces[1].y >= generator.height) ||
                        ((int)spaces[2].x < 0 || (int)spaces[2].x >= generator.width || (int)spaces[2].y < 0 || (int)spaces[2].y >= generator.height) ||
                        ((int)spaces[3].x < 0 || (int)spaces[3].x >= generator.width || (int)spaces[3].y < 0 || (int)spaces[3].y >= generator.height) ||
                        ((int)spaces[4].x < 0 || (int)spaces[4].x >= generator.width || (int)spaces[4].y < 0 || (int)spaces[4].y >= generator.height) ||
                        ((int)spaces[5].x < 0 || (int)spaces[5].x >= generator.width || (int)spaces[5].y < 0 || (int)spaces[5].y >= generator.height) ||
                        ((int)spaces[6].x < 0 || (int)spaces[6].x >= generator.width || (int)spaces[6].y < 0 || (int)spaces[6].y >= generator.height) ||
                        ((int)spaces[7].x < 0 || (int)spaces[7].x >= generator.width || (int)spaces[7].y < 0 || (int)spaces[7].y >= generator.height) ||
                        ((int)spaces[8].x < 0 || (int)spaces[8].x >= generator.width || (int)spaces[8].y < 0 || (int)spaces[8].y >= generator.height))

                    {
                        continue;
                    }

                    // checks if slot is taken
                    if((grid[(int)spaces[0].x, (int)spaces[0].y] == 1) ||
                       (grid[(int)spaces[1].x, (int)spaces[1].y] == 1) ||
                       (grid[(int)spaces[2].x, (int)spaces[2].y] == 1) ||
                       (grid[(int)spaces[3].x, (int)spaces[3].y] == 1) ||
                       (grid[(int)spaces[4].x, (int)spaces[4].y] == 1) ||
                       (grid[(int)spaces[5].x, (int)spaces[5].y] == 1) ||
                       (grid[(int)spaces[6].x, (int)spaces[6].y] == 1) ||
                       (grid[(int)spaces[7].x, (int)spaces[7].y] == 1) ||
                       (grid[(int)spaces[8].x, (int)spaces[8].y] == 1))
                    {
                        continue;
                    }

                    // add if its the right door direction
                    if (direction == new Vector2(i, j))
                    {
                        list.Add(new Vector2(x * generator.offset, y * generator.offset));
                        list.Add(spaces[1]);
                        list.Add(spaces[2]);
                        list.Add(spaces[3]);
                        list.Add(spaces[4]);
                        list.Add(spaces[5]);
                        list.Add(spaces[6]);
                        list.Add(spaces[7]);
                        list.Add(spaces[8]);
                    }

                } else if ((i == 1 && j == 0) || (i == -1 && j == 0)) {
                    spaces[0] = new Vector2(x, y);
                    spaces[1] = new Vector2(x, y + j);
                    spaces[2] = new Vector2(x, y - j);
                    spaces[3] = new Vector2(x + j, y);
                    spaces[4] = new Vector2(x + j, y + j);
                    spaces[5] = new Vector2(x + j, y - j);
                    spaces[6] = new Vector2(x + j + j, y);
                    spaces[7] = new Vector2(x + j + j, y + j);
                    spaces[8] = new Vector2(x + j + j, y - j);

                    // check if out-of-bounds
                    if (((int)spaces[0].x < 0 || (int)spaces[0].x >= generator.width || (int)spaces[0].y < 0 || (int)spaces[0].y >= generator.height) ||
                        ((int)spaces[1].x < 0 || (int)spaces[1].x >= generator.width || (int)spaces[1].y < 0 || (int)spaces[1].y >= generator.height) ||
                        ((int)spaces[2].x < 0 || (int)spaces[2].x >= generator.width || (int)spaces[2].y < 0 || (int)spaces[2].y >= generator.height) ||
                        ((int)spaces[3].x < 0 || (int)spaces[3].x >= generator.width || (int)spaces[3].y < 0 || (int)spaces[3].y >= generator.height) ||
                        ((int)spaces[4].x < 0 || (int)spaces[4].x >= generator.width || (int)spaces[4].y < 0 || (int)spaces[4].y >= generator.height) ||
                        ((int)spaces[5].x < 0 || (int)spaces[5].x >= generator.width || (int)spaces[5].y < 0 || (int)spaces[5].y >= generator.height) ||
                        ((int)spaces[6].x < 0 || (int)spaces[6].x >= generator.width || (int)spaces[6].y < 0 || (int)spaces[6].y >= generator.height) ||
                        ((int)spaces[7].x < 0 || (int)spaces[7].x >= generator.width || (int)spaces[7].y < 0 || (int)spaces[7].y >= generator.height) ||
                        ((int)spaces[8].x < 0 || (int)spaces[8].x >= generator.width || (int)spaces[8].y < 0 || (int)spaces[8].y >= generator.height))

                    {
                        continue;
                    }

                    // checks if slot is taken
                    if ((grid[(int)spaces[0].x, (int)spaces[0].y] == 1) ||
                       (grid[(int)spaces[1].x, (int)spaces[1].y] == 1) ||
                       (grid[(int)spaces[2].x, (int)spaces[2].y] == 1) ||
                       (grid[(int)spaces[3].x, (int)spaces[3].y] == 1) ||
                       (grid[(int)spaces[4].x, (int)spaces[4].y] == 1) ||
                       (grid[(int)spaces[5].x, (int)spaces[5].y] == 1) ||
                       (grid[(int)spaces[6].x, (int)spaces[6].y] == 1) ||
                       (grid[(int)spaces[7].x, (int)spaces[7].y] == 1) ||
                       (grid[(int)spaces[8].x, (int)spaces[8].y] == 1))
                    {
                        continue;
                    }

                    // add if its the right door direction
                    if (direction == new Vector2(i, j))
                    {
                        list.Add(new Vector2(x * generator.offset, y * generator.offset));
                        list.Add(spaces[1]);
                        list.Add(spaces[2]);
                        list.Add(spaces[3]);
                        list.Add(spaces[4]);
                        list.Add(spaces[5]);
                        list.Add(spaces[6]);
                        list.Add(spaces[7]);
                        list.Add(spaces[8]);
                    }
                }
            }
        }
        return list;
    }
}
