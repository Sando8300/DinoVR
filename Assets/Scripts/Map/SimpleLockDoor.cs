using UnityEngine;

public class SimpleLockDoor : MonoBehaviour
{
    private Animator anim;

    void Awake()
    {
        anim = GetComponentInParent<Animator>();

        // 게임 시작 시: 이미 열려 있는 상태로 시작
        PlayAnim("Static Open");
    }

    // 문 열기 명령
    public void Open()
    {
        // 문이 열리는 애니메이션 재생
        PlayAnim("Gate Open");
    }

    // 문 닫기 명령
    public void Close()
    {
        // 문이 닫히는 애니메이션 재생
        PlayAnim("Gate Close");
    }

    // 애니메이션 재생 헬퍼 함수
    void PlayAnim(string stateName)
    {
        if (anim != null)
        {
            anim.Play(stateName);
        }
    }
}