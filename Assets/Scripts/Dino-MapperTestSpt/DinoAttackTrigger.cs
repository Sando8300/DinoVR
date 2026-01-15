using UnityEngine;

public class DinoAttackTrigger : MonoBehaviour
{
    [Header("설정")]
    public int damage = 10;             // 데미지 (인스펙터에서 수정 가능)
    public float damageCooldown = 1.0f; // 연속 타격 방지 시간 (1초)

    // 내부 변수
    private Animator anim;
    private float lastDamageTime = 0f;

    // ★ 파일에서 직접 추출한 Jurassic Pack 공격 애니메이션 이름들 ★
    // 이 이름들과 현재 동작이 일치해야만 데미지가 들어갑니다.
    private string[] attackStateNames = new string[]
    {
        // 1. Troo (작은 육식공룡)
        "Troo|RunAtk1", "Troo|RunAtk2",
        "Troo|IdleAtk1", "Troo|IdleAtk2", "Troo|IdleAtk3",
        "Troo|GroundAtk", "Troo|JumpAtk",

        // 2. Dimo (익룡)
        "Dimo|RunAtk1", "Dimo|RunAtk2",
        "Dimo|IdleAtk3", "Dimo|FlyAtk",

        // 3. Pachy (박치기 공룡)
        "Pachy|RunAtk", "Pachy|RunAtk", // 코드에 중복 조건이 있어 둘 다 넣음
        "Pachy|IdleAtk"
    };

    void Start()
    {
        // 부모(공룡 몸체)에 있는 Animator를 찾아옵니다.
        anim = GetComponentInParent<Animator>();
    }

    // OnTriggerEnter 대신 Stay를 씁니다.
    // 이유: 플레이어가 이미 입 안에 있을 때 공격이 시작될 수도 있기 때문입니다.
    void OnTriggerStay(Collider other)
    {
        // 1. 플레이어인지 확인
        if (other.CompareTag("Player"))
        {
            // 2. 쿨타임 체크 (1초가 지났는지)
            if (Time.time - lastDamageTime < damageCooldown)
            {
                return; // 아직 때릴 타이밍 아님
            }

            // 3. ★ 핵심: 지금 공격 애니메이션 중인가? ★
            if (IsAttacking())
            {
                Debug.Log($" {transform.root.name}의 공격이 적중했습니다! (데미지: {damage})");

                // --- 데미지 처리 부분 (PlayerHealth 연결 시 주석 해제) ---
                /*
                var hp = other.GetComponent<PlayerHealth>(); // 혹은 GameManager
                if (hp != null)
                {
                    hp.TakeDamage(damage);
                }
                */
                // -----------------------------------------------------

                // 때렸으니 시간 기록 (쿨타임 시작)
                lastDamageTime = Time.time;
            }
        }
    }

    // 현재 애니메이션이 공격 목록에 있는지 검사하는 함수
    bool IsAttacking()
    {
        if (anim == null) return true; // 애니메이터가 없으면 그냥 닿으면 데미지 (안전장치)

        // 현재 0번 레이어(Base Layer)의 상태 정보 가져오기
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 등록해둔 공격 이름 리스트를 하나씩 확인
        foreach (string name in attackStateNames)
        {
            if (stateInfo.IsName(name))
            {
                return true; // "어! 지금 때리는 동작 중이다!"
            }
        }

        return false; // 공격 중 아님
    }
}