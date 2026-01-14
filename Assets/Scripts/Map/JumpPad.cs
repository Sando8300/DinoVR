using System.Collections;
using Unity.AI.Navigation;
using Unity.Services.Analytics;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    
    void OnTriggerEnter(Collider other)
    {
        if (isJumped) return;

        // 플레이어 확인 (Tag: Player)
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {

            player = other.GetComponent<Transform>();
         
                Debug.Log($"점프시작");
                StartCoroutine(General());
           
        }
    }

  /*  IEnumerator DoParabolicJump(CharacterController cc)
    {
        isBouncing = true;
        float elapsed = 0f;

        // 1. 시작점과 도착점 계산
        Vector3 startPos = cc.transform.position;
        Vector3 endPos = landingPoint.position;

        while (elapsed < duration)
        {
            // 0 ~ 1 사이의 진행률 (normalizedTime)
            float t = elapsed / duration;

            // 2. 직선 이동 (Lerp): 시작점 -> 도착점 사이의 현재 위치
            Vector3 linearPos = Vector3.Lerp(startPos, endPos, t);

            // 3. 높이 계산 (포물선 공식)
            // 보내주신 코드: t - t*t (최대 0.25) -> 여기에 4를 곱해야 설정한 height만큼 정확히 올라갑니다.
            float yOffset = 4 * height * (t - t * t);

            // 4. 최종 목표 위치
            Vector3 targetPos = linearPos + Vector3.up * yOffset;

            // 5. 이동 실행 (CharacterController.Move 사용)
            // 현재 위치와 목표 위치의 차이(Delta)만큼 이동
            Vector3 moveDir = targetPos - cc.transform.position;
            cc.Move(moveDir);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 6. 정확한 착지 보정
        cc.Move(endPos - cc.transform.position);

        // 쿨타임
        yield return new WaitForSeconds(0.5f);
        isBouncing = false;
    }

    // 에디터에서 도착 지점까지 선을 그려줌 (디버그용)
    private void OnDrawGizmos()
    {
        if (landingPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, landingPoint.position);
            Gizmos.DrawWireSphere(landingPoint.position, 0.3f);
        }
    }
*/

    public float jumpDelay = 3;
    public bool isJumped = false;
     float nomarlizeTime;
    public Transform player;
    public Vector3 endpos;
    public float height = 1;
    public float duration = 2;
    public float fwd;
    public float upd;

    private void Awake()
    {
        //imsi = new Vector3(0, 10, 1);
        nomarlizeTime = 0;
    }
    IEnumerator General()
    {
        nomarlizeTime = 0;
        endpos = transform.position + player.forward * fwd + player.up * upd;
        isJumped = true;
        while (jumpDelay < 0.8f)
        {
            jumpDelay += Time.deltaTime;
            yield return null;
        }


        
        while (nomarlizeTime < 1f)
        {

            float yOffset = height * (nomarlizeTime - nomarlizeTime * nomarlizeTime);
            player.position = Vector3.Lerp(transform.position, endpos, nomarlizeTime) + height * Vector3.up * yOffset;
            nomarlizeTime = nomarlizeTime + Time.deltaTime / duration;
            yield return null;

        }


        while (jumpDelay < 1.5f)
        {
            jumpDelay += Time.deltaTime;
            yield return null;
        }


        isJumped = false;



    }

}