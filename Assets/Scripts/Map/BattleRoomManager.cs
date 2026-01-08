using UnityEngine;
using System.Collections.Generic;

public class BattleRoomManager : MonoBehaviour
{
    [Header("연결")]
    public SimpleLockDoor[] myDoors;
    public GameObject[] invisibleWalls;

    [Header("몬스터 (럭키맵은 비워두세요)")]
    public GameObject monsterPrefab;
    public Transform[] spawnPoints;

    private bool isCleared = false;
    private bool battleStarted = false;
    private List<GameObject> activeMonsters = new List<GameObject>();

    void Start()
    {
        foreach (var door in myDoors) if (door) door.Open();
        foreach (var wall in invisibleWalls) if (wall) wall.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCleared || battleStarted) return;
        if (other.CompareTag("Player")) StartBattle();
    }

    void StartBattle()
    {
        battleStarted = true;

        // ★ 럭키맵 처리 로직 ★
        // 몬스터가 없으면 -> "즉시 승리(true)" 처리 -> 확률 증가!
        if (monsterPrefab == null || spawnPoints.Length == 0)
        {
            Debug.Log("🍀 럭키맵! 몬스터 없이 바로 클리어 처리.");
            EndBattle(true);
            return;
        }

        // 공룡맵: 문 닫고 전투 시작
        foreach (var door in myDoors) if (door) door.Close();
        foreach (var wall in invisibleWalls) if (wall) wall.SetActive(true);

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
        if (battleStarted && !isCleared)
        {
            activeMonsters.RemoveAll(x => x == null);
            if (activeMonsters.Count == 0) EndBattle(true);
        }
    }

    void EndBattle(bool isVictory)
    {
        isCleared = true;

        // 문 열기
        foreach (var door in myDoors) if (door) door.Open();
        foreach (var wall in invisibleWalls) if (wall) wall.SetActive(false);

        // ★ 승리 시(럭키맵 포함) 매니저에게 보고하여 확률 증가
        if (isVictory && MapManager.Instance != null)
        {
            MapManager.Instance.IncreaseChance();
        }
    }
}