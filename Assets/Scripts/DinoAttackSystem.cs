using UnityEngine;

public class DinoAttackSystem : MonoBehaviour
{
    [Header("공격 설정 (인스펙터에서 수정 가능)")]
    public int attackDamage = 10;      // 공룡마다 다르게 설정 가능
    public float attackRange = 1.0f;   // 공격 사거리
    public float hitTiming = 0.55f;    // 공격 판정 타이밍 (0.0 ~ 1.0, 55%는 0.55)

    [Header("디버그")]
    public bool showGizmos = true;     // 공격 범위 눈으로 보기

    private Animator anim;
    private bool hasDealtDamage = false; // 한 번의 공격에 데미지가 중복으로 들어가는 것 방지

    // 플레이어 태그 확인용 (플레이어 오브젝트 태그를 "Player"로 설정해주세요)
    private string targetTag = "Player";

    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        CheckAttackAnimation();
    }

    void CheckAttackAnimation()
    {
        // 현재 0번 레이어의 애니메이션 상태 가져오기
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 1. 현재 재생 중인 애니메이션이 'Attack' 태그를 가지고 있거나 이름이 Attack이 포함되어 있는지 확인
        // (Jurassic Pack은 보통 Attack 상태 이름을 씁니다. 만약 안 되면 Animator 창에서 해당 State의 Tag를 "Attack"으로 지정해주세요)
        bool isAttacking = stateInfo.IsTag("Attack") || stateInfo.IsName("Attack");

        if (isAttacking)
        {
            // 2. 애니메이션 진행도가 설정한 타이밍(55%)을 지났고, 아직 데미지를 주지 않았다면
            // (stateInfo.normalizedTime은 0~1 사이로 진행도를 나타냄, 루프되면 1을 넘음)
            float progress = stateInfo.normalizedTime % 1.0f;

            if (progress >= hitTiming && !hasDealtDamage)
            {
                PerformAttack();
                hasDealtDamage = true; // 이번 공격 모션에서는 더 이상 데미지 안 줌
            }
        }
        else
        {
            // 공격 상태가 아니면 플래그 초기화 (다음 공격 준비)
            hasDealtDamage = false;
        }
    }

    void PerformAttack()
    {
        // 공룡의 입 위치나 전방을 기준으로 공격 판정
        // 여기서는 공룡의 앞쪽(transform.forward)으로 구체를 쏘아서 검사 (OverlapSphere)
        Vector3 attackCenter = transform.position + transform.forward * (attackRange * 0.5f) + Vector3.up;

        // 지정된 위치에 있는 모든 콜라이더 검사
        Collider[] hitColliders = Physics.OverlapSphere(attackCenter, attackRange / 1.5f);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(targetTag))
            {
                // 플레이어 스크립트 찾아서 데미지 주기
                PlayerHealth player = hitCollider.GetComponent<PlayerHealth>();
                // 혹은 GameManager를 통해 접근
                // PlayerHealth player = GameManager.Instance.GetComponent<PlayerHealth>(); 

                if (player != null)
                {
                    player.TakeDamage(attackDamage);
                    Debug.Log($"{gameObject.name}가 플레이어를 공격했습니다!");
                }

                // 한 번에 한 명만 때리려면 여기서 break;
            }
        }
    }

    // 에디터에서 공격 범위를 눈으로 확인하기 위한 함수
    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.red;
            Vector3 attackCenter = transform.position + transform.forward * (attackRange * 0.5f) + Vector3.up;
            Gizmos.DrawWireSphere(attackCenter, attackRange / 1.5f);
        }
    }
}