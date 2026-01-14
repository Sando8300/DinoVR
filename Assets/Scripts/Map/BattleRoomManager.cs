using UnityEngine;
using System.Collections.Generic;

public class BattleRoomManager : MonoBehaviour
{
    [Header("연결: 문 & 투명벽")]
    public SimpleLockDoor[] myDoors;
    public GameObject[] invisibleWalls;

    [Header("연결: 사다리 (전투 끝나면 나타남)")]
    // 배열([])로 변경! 여기에 동서남북 사다리 4개를 다 넣으세요.
    public GameObject[] ladders;

    [Header("몬스터 설정 (럭키맵은 비워두세요)")]
    public GameObject monsterPrefab;
    public Transform[] spawnPoints;

    private bool isCleared = false;
    private bool battleStarted = false;
    private List<GameObject> activeMonsters = new List<GameObject>();

    void Start()
    {
        // 시작 시 초기화
        foreach (var door in myDoors) if (door) door.Open();
        foreach (var wall in invisibleWalls) if (wall) wall.SetActive(false);

        // 시작할 때 모든 사다리는 일단 켜둠
        if (ladders != null)
        {
            foreach (var ladder in ladders)
            {
                if (ladder != null) ladder.SetActive(true);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCleared || battleStarted) return;
        if (other.CompareTag("Player")) StartBattle();
    }

    void StartBattle()
    {
        battleStarted = true;

        // 1. 럭키맵(몬스터 없음) 처리
        if (monsterPrefab == null || spawnPoints.Length == 0)
        {
            Debug.Log("럭키맵! 프리패스.");
            EndBattle(true);
            return;
        }

        // 2. 전투 시작: 문 닫고, 벽 치고, 사다리 숨기기
        foreach (var door in myDoors) if (door) door.Close();
        foreach (var wall in invisibleWalls) if (wall) wall.SetActive(true);

        // 전투 중엔 도망 못 가게 모든 사다리 끄기!
        if (ladders != null)
        {
            foreach (var ladder in ladders)
            {
                if (ladder != null) ladder.SetActive(false);
            }
        }

        // 몬스터 소환
        foreach (var sp in spawnPoints)
        {
            if (sp != null)
            {
                GameObject mon = Instantiate(monsterPrefab, sp.position, sp.rotation);
                mon.SetActive(true); // 혹시 꺼져있을까봐 켜줌
                activeMonsters.Add(mon);
            }
        }
    }

    void Update()
    {
        if (battleStarted && !isCleared)
        {
            activeMonsters.RemoveAll(x => x == null); // 죽어서 사라진 놈 명단에서 제거
            if (activeMonsters.Count == 0) EndBattle(true); // 다 죽었으면 승리
        }
    }

    void EndBattle(bool isVictory)
    {
        isCleared = true;

        // 3. 전투 종료: 문 열고, 벽 치우고, 사다리 나타나기
        foreach (var door in myDoors) if (door) door.Open();
        foreach (var wall in invisibleWalls) if (wall) wall.SetActive(false);

        // 이겼으니 모든 사다리 활성화!
        if (ladders != null)
        {
            foreach (var ladder in ladders)
            {
                if (ladder != null) ladder.SetActive(true);
            }
        }

        if (isVictory && MapManager.Instance != null)
        {
            MapManager.Instance.IncreaseChance();
        }
    }
}