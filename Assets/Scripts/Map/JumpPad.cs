using UnityEngine;
using System.Collections;

public class JumpPad : MonoBehaviour
{
    [Header("점프 설정")]
    public float jumpHeight = 3.0f;  // 얼마나 높이 뛸지
    public float duration = 0.5f;    // 체공 시간 (짧을수록 빠르게 튀어 오름)
    public bool forwardJump = false; // 체크하면 '보는 방향'으로도 날아감

    private bool isActive = false;   // 중복 작동 방지

    void OnTriggerEnter(Collider other)
    {
        if (isActive) return;

        // 태그가 Player이거나, 부모 중에 Player 태그가 있는지 확인
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            // XR Origin에는 보통 CharacterController가 있습니다. 그걸 찾습니다.
            CharacterController cc = other.transform.root.GetComponentInChildren<CharacterController>();

            if (cc != null)
            {
                Debug.Log("점프 가동! (Rigidbody 없음)");
                StartCoroutine(DoJump(cc, other.transform.root));
            }
            else
            {
                Debug.LogWarning("플레이어에게 CharacterController가 없습니다!");
            }
        }
    }

    IEnumerator DoJump(CharacterController cc, Transform playerRoot)
    {
        isActive = true;
        float elapsed = 0f;

        // 점프 방향 결정 (기본: 위쪽 / 옵션: 위 + 플레이어가 보는 앞쪽)
        Vector3 jumpDir = Vector3.up;
        if (forwardJump)
        {
            jumpDir += playerRoot.forward * 0.5f; // 앞으로도 살짝 밀어줌
            jumpDir.Normalize();
        }

        while (elapsed < duration)
        {
            // 시간 흐름에 따라 힘이 줄어드는 곡선 만들기 (자연스러운 점프)
            // 1.0에서 시작해서 0.0으로 줄어듦
            float strength = Mathf.Lerp(jumpHeight, 0, elapsed / duration);

            // 실제 이동 명령 (물리 엔진 무시하고 강제 이동)
            cc.Move(jumpDir * strength * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 쿨타임 살짝 줌
        yield return new WaitForSeconds(0.5f);
        isActive = false;
    }
}