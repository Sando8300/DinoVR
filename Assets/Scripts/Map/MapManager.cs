using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance; // 싱글톤

    [Header("맵 설정")]
    public float roomSize = 50f; // 방 크기 (반드시 50으로 설정!)

    [Header("확률 시스템")]
    public float currentEscapeChance = 0f; // 0%에서 시작
    public float chanceIncrease = 5f;      // 클리어 시 5%씩 증가

    [Header("프리팹 연결")]
    public GameObject exitRoomPrefab;      // 탈출 방 (포탈)
    public GameObject[] randomRoomPrefabs; // 공룡 방들 (Dimo, Troo, Pachy 등)

    // 생성된 방 목록 (좌표 : 방 오브젝트)
    private Dictionary<Vector2Int, GameObject> spawnedRooms = new Dictionary<Vector2Int, GameObject>();

    void Awake()
    {
        Instance = this;
        // 시작 방(0,0)은 이미 씬에 있다고 가정하고 장부에 등록
        // (만약 시작 방을 씬에 미리 배치했다면, 그 위치를 (0,0)으로 인식하게 설정 필요)
        // 가장 쉬운 방법: 시작 방 프리팹을 (0,0,0)에 두고 시작하기.
        spawnedRooms.Add(new Vector2Int(0, 0), gameObject);
    }

    // 문 센서가 호출: "내 위치(currentPos)에서 저쪽(direction)에 방 만들어줘"
    public void SpawnRoom(Vector3 sensorPos, Vector3 direction)
    {
        // 1. 센서 위치를 기준으로 현재 방의 그리드 좌표 계산
        Vector2Int currentGrid = WorldToGrid(sensorPos);

        // 2. 목표 좌표 계산 (북쪽이면 z+1, 동쪽이면 x+1)
        Vector2Int targetGrid = currentGrid + new Vector2Int((int)direction.x, (int)direction.z);

        // 3. 이미 방이 있다면 생성 취소
        if (spawnedRooms.ContainsKey(targetGrid)) return;

        // 4. 생성할 방 종류 결정 (룰렛)
        GameObject prefabToSpawn = GetRandomRoomPrefab();

        // 5. 실제 생성 (Instantiate)
        Vector3 spawnWorldPos = new Vector3(targetGrid.x * roomSize, 0, targetGrid.y * roomSize);
        GameObject newRoom = Instantiate(prefabToSpawn, spawnWorldPos, Quaternion.identity);

        // 생성된 방의 이름을 좌표로 보기 좋게 변경
        newRoom.name = $"Room_{targetGrid.x}_{targetGrid.y}";

        // 6. 장부 등록
        spawnedRooms.Add(targetGrid, newRoom);
    }

    GameObject GetRandomRoomPrefab()
    {
        // 0.0 ~ 100.0 주사위 굴리기
        float dice = Random.Range(0f, 100f);

        // 현재 확률 안에 들면 탈출 방 당첨!
        if (dice < currentEscapeChance)
        {
            Debug.Log($"🎉 탈출 방 당첨! (확률: {currentEscapeChance}%)");
            return exitRoomPrefab;
        }
        else
        {
            // 꽝 -> 공룡 방 랜덤 선택
            int rnd = Random.Range(0, randomRoomPrefabs.Length);
            return randomRoomPrefabs[rnd];
        }
    }

    // 월드 좌표 -> 그리드 좌표 변환기 (반올림 사용)
    Vector2Int WorldToGrid(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x / roomSize), Mathf.RoundToInt(pos.z / roomSize));
    }

    // 방 클리어 시 확률 증가 (BattleRoomManager가 호출)
    public void IncreaseChance()
    {
        currentEscapeChance += chanceIncrease;
        // 확률이 100을 넘지 않게 캡 씌우기 (선택 사항)
        if (currentEscapeChance > 100f) currentEscapeChance = 100f;

        Debug.Log($"📈 확률 증가! 현재 탈출 확률: {currentEscapeChance}%");
    }
}