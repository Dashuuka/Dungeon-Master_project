using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject floor;
    public GameObject wall;
    public int roomCount;
    public Vector2Int roomSizeMin;
    public Vector2Int roomSizeMax;
    public int gridWidth;
    public int gridHeight;

    private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

    void Awake()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        List<RectInt> rooms = new List<RectInt>();

        for (int i = 0; i < roomCount; i++)
        {
            RectInt newRoom = GenerateRoom();

            bool overlap = false;
            foreach (RectInt room in rooms)
            {
                if (newRoom.Overlaps(room))
                {
                    overlap = true;
                    break;
                }
            }

            if (!overlap)
            {
                rooms.Add(newRoom);
                CreateRoom(newRoom);
            }
        }

        for (int i = 1; i < rooms.Count; i++)
        {
            Vector2Int pointA = GetRandomPointInRoom(rooms[i - 1]);
            Vector2Int pointB = GetRandomPointInRoom(rooms[i]);
            CreateCorridor(pointA, pointB);
        }

        CreateWalls();

        foreach (Vector2Int pos in floorPositions)
        {
            Instantiate(floor, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        }

        foreach (Vector2Int pos in wallPositions)
        {
            Instantiate(wall, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        }
    }

    RectInt GenerateRoom()
    {
        int width = Random.Range(roomSizeMin.x, roomSizeMax.x);
        int height = Random.Range(roomSizeMin.y, roomSizeMax.y);
        int x = Random.Range(0, gridWidth - width);
        int y = Random.Range(0, gridHeight - height);

        return new RectInt(x, y, width, height);
    }

    Vector2Int GetRandomPointInRoom(RectInt room)
    {
        int x = Random.Range(room.xMin, room.xMax);
        int y = Random.Range(room.yMin, room.yMax);
        return new Vector2Int(x, y);
    }

    void CreateRoom(RectInt room)
    {
        for (int x = room.xMin; x < room.xMax; x++)
        {
            for (int y = room.yMin; y < room.yMax; y++)
            {
                floorPositions.Add(new Vector2Int(x, y));
            }
        }
    }

    void CreateCorridor(Vector2Int pointA, Vector2Int pointB)
    {
        Vector2Int currentPos = pointA;

        while (currentPos != pointB)
        {
            if (currentPos.x != pointB.x)
            {
                if (currentPos.x < pointB.x)
                {
                    AddCorridorSection(new Vector2Int(currentPos.x, currentPos.y), true);
                    currentPos.x++;
                }
                else
                {
                    AddCorridorSection(new Vector2Int(currentPos.x - 1, currentPos.y), true);
                    currentPos.x--;
                }
            }
            else if (currentPos.y != pointB.y)
            {
                if (currentPos.y < pointB.y)
                {
                    AddCorridorSection(new Vector2Int(currentPos.x, currentPos.y), false);
                    currentPos.y++;
                }
                else
                {
                    AddCorridorSection(new Vector2Int(currentPos.x, currentPos.y - 1), false);
                    currentPos.y--;
                }
            }
        }
    }

    void AddCorridorSection(Vector2Int start, bool horizontal)
    {
        if (horizontal)
        {
            floorPositions.Add(start);
            floorPositions.Add(new Vector2Int(start.x, start.y + 1));
        }
        else
        {
            floorPositions.Add(start);
            floorPositions.Add(new Vector2Int(start.x + 1, start.y));
        }
    }

    void CreateWalls()
    {
        foreach (Vector2Int pos in floorPositions)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    
                    Vector2Int neighborPos = new Vector2Int(pos.x + x, pos.y + y);
                    if (!floorPositions.Contains(neighborPos))
                    {
                        wallPositions.Add(neighborPos);
                    }
                }
            }
        }
    }
}
