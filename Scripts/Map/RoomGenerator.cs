using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenerator : MonoBehaviour
{
    public GameObject startRoomPrefab;   // ���� �� ������
    public GameObject bossRoomPrefab;    // ���� �� ������
    public GameObject bossPrefab;
    public GameObject chestRoomPrefab;
    public GameObject upgradeRoomPrefab;
    public List<GameObject> roomPrefabs; // �������� ������ �� ������ ���
    public GameObject playerPrefab;
    public Transform playerStartPoint;
    public List<GameObject> monsterPrefabs;

    public int numberOfRooms = 8;  // ������ ���� ����
    public float gridSize = 1f;   // �� ������ ����
    public int maxAttempts = 10;   // ��ġ ������ �õ� Ƚ��

    private List<GameObject> spawnedRooms = new List<GameObject>();
    private CinemachineVirtualCamera virtualCamera;
    void Start()
    {
        StartCoroutine(SetupCameraAndGenerateRooms());
    }

    private void Awake()
    {
        GameManager.instance.roomGenerator = this;
    }
    IEnumerator SetupCameraAndGenerateRooms()
    {
        // GameManager.instance.player�� ������ ������ ��ٸ��ϴ�.
        while (GameManager.instance.player == null)
        {
            yield return null;
        }

        // ������ �÷��̾�� Virtual Camera�� ã���ϴ�.
        virtualCamera = GameManager.instance.player.GetComponentInChildren<CinemachineVirtualCamera>();

        if (virtualCamera != null)
        {
            // ���� �����մϴ�.
            GenerateRooms();

            // �� ������ �Ϸ�� �� �÷��̾ ���� �������� �̵��մϴ�.
            if (playerStartPoint != null)
            {
                testplayer();
                GameManager.instance.player.transform.position = playerStartPoint.position;
                UpdateCameraConfiner(spawnedRooms[0]);  // ù ��° ���� ī�޶� �����̳ʸ� ����
            }
            else
            {
                Debug.LogWarning("PlayerStartPoint�� �������� �ʾҽ��ϴ�.");
            }
        }
        else
        {
            Debug.LogError("CinemachineVirtualCamera�� �÷��̾�� ã�� �� �����ϴ�.");
        }
    }
    public void testplayer()
    {
        playerPrefab = GameManager.instance.player;
       // playerPrefab.transform.position = playerStartPoint.position;

    }

    void GenerateRooms()
    {
        // ����� ������ �����ϰ� ����
        List<GameObject> shuffledRooms = new List<GameObject>(roomPrefabs);
        ShuffleList(shuffledRooms);

        // ���� ���� (������, ����, ����, �Ʒ���)
        Vector2[] directions = { Vector2.right, Vector2.up, Vector2.left, Vector2.down };

        // ���� �� ����
        Vector2 currentPosition = Vector2.zero;

        GameObject startRoom = Instantiate(startRoomPrefab, currentPosition, Quaternion.identity);
        spawnedRooms.Add(startRoom);
        playerStartPoint = startRoom.transform.GetChild(0);
        // �߰� �� ����
        playerStartPoint = startRoom.transform.Find("PlayerStartPoint");
        if (playerStartPoint == null)
        {
            Debug.LogError("PlayerStartPoint�� ���� �濡�� ã�� �� �����ϴ�.");
            return;
        }

        UpdateCameraConfiner(startRoom);

        for (int i = 0; i < numberOfRooms; i++)
        {
            bool positionFound = false;
            int attempts = 0;

            while (!positionFound && attempts < maxAttempts)
            {
                Vector2 direction = directions[Random.Range(0, directions.Length)];
                Vector2 roomSize = GetRoomSize(shuffledRooms[i % shuffledRooms.Count]);
                Vector2 newPosition = currentPosition + direction * (roomSize + new Vector2(gridSize, gridSize)); // ������ �κ�

                if (!IsPositionOccupied(newPosition, shuffledRooms[i % shuffledRooms.Count]))
                {
                    GameObject spawnedRoom = Instantiate(shuffledRooms[i % shuffledRooms.Count], newPosition, Quaternion.identity);
                    spawnedRooms.Add(spawnedRoom);
                    currentPosition = newPosition;
                    positionFound = true;

                    SpawnEntitiesInRoom(spawnedRoom);

                    UpdateCameraConfiner(spawnedRoom);
                }

                attempts++;
            }

            if (i > 0)
            {
                ConnectTeleportPoints();
            }
        }

        // ���� �� ����
        bool bossPositionFound = false;
        int bossAttempts = 0;

        while (!bossPositionFound && bossAttempts < maxAttempts)
        {
            Vector2 bossDirection = directions[Random.Range(0, directions.Length)];
            Vector2 bossRoomSize = GetRoomSize(bossRoomPrefab);
            Vector2 bossRoomPosition = currentPosition + bossDirection * (bossRoomSize + new Vector2(gridSize, gridSize)); // ������ �κ�

            if (!IsPositionOccupied(bossRoomPosition, bossRoomPrefab))
            {
                GameObject bossRoom = Instantiate(bossRoomPrefab, bossRoomPosition, Quaternion.identity);
                spawnedRooms.Add(bossRoom);
                bossPositionFound = true;

                Instantiate(bossPrefab, bossRoom.transform.position, Quaternion.identity);
                ConnectTeleportPoints();
                UpdateCameraConfiner(bossRoom);
            }

            bossAttempts++;
        }

    }
    Vector2 GetRoomSize(GameObject roomPrefab)
    {
        // Tilemap�� �̿��� ���� ũ�⸦ ����ϴ� ����
        Tilemap tilemap = roomPrefab.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            // Ÿ�ϸ��� ��踦 �����ɴϴ�.
            BoundsInt bounds = tilemap.cellBounds;
            Vector3Int size = bounds.size;
            return new Vector2(size.x * tilemap.cellSize.x, size.y * tilemap.cellSize.y);
        }

        // BoxCollider2D�� �ִ� ���
        BoxCollider2D boxCollider = roomPrefab.GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            return boxCollider.size;
        }

        // Renderer�� �ִ� ���
        Renderer renderer = roomPrefab.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size;
        }

        return new Vector2(20, 20);
    }
    bool IsPositionOccupied(Vector2 position, GameObject roomPrefab)
    {
        Vector2 roomSize = GetRoomSize(roomPrefab);
        foreach (var room in spawnedRooms)
        {
            Vector2 existingRoomPosition = room.transform.position;
            Vector2 existingRoomSize = GetRoomSize(room);

            if (position.x < existingRoomPosition.x + existingRoomSize.x / 2 &&
                position.x + roomSize.x / 2 > existingRoomPosition.x &&
                position.y < existingRoomPosition.y + existingRoomSize.y / 2 &&
                position.y + roomSize.y / 2 > existingRoomPosition.y)
            {
                return true;
            }
        }
        return false;
    }

    
    void ConnectTeleportPoints()
    {
        for (int i = 0; i < spawnedRooms.Count - 1; i++)
        {
            GameObject currentRoopm = spawnedRooms[i];
            GameObject nextRoom = spawnedRooms[i + 1];

            Transform fromExit = currentRoopm.transform.Find("TeleportPoint_Exit");
            Transform toEntrance = nextRoom.transform.Find("TeleportPoint_Entrance");
            if (fromExit != null && toEntrance != null)
            {
                var fromTeleport = fromExit.GetComponent<Teleport>();
                if (fromTeleport != null)
                {
                    fromTeleport.destination = toEntrance;
                }
            }
        }
    }
    void SpawnEntitiesInRoom(GameObject room)
    {
        int numMonsters = Random.Range(1, 10);

        if (Random.value < 0.7f)
        {
            for (int i = 0; i < numMonsters; i++)
            {
                // �����ϰ� ���� ������ ����
                int randomIndex = Random.Range(0, monsterPrefabs.Count);
                GameObject selectedMonster = monsterPrefabs[randomIndex];

                // ���͸� �ణ�� �������� �־� �����Ͽ� ���� ��ġ�� ��ġ�� �ʰ� ��
                Vector3 spawnPosition = room.transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
                Instantiate(selectedMonster, spawnPosition, Quaternion.identity);
            }
        }
        // Ư�� Ȯ���� ���ڹ� ����
        if (Random.value < 0.3f) // ��: 10% Ȯ���� ���ڹ� ����
        {
            Instantiate(chestRoomPrefab, room.transform.position, Quaternion.identity);
        }
    }
    void UpdateCameraConfiner(GameObject room)
    {
        if (virtualCamera != null)
        {
            CinemachineConfiner2D confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
            if (confiner != null)
            {
                // �ڽ� ������Ʈ���� cameraCollider ã��
                Transform colliderTransform = room.transform.Find("cameraCollider");
                if (colliderTransform != null)
                {
                    PolygonCollider2D roomCollider = colliderTransform.GetComponent<PolygonCollider2D>();
                    if (roomCollider != null)
                    {
                        confiner.m_BoundingShape2D = roomCollider;
                        Debug.Log("ī�޶� ��谡 ������Ʈ�Ǿ����ϴ�: " + room.name);
                    }
                    else
                    {
                        Debug.LogWarning("cameraCollider�� PolygonCollider2D�� �����ϴ�: " + room.name);
                    }
                }
            }
        }
    }
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}