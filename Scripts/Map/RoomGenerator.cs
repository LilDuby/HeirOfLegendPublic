using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenerator : MonoBehaviour
{
    public GameObject startRoomPrefab;   // 시작 방 프리팹
    public GameObject bossRoomPrefab;    // 보스 방 프리팹
    public GameObject bossPrefab;
    public GameObject chestRoomPrefab;
    public GameObject upgradeRoomPrefab;
    public List<GameObject> roomPrefabs; // 랜덤으로 생성될 방 프리팹 목록
    public GameObject playerPrefab;
    public Transform playerStartPoint;
    public List<GameObject> monsterPrefabs;

    public int numberOfRooms = 8;  // 생성할 방의 개수
    public float gridSize = 1f;   // 방 사이의 간격
    public int maxAttempts = 10;   // 위치 재조정 시도 횟수

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
        // GameManager.instance.player가 생성될 때까지 기다립니다.
        while (GameManager.instance.player == null)
        {
            yield return null;
        }

        // 생성된 플레이어에서 Virtual Camera를 찾습니다.
        virtualCamera = GameManager.instance.player.GetComponentInChildren<CinemachineVirtualCamera>();

        if (virtualCamera != null)
        {
            // 방을 생성합니다.
            GenerateRooms();

            // 방 생성이 완료된 후 플레이어를 시작 지점으로 이동합니다.
            if (playerStartPoint != null)
            {
                testplayer();
                GameManager.instance.player.transform.position = playerStartPoint.position;
                UpdateCameraConfiner(spawnedRooms[0]);  // 첫 번째 방의 카메라 컨파이너를 설정
            }
            else
            {
                Debug.LogWarning("PlayerStartPoint가 설정되지 않았습니다.");
            }
        }
        else
        {
            Debug.LogError("CinemachineVirtualCamera를 플레이어에서 찾을 수 없습니다.");
        }
    }
    public void testplayer()
    {
        playerPrefab = GameManager.instance.player;
       // playerPrefab.transform.position = playerStartPoint.position;

    }

    void GenerateRooms()
    {
        // 방들의 순서를 랜덤하게 섞기
        List<GameObject> shuffledRooms = new List<GameObject>(roomPrefabs);
        ShuffleList(shuffledRooms);

        // 방향 설정 (오른쪽, 위쪽, 왼쪽, 아래쪽)
        Vector2[] directions = { Vector2.right, Vector2.up, Vector2.left, Vector2.down };

        // 시작 방 생성
        Vector2 currentPosition = Vector2.zero;

        GameObject startRoom = Instantiate(startRoomPrefab, currentPosition, Quaternion.identity);
        spawnedRooms.Add(startRoom);
        playerStartPoint = startRoom.transform.GetChild(0);
        // 중간 방 생성
        playerStartPoint = startRoom.transform.Find("PlayerStartPoint");
        if (playerStartPoint == null)
        {
            Debug.LogError("PlayerStartPoint를 시작 방에서 찾을 수 없습니다.");
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
                Vector2 newPosition = currentPosition + direction * (roomSize + new Vector2(gridSize, gridSize)); // 수정된 부분

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

        // 보스 방 생성
        bool bossPositionFound = false;
        int bossAttempts = 0;

        while (!bossPositionFound && bossAttempts < maxAttempts)
        {
            Vector2 bossDirection = directions[Random.Range(0, directions.Length)];
            Vector2 bossRoomSize = GetRoomSize(bossRoomPrefab);
            Vector2 bossRoomPosition = currentPosition + bossDirection * (bossRoomSize + new Vector2(gridSize, gridSize)); // 수정된 부분

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
        // Tilemap을 이용해 방의 크기를 계산하는 로직
        Tilemap tilemap = roomPrefab.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            // 타일맵의 경계를 가져옵니다.
            BoundsInt bounds = tilemap.cellBounds;
            Vector3Int size = bounds.size;
            return new Vector2(size.x * tilemap.cellSize.x, size.y * tilemap.cellSize.y);
        }

        // BoxCollider2D가 있는 경우
        BoxCollider2D boxCollider = roomPrefab.GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            return boxCollider.size;
        }

        // Renderer가 있는 경우
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
                // 랜덤하게 몬스터 프리팹 선택
                int randomIndex = Random.Range(0, monsterPrefabs.Count);
                GameObject selectedMonster = monsterPrefabs[randomIndex];

                // 몬스터를 약간의 오프셋을 주어 생성하여 같은 위치에 겹치지 않게 함
                Vector3 spawnPosition = room.transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
                Instantiate(selectedMonster, spawnPosition, Quaternion.identity);
            }
        }
        // 특정 확률로 상자방 생성
        if (Random.value < 0.3f) // 예: 10% 확률로 상자방 생성
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
                // 자식 오브젝트에서 cameraCollider 찾기
                Transform colliderTransform = room.transform.Find("cameraCollider");
                if (colliderTransform != null)
                {
                    PolygonCollider2D roomCollider = colliderTransform.GetComponent<PolygonCollider2D>();
                    if (roomCollider != null)
                    {
                        confiner.m_BoundingShape2D = roomCollider;
                        Debug.Log("카메라 경계가 업데이트되었습니다: " + room.name);
                    }
                    else
                    {
                        Debug.LogWarning("cameraCollider에 PolygonCollider2D가 없습니다: " + room.name);
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