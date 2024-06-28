using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject floor;
    public Sprite[] floorSprites;
    public GameObject wall;
    public Sprite[] wallSprites;
    public int roomCount;
    public Vector2Int roomSizeMin;
    public Vector2Int roomSizeMax;
    public int gridWidth;
    public int gridHeight;
    public GameObject player; // Ссылка на объект игрока
    public GameObject exitPrefab; // Ссылка на префаб выхода
    public GameObject stairsDownPrefab; // Ссылка на префаб лестницы вниз

    private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

    void Awake()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        List<RectInt> rooms = GenerateRooms();

        ConnectRooms(rooms);

        CreateWalls();

        InstantiateTiles(floor, floorPositions);
        InstantiateTiles(wall, wallPositions);

        // Размещение игрока и префабов
        PlacePlayerAndPrefabs(rooms);
    }

    List<RectInt> GenerateRooms()
    {
        List<RectInt> rooms = new List<RectInt>();

        for (int i = 0; i < roomCount; i++)
        {
            RectInt newRoom = GetNewRoom();
            if (!RoomsOverlap(newRoom, rooms))
            {
                rooms.Add(newRoom);
                CreateRoom(newRoom);
            }
        }

        return rooms;
    }

    RectInt GetNewRoom()
    {
        int width = Random.Range(roomSizeMin.x, roomSizeMax.x);
        int height = Random.Range(roomSizeMin.y, roomSizeMax.y);
        int x = Random.Range(0, gridWidth - width);
        int y = Random.Range(0, gridHeight - height);

        return new RectInt(x, y, width, height);
    }

    bool RoomsOverlap(RectInt newRoom, List<RectInt> rooms)
    {
        foreach (RectInt room in rooms)
        {
            if (newRoom.Overlaps(room))
            {
                return true;
            }
        }
        return false;
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

    void ConnectRooms(List<RectInt> rooms)
    {
        for (int i = 1; i < rooms.Count; i++)
        {
            Vector2Int pointA = GetRandomPointInRoom(rooms[i - 1]);
            Vector2Int pointB = GetRandomPointInRoom(rooms[i]);

            // Connect only horizontally or vertically between adjacent rooms
            CreateCorridor(pointA, new Vector2Int(pointB.x, pointA.y));
            CreateCorridor(new Vector2Int(pointB.x, pointA.y), pointB);
        }
    }

    Vector2Int GetRandomPointInRoom(RectInt room)
    {
        int x = Random.Range(room.xMin, room.xMax);
        int y = Random.Range(room.yMin, room.yMax);
        return new Vector2Int(x, y);
    }

    void CreateCorridor(Vector2Int pointA, Vector2Int pointB)
    {
        Vector2Int currentPos = pointA;

        while (currentPos != pointB)
        {
            if (currentPos.x != pointB.x)
            {
                AddCorridorSection(currentPos, true);
                currentPos.x += (int)Mathf.Sign(pointB.x - currentPos.x);
            }
            else if (currentPos.y != pointB.y)
            {
                AddCorridorSection(currentPos, false);
                currentPos.y += (int)Mathf.Sign(pointB.y - currentPos.y);
            }
        }
    }

    void AddCorridorSection(Vector2Int start, bool horizontal)
    {
        if (horizontal)
        {
            floorPositions.Add(start);
            floorPositions.Add(start + Vector2Int.up);
        }
        else
        {
            floorPositions.Add(start);
            floorPositions.Add(start + Vector2Int.right);
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

                    Vector2Int neighborPos = pos + new Vector2Int(x, y);
                    if (!floorPositions.Contains(neighborPos))
                    {
                        wallPositions.Add(neighborPos);
                    }
                }
            }
        }
    }

    void InstantiateTiles(GameObject prefab, HashSet<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions)
        {
            var tile = Instantiate(prefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
            tile.GetComponent<SpriteRenderer>().sprite = floorSprites[Random.Range(0, floorSprites.Length)];

            if (prefab == floor)
            {
                tile.GetComponent<SpriteRenderer>().sprite = floorSprites[Random.Range(0, floorSprites.Length)];
            }
            else if (prefab == wall)
            {
                tile.GetComponent<SpriteRenderer>().sprite = wallSprites[Random.Range(0, wallSprites.Length)];
            }
        }
    }

    void PlacePlayerAndPrefabs(List<RectInt> rooms)
    {
        if (rooms.Count > 0)
        {
            // Размещение игрока и выхода в первой комнате
            Vector2Int startRoomCenter = GetRoomCenter(rooms[0]);
            player.transform.position = new Vector3(startRoomCenter.x, startRoomCenter.y, 0);
            Instantiate(exitPrefab, new Vector3(startRoomCenter.x + 1, startRoomCenter.y, 0), Quaternion.identity);

            // Найти самую далекую комнату от стартовой
            RectInt furthestRoom = GetFurthestRoom(startRoomCenter, rooms);
            Vector2Int furthestRoomCenter = GetRoomCenter(furthestRoom);
            Instantiate(stairsDownPrefab, new Vector3(furthestRoomCenter.x, furthestRoomCenter.y, 0), Quaternion.identity);
        }
    }

    Vector2Int GetRoomCenter(RectInt room)
    {
        int x = (room.xMin + room.xMax) / 2;
        int y = (room.yMin + room.yMax) / 2;
        return new Vector2Int(x, y);
    }

    RectInt GetFurthestRoom(Vector2Int startRoomCenter, List<RectInt> rooms)
    {
        RectInt furthestRoom = rooms[0];
        float maxDistance = 0f;

        foreach (RectInt room in rooms)
        {
            Vector2Int roomCenter = GetRoomCenter(room);
            float distance = Vector2Int.Distance(startRoomCenter, roomCenter);

            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestRoom = room;
            }
        }

        return furthestRoom;
    }
}
