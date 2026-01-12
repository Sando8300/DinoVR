using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [Header("설정")]
    public float roomSize = 50f;
    public GameObject firstStartRoom; // 씬에 있는 00.StartMap 연결

    [Header("확률 설정 (초기값)")]
    public float startDinoChance = 30f;  // 공룡방 각각의 확률
    public float startLuckyChance = 10f; // 럭키맵 확률
    public float chanceIncrease = 5f;    // 클리어 시 증가할 탈출 확률

    [Header("확률 감소량 (탈출 확률이 5% 늘 때마다)")]
    public float decayDino = 1.5f;       // 공룡방 감소량
    public float decayLucky = 0.5f;      // 럭키맵 감소량

    [Header("현재 상태 (자동 계산)")]
    public float currentEscapeChance = 0f;
    public int totalClears = 0; // 방을 몇 개 깼는지

    [Header("프리팹 연결")]
    public GameObject exitRoomPrefab;      // 탈출 방
    public GameObject luckyRoomPrefab;     // 럭키 맵 (04.LuckyMap)
    public GameObject[] dinoRoomPrefabs;   // 공룡 방들 (01~03)

    private Dictionary<Vector2Int, GameObject> spawnedRooms = new Dictionary<Vector2Int, GameObject>();

    void Awake()
    {
        Instance = this;
        if (firstStartRoom != null) spawnedRooms.Add(new Vector2Int(0, 0), firstStartRoom);
        else spawnedRooms.Add(new Vector2Int(0, 0), gameObject);
    }

    public void SpawnRoom(Vector3 sensorPos, Vector3 direction)
    {
        Vector2Int currentGrid = WorldToGrid(sensorPos);
        Vector2Int targetGrid = currentGrid + new Vector2Int((int)direction.x, (int)direction.z);

        if (spawnedRooms.ContainsKey(targetGrid)) return;

        GameObject prefabToSpawn = GetRandomRoomPrefab();

        Vector3 spawnWorldPos = new Vector3(targetGrid.x * roomSize, 0, targetGrid.y * roomSize);
        GameObject newRoom = Instantiate(prefabToSpawn, spawnWorldPos, Quaternion.identity);
        newRoom.name = $"Room_{targetGrid.x}_{targetGrid.y}";

        spawnedRooms.Add(targetGrid, newRoom);
    }

    // 핵심 확률 로직
    GameObject GetRandomRoomPrefab()
    {
        // 1. 현재 클리어 횟수에 따른 각 방의 확률 계산
        // Mathf.Max(0, ...)을 써서 확률이 음수가 되지 않게 방지
        float currentDino = Mathf.Max(0, startDinoChance - (totalClears * decayDino));
        float currentLucky = Mathf.Max(0, startLuckyChance - (totalClears * decayLucky));

        // 2. 주사위 굴리기 (0 ~ 100)
        float dice = Random.Range(0f, 100f);
        float cumulative = 0f;

        // [우선순위 1] 탈출 방 (현재 확률만큼)
        // 예: 5%면 0~5 사이
        cumulative += currentEscapeChance;
        if (dice < cumulative) return exitRoomPrefab;

        // [우선순위 2] 럭키 맵
        cumulative += currentLucky;
        if (dice < cumulative) return luckyRoomPrefab;

        // [우선순위 3] 공룡 방 3개 (남은 확률을 나눠 가짐)
        // 그냥 랜덤하게 아무거나 뽑아도 되지만, 정확한 확률 분포를 위해 룰렛 방식 사용
        for (int i = 0; i < dinoRoomPrefabs.Length; i++)
        {
            cumulative += currentDino;
            if (dice < cumulative) return dinoRoomPrefabs[i];
        }

        // 혹시 계산 오차로 남는 게 있다면 기본적으로 공룡방 리턴
        return dinoRoomPrefabs[0];
    }

    Vector2Int WorldToGrid(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x / roomSize), Mathf.RoundToInt(pos.z / roomSize));
    }

    // BattleRoomManager가 호출
    public void IncreaseChance()
    {
        totalClears++; // 클리어 횟수 증가
        currentEscapeChance += chanceIncrease; // 확률 5% 증가

        if (currentEscapeChance > 100f) currentEscapeChance = 100f;

        Debug.Log($"[클리어 {totalClears}회] 탈출확률: {currentEscapeChance}% (공룡: {Mathf.Max(0, startDinoChance - totalClears * decayDino)}% / 럭키: {Mathf.Max(0, startLuckyChance - totalClears * decayLucky)}%)");
    }
}