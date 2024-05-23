using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    // Define an array of room shape prefabs
    public GameObject[] roomShapes;
    public GameObject corridorPrefab;
    public int corridorWidth;
    public int numRooms = 10;
    public int minX = -10;
    public int maxX = 10;
    public int minY = -10;
    public int maxY = 10;
    private List<GameObject> rooms = new List<GameObject>();
    public Sprite sprite;

    // Start is called before the first frame update
    void Start()
    {
        GenerateDungeonLayout();
        createCorridors();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Generate a dungeon layout
    public void GenerateDungeonLayout()
    {
        // Create a new empty game object to serve as the parent of the dungeon layout
        GameObject dungeonLayout = new GameObject("DungeonLayout");
        List<Bounds[]> boundsList = new List<Bounds[]>();

        // Loop through a predetermined number of times to create a series of interconnected rooms
        for (int i = 0; i < numRooms; i++)
        {

            // Instantiate a random room shape prefab
            GameObject roomShape = Instantiate(roomShapes[Random.Range(0, roomShapes.Length)]);

            // Position the room shape randomly within the dungeon layout
            roomShape.transform.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0f);

            //Update door position
            Room room = roomShape.GetComponent<Room>();
            room.updateRooms(roomShape.transform.position);

            SpriteRenderer[] childRenderer = roomShape.GetComponentsInChildren<SpriteRenderer>();
            Bounds[] boundsChild = new Bounds[childRenderer.Length];

            bool overlap = true;
            for(int j = 0; j < childRenderer.Length; j++)
            {
                Bounds bounds = childRenderer[j].bounds;
                foreach (Bounds[] boundC in boundsList)
                {
                    for(int k = 0; k < boundC.Length; k++)
                    {
                        if (bounds.Intersects(boundC[k]))
                        {
                            overlap = false;
                        }
                    }
                }
                boundsChild[j] = bounds;
            }

            if(overlap)
            {
                rooms.Add(roomShape);
                boundsList.Add(boundsChild);
                // Add the room shape to the dungeon layout
                roomShape.transform.parent = dungeonLayout.transform;
            } else
            {
                Destroy(roomShape);
                i--;
                Debug.Log("hit");
            }
        }
    }

    public void createCorridors()
    { 
        foreach (GameObject roomOne in rooms)
        {
            float length = float.MaxValue;
            List<GameObject> corridors = new List<GameObject>();
            foreach(GameObject roomTwo in rooms)
            {
                if(roomOne != roomTwo)
                {
                    length = closestDoor(roomOne.GetComponent<Room>().doorPositions, roomTwo.GetComponent<Room>().doorPositions, length, corridors);
                }
            }
        }
    }

    public float closestDoor(List<Vector2> roomDoorListA, List<Vector2> roomDoorListB, float length, List<GameObject> corridors)
    {
        float closestDistance = float.MaxValue;
        Vector2 closestDoorA = Vector2.zero;
        Vector2 closestDoorB = Vector2.zero;

        foreach (Vector2 doorAS in roomDoorListA)
        {
            foreach (Vector2 doorBS in roomDoorListB)
            {
                float distance = Vector2.Distance(doorAS, doorBS);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestDoorA = doorAS;
                    closestDoorB = doorBS;
                }
            }
        }
        
        // Checks if cooridor is the shortest from the rest of the cooridors 
        if(closestDistance > length)
        {
            for(int i = 0; i < corridors.Count - 1; i++)
            {
                Destroy(corridors[i]);
            }
            return length;
        } else
        {
            GameObject corridor = new GameObject("corridor");

            // Calculate the direction vector
            Vector2 direction = (closestDoorB - closestDoorA).normalized;

            // Calculate the rotation in degrees
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


            corridor.transform.position = new Vector2((closestDoorA.x + closestDoorB.x) / 2, (closestDoorA.y + closestDoorB.y) / 2);
            corridor.transform.localScale = new Vector2(closestDistance * 7, 2);
            corridor.transform.rotation = Quaternion.Euler(0, 0, angle);
            SpriteRenderer spriteRender = corridor.AddComponent<SpriteRenderer>();
            spriteRender.sprite = sprite;

            corridors.Add(corridor);

            return closestDistance;
        }
    }
}
