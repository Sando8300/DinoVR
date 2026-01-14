using UnityEngine;
using System.Collections.Generic;

public class BattleRoomManager : MonoBehaviour
{
    [Header("연결: 문 & 투명벽")]
    public SimpleLockDoor[] myDoors;
    public GameObject[] invisibleWalls;

    [Header("연결: 점프대 (전투 끝나면 나타남)")]
    // 이름 변경: ladders -> jumpPads
    // 여기에 설치한 점프대 오브젝트들을 모두 넣으세요.
    public GameObject[] jumpPads;

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

        // ★ 시작할 때 점프대는 일단 켜둠 (전투가 없는 방일 수도 있으니)
        if (jumpPads != null)
        {
            foreach (var pad in jumpPads)
            {
                if (pad != null) pad.SetActive(true);
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

        // 2. 전투 시작: 문 닫고, 벽 치고, 점프대 숨기기
        foreach (var door in myDoors) if (door) door.Close();
        foreach (var wall in invisibleWalls) if (wall) wall.SetActive(true);

        // ★ 전투 중엔 도망 못 가게 점프대 끄기!
        if (jumpPads != null)
        {
            foreach (var pad in jumpPads)
            {
                if (pad != null) pad.SetActive(false);
            }
        }

        // 몬스터 소환
        foreach (var sp in spawnPoints)
        {
            if (sp != null)
            {
                Debug.Log($"{sp.position}");
                GameObject mon = Instantiate(monsterPrefab, sp.position, sp.rotation);
                mon.SetActive(true); // 혹시 꺼져있을까봐 켜줌
                mon.GetComponent<CustomDinoController>();
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

        // 3. 전투 종료: 문 열고, 벽 치우고, 점프대 나타나기
        foreach (var door in myDoors) if (door) door.Open();
        foreach (var wall in invisibleWalls) if (wall) wall.SetActive(false);

        // 이겼으니 점프대 활성화!
        if (jumpPads != null)
        {
            foreach (var pad in jumpPads)
            {
                if (pad != null) pad.SetActive(true);
            }
        }

        if (isVictory && MapManager.Instance != null)
        {
            MapManager.Instance.IncreaseChance();
        }
    }
}