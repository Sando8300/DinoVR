using UnityEngine;

public class DoorSensor : MonoBehaviour
{
    [Tooltip("이 문이 향하는 방향 (예: 북쪽이면 0, 0, 1)")]
    public Vector3 direction;

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return; // 이미 작동했으면 무시

        if (other.CompareTag("Player"))
        {
            // 매니저에게 방 생성 요청
            // transform.position은 센서의 위치이므로, 현재 방 영역 안에 있어야 함
            MapManager.Instance.SpawnRoom(transform.position, direction);
            triggered = true;
        }
    }
}