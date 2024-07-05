using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [System.Serializable]
    public class Floor
    {
        public int minEnemies;
        public int maxEnemies;
        public List<GameObject> enemyPrefabs;
        public Color floorColor;
        public Color wallColor;
        public List<GameObject> randomDecorations;
        public List<GameObject> regularDecorations;
        public int minRandomDecorations;
        public int maxRandomDecorations;
        public int minRegularDecorations;
        public int maxRegularDecorations;
        public AudioClip backgroundMusic;  // Добавьте поле для музыки
    }

    [Header("Generation Settings")]
    public int roomCount;
    public Vector2Int roomSizeMin;
    public Vector2Int roomSizeMax;
    public int gridWidth;
    public int gridHeight;

    [Header("Objects and Sprites")]
    public GameObject floor;
    public Sprite[] floorSprites;
    public GameObject wall;
    public Sprite[] wallSprites;
    public GameObject player;
    public GameObject stairsDownPrefab;

    [Header("Floors")]
    public List<Floor> floors;

    [Header("Data structures")]
    private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> decorationPositions = new HashSet<Vector2Int>();

    private int currentFloorIndex = 0;
    private List<GameObject> instantiatedObjects = new List<GameObject>();
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        GenerateDungeon();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            NextFloor();
        }
    }

    void GenerateDungeon()
    {
        List<RectInt> rooms = GenerateRooms();

        ConnectRooms(rooms);

        CreateWalls();

        Floor currentFloor = floors[currentFloorIndex];
        InstantiateTiles(floor, floorPositions, currentFloor.floorColor);
        InstantiateTiles(wall, wallPositions, currentFloor.wallColor);

        PlacePlayerAndPrefabs(rooms);
        PlaceDecorations(rooms);

        PlayFloorMusic(currentFloor.backgroundMusic);
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

    void InstantiateTiles(GameObject prefab, HashSet<Vector2Int> positions, Color color)
    {
        foreach (Vector2Int pos in positions)
        {
            var tile = Instantiate(prefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
            instantiatedObjects.Add(tile);
            var spriteRenderer = tile.GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;

            if (prefab == floor)
            {
                spriteRenderer.sprite = floorSprites[Random.Range(0, floorSprites.Length)];
            }
            else if (prefab == wall)
            {
                spriteRenderer.sprite = wallSprites[Random.Range(0, wallSprites.Length)];
            }
        }
    }

    void PlacePlayerAndPrefabs(List<RectInt> rooms)
    {
        if (rooms.Count > 0)
        {
            Vector2Int startRoomCenter = GetRoomCenter(rooms[0]);
            player.transform.position = new Vector3(startRoomCenter.x, startRoomCenter.y, 0);

            Floor currentFloor = floors[currentFloorIndex];
            for (int i = 1; i < rooms.Count; i++)
            {
                Vector2Int roomCenter = GetRoomCenter(rooms[i]);
                int enemyCount = Random.Range(currentFloor.minEnemies, currentFloor.maxEnemies + 1);
                for (int j = 0; j < enemyCount; j++)
                {
                    Vector2Int spawnPos = GetRandomPointInRoom(rooms[i]);
                    GameObject mobPrefab = currentFloor.enemyPrefabs[Random.Range(0, currentFloor.enemyPrefabs.Count)];
                    var enemyInstance = Instantiate(mobPrefab, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
                    instantiatedObjects.Add(enemyInstance);
                }
            }

            RectInt furthestRoom = GetFurthestRoom(startRoomCenter, rooms);
            Vector2Int furthestRoomCenter = GetRoomCenter(furthestRoom);
            var stairsInstance = Instantiate(stairsDownPrefab, new Vector3(furthestRoomCenter.x, furthestRoomCenter.y, 0), Quaternion.identity);
            instantiatedObjects.Add(stairsInstance);
        }
    }

    void PlaceDecorations(List<RectInt> rooms)
    {
        Floor currentFloor = floors[currentFloorIndex];
        foreach (RectInt room in rooms)
        {
            int randomDecorationCount = Random.Range(currentFloor.minRandomDecorations, currentFloor.maxRandomDecorations + 1);
            for (int i = 0; i < randomDecorationCount; i++)
            {
                Vector2Int randomPos = GetRandomPointInRoom(room);
                if (!decorationPositions.Contains(randomPos))
                {
                    GameObject randomDecoration = currentFloor.randomDecorations[Random.Range(0, currentFloor.randomDecorations.Count)];
                    var decorationInstance = Instantiate(randomDecoration, new Vector3(randomPos.x, randomPos.y, 0), Quaternion.Euler(0, 0, Random.Range(0, 360)));
                    instantiatedObjects.Add(decorationInstance);
                    decorationPositions.Add(randomPos);
                }
            }

            int regularDecorationCount = Random.Range(currentFloor.minRegularDecorations, currentFloor.maxRegularDecorations + 1);
            for (int i = 0; i < regularDecorationCount; i++)
            {
                Vector2Int regularPos = GetRandomPointInRoom(room);
                if (!decorationPositions.Contains(regularPos))
                {
                    GameObject regularDecoration = currentFloor.regularDecorations[Random.Range(0, currentFloor.regularDecorations.Count)];
                    var decorationInstance = Instantiate(regularDecoration, new Vector3(regularPos.x, regularPos.y, 0), Quaternion.identity);
                    instantiatedObjects.Add(decorationInstance);
                    decorationPositions.Add(regularPos);
                }
            }
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

    void ClearPreviousFloor()
    {
        foreach (var obj in instantiatedObjects)
        {
            Destroy(obj);
        }
        instantiatedObjects.Clear();
        floorPositions.Clear();
        wallPositions.Clear();
        decorationPositions.Clear();
    }

    public void NextFloor()
    {
        currentFloorIndex++;
        if (currentFloorIndex < floors.Count)
        {
            ClearPreviousFloor();
            GenerateDungeon();
        }
        else
        {
            Debug.Log("No more floors available.");
        }
    }

    void PlayFloorMusic(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
