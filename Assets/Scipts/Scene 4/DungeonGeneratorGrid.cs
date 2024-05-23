using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DungeonGeneratorGrid : MonoBehaviour
{
    public GameObject[] bottomRooms;
    public GameObject[] topRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;

    public GameObject closedRoom;

    public int width;
    public int height;
    public int roomLimit;
    public int offset;
    public int[,] grid;

    public List<GameObject> rooms;

    public GameObject[] roomTypes;

    public bool showOnes = true;

    public float timer = 5;
    // Start is called before the first frame update
    void Start()
    {
        // create grid
        grid = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                grid[i, j] = 0;
            }
        }

        // add room in the middle of the grid
        GameObject room = Instantiate(roomTypes[0], new Vector2((width / 2) * offset, (height / 2) * offset), Quaternion.identity);
        grid[(width / 2), (height / 2)] = 1;
        rooms.Add(room);
    }

    // Update is called once per frame
    void Update()
    {
        if(showOnes)
        {
            timer -= Time.deltaTime;
        }

        // restarts map generation by restarting scene
        if(timer < 0)
        {
            SceneManager.LoadScene(1);
        }

        // Prints rooms, up is left 
        if(rooms.Count == roomLimit && showOnes)
        {
            printGrid();
            showOnes = false;
        }
    }

    // prints grid
    public void printGrid()
    {
        for (int i = height - 1; i >= 0; i--)
        {
            string row = "";
            for (int j = 0; j < height; j++)
            {
                row += grid[j, i] + " ";
            }
            Debug.Log(row);
        }
    }

    // function for a placed room to update the grid
    public void updateGrid(Vector2 index)
    {
        grid[(int)index.x, (int)index.y] = 1;
    }

    // set empty slot in the grid as 0
    public void setEmpty(Vector2 index)
    {
        grid[(int)index.x / offset, (int)index.y / offset] = 0;
    }
}
