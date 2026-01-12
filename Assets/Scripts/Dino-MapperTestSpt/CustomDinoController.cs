using UnityEngine;

public class CustomDinoController : MonoBehaviour
{
    [Header("공룡 스펙 설정")]
    public float moveSpeed = 6.0f;       // 이동 속도 (Dimo는 빠르게, Pachy는 느리게 조절)
    public float rotSpeed = 2.0f;        // 회전 속도
    public float attackRange = 2.5f;     // 공격 사거리
    public int maxHealth = 100;          // 체력

    // 내부 변수
    private Animator anim;
    private Transform player;
    private bool isDead = false;
    private int currentHealth;

    //  쥬라기 팩 애니메이션 파라미터 이름 (수정 금지) 
    private readonly string ANI_MOVE = "Move";     // 0:정지, 2:달리기
    private readonly string ANI_ATTACK = "Attack"; // 공격 신호
    private readonly string ANI_ONGROUND = "OnGround"; // 땅에 붙어있음 (Dimo용)

    void Start()
    {
        // 1. 애니메이터 연결
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        // 2. 기존 Rigidbody 마취 (물리 충돌로 날아가는 것 방지)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // 3. 플레이어 찾기
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

        // 4. 초기화
        currentHealth = maxHealth;

        // Dimo(익룡)가 날지 않고 뛰어오도록 강제 설정
        anim.SetBool(ANI_ONGROUND, true);
    }

    void Update()
    {
        if (isDead || player == null) return;

        // 플레이어와의 거리 계산
        float distance = Vector3.Distance(transform.position, player.position);

        // 공격 범위 안에 들어왔는가?
        if (distance <= attackRange)
        {
            DoAttack();
        }
        else
        {
            DoChase();
        }
    }

    // 추격 모드
    void DoChase()
    {
        anim.SetBool(ANI_ATTACK, false); // 공격 끄기
        anim.SetInteger(ANI_MOVE, 2);    // 달리기(Run) 켜기

        // 플레이어 쪽 바라보기
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotSpeed);
        }

        // 앞으로 전진
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    // 공격 모드
    void DoAttack()
    {
        anim.SetInteger(ANI_MOVE, 0);   // 제자리 멈춤
        anim.SetBool(ANI_ATTACK, true); // 공격 애니메이션 켜기

        // (실제 데미지 판정은 입에 붙인 SmartDinoAttack 스크립트가 알아서 함)
    }

    // 사망 처리
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false; // 길막 방지

        anim.CrossFade("Die", 0.2f); // 사망 모션
        Destroy(gameObject, 3.0f);   // 3초 뒤 삭제
    }
}