using UnityEngine;

public class TestMonster : MonoBehaviour
{
    [Header("생존 시간 (초)")]
    public float lifeTime = 5f; // 기본 5초

    void Start()
    {
        // 태어나자마자 시한부 선고
        // lifeTime 초 뒤에 Destroy(파괴) 함수가 실행됨 -> BattleRoomManager가 감지함
        Destroy(gameObject, lifeTime);
    }
}