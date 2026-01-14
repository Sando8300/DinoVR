using UnityEngine;
using UnityEngine.Playables; // 타임라인 제어를 위해 필수

/// <summary>
/// XRI Push Button과 연결하여 타임라인을 재생하는 스크립트입니다.
/// </summary>
public class EndingTimeline : MonoBehaviour
{
    [Header("Timeline Settings")]
    [Tooltip("재생할 타임라인 디렉터 컴포넌트")]
    public PlayableDirector targetTimeline;

    [Tooltip("체크하면 게임 중 딱 한 번만 실행됩니다.")]
    public bool playOnce = true;

    private bool hasPlayed = false;

    /// <summary>
    /// 버튼이 눌렸을 때 외부(XR Push Button)에서 호출할 함수입니다.
    /// </summary>
    public void PlayTimeline()
    {
        // 1. 이미 재생했고, 한 번만 재생 옵션이 켜져있다면 무시
        if (playOnce && hasPlayed) return;

        // 2. 타임라인이 할당되어 있다면 재생
        if (targetTimeline != null)
        {
            // 이미 재생 중이라면 처음부터 다시 재생할지, 무시할지 결정 (여기선 처음부터 재생)
            if (targetTimeline.state == PlayState.Playing)
            {
                targetTimeline.Stop();
            }

            targetTimeline.Play();
            hasPlayed = true;
            Debug.Log("Button Pressed: Timeline Started.");
        }
        else
        {
            Debug.LogWarning("Target Timeline is not assigned!");
        }
    }
}