using UnityEngine;
using System.Collections.Generic;

public class BattleRoomManager : MonoBehaviour
{
    [Header("연결: 내 방의 문 4개")]
    public SimpleLockDoor[] myDoors;

    [Header("설정: 몬스터와 위치")]
    public GameObject monsterPrefab; // 소환할 몬스터 (없으면 안전한 방)
    public Transform[] spawnPoints;  // 소환 위치들

    // 내부 상태 변수
    private bool isCleared = false;
    private bool battleStarted = false;
    private List<GameObject> activeMonsters = new List<GameObject>();

    void Start()
    {
        // 1. 게임 시작 시 문은 모두 열어둠 (함정 세팅)
        foreach (var door in myDoors)
        {
            if (door != null) door.Open();
        }
    }

    // 플레이어가 사다리 타고 올라와서 바닥(트리거)을 밟았을 때
    void OnTriggerEnter(Collider other)
    {
        if (isCleared || battleStarted) return;

        if (other.CompareTag("Player"))
        {
            StartBattle();
        }
    }

    void StartBattle()
    {
        battleStarted = true;

        // 몬스터가 없거나 스폰 포인트가 없는 방(안전한 방/시작 방)인 경우
        if (monsterPrefab == null || spawnPoints.Length == 0)
        {
            // 전투 없이 바로 클리어 처리 (문 안 잠그고 놔둠)
            // 확률을 올릴지 말지는 선택 사항 (여기서는 안전한 방은 확률 안 올림)
            isCleared = true;
            return;
        }

        // 1. 함정 발동! 문 닫기
        foreach (var door in myDoors)
        {
            if (door != null) door.Close();
        }

        // 2. 몬스터 소환
        foreach (var sp in spawnPoints)
        {
            if (sp != null)
            {
                GameObject mon = Instantiate(monsterPrefab, sp.position, sp.rotation);
                activeMonsters.Add(mon);
            }
        }
    }

    void Update()
    {
        // 전투 중일 때 몬스터 상태 체크
        if (battleStarted && !isCleared)
        {
            // 죽어서 사라진(null) 몬스터는 리스트에서 제거
            activeMonsters.RemoveAll(x => x == null);

            // 몬스터가 전멸했다면?
            if (activeMonsters.Count == 0)
            {
                EndBattle();
            }
        }
    }

    void EndBattle()
    {
        isCleared = true;

        // 1. 보상: 문 다시 열기
        foreach (var door in myDoors)
        {
            if (door != null) door.Open();
        }

        // 2. 매니저에게 보고해서 탈출 확률 증가 (+5%)
        if (MapManager.Instance != null)
        {
            MapManager.Instance.IncreaseChance();
        }
    }
}