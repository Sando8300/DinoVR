using UnityEngine;

public class LifeTimer : MonoBehaviour
{
    [Header("생존 시간 설정 (초)")]
    public float lifeTime = 10f; // 인스펙터 창에서 공룡마다 다르게 입력하세요!

    void Start()
    {
        // 설정된 시간(lifeTime)이 지나면 Die 함수 실행
        Invoke("Die", lifeTime);
    }

    void Die()
    {
        Debug.Log($" {gameObject.name} 수명 종료 -> 방 클리어 처리됨.");
        // 공룡이 사라지면 BattleRoomManager가 감지하고 문을 엽니다.
        Destroy(gameObject);
    }
}