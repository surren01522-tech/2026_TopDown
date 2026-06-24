using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동용

public class GameClearUIManager : MonoBehaviour
{
    public static GameClearUIManager Instance { get; private set; }

    [Header("게임 클리어 UI 패널")]
    public GameObject clearPanel; // 클리어 시 보여줄 패널

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 처음에는 클리어 창을 숨겨둡니다.
        if (clearPanel != null) clearPanel.SetActive(false);
    }

    // 클리어 창 띄우기
    public void ShowClearPanel()
    {
        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
            // 게임 일시정지 (선택 사항)
            Time.timeScale = 0f; 
        }
    }

    // 버튼용 함수: 타이틀 화면으로 돌아가기
    public void GoToTitle()
    {
        Time.timeScale = 1f; // 시간 다시 흐르게 설정
        SceneManager.LoadScene("TitleScene"); // 본인의 타이틀 씬 이름으로 변경
    }
}