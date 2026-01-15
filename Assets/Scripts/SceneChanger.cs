using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("이동할 씬의 이름을 정확하게 입력하세요.")]
    public string nextSceneName;

    [Header("Time Settings")]
    [Tooltip("몇 초 뒤에 씬을 변경할지 설정하세요.")]
    public float delayTime = 3.0f;

    [Tooltip("체크하면 게임 시작 시 자동으로 카운트다운을 시작합니다.")]
    public bool autoStart = true;

    void Start()
    {
        // autoStart가 켜져 있다면 시작하자마자 코루틴 실행
        if (autoStart)
        {
            StartCoroutine(LoadSceneAfterDelay());
        }
    }

    // 외부(버튼 등)에서 호출하고 싶다면 이 함수를 연결하세요.
    public void TriggerSceneChange()
    {
        StartCoroutine(LoadSceneAfterDelay());
    }

    IEnumerator LoadSceneAfterDelay()
    {
        Debug.Log(delayTime + "초 뒤에 " + nextSceneName + " 씬으로 이동합니다.");

        // 설정한 시간만큼 대기
        yield return new WaitForSeconds(delayTime);

        // 씬 로드 시도
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("이동할 씬의 이름(Next Scene Name)이 비어있습니다!");
        }
    }
}