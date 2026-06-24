using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LogUIManager : MonoBehaviour
{
    // ✨ 다른 스크립트들이 언제든 쉽게 접근할 수 있게 해주는 싱글톤 인스턴스
    public static LogUIManager Instance { get; private set; }

    [Header("UI 요소 연결 (인스펙터에서 드래그)")]
    public GameObject logCanvasWindow;    // 껐다 켰다 할 Scroll View 오브젝트
    public TextMeshProUGUI logTextPrefab; // 우리가 둥근모꼴 폰트를 설정한 프리팹
    public TextMeshProUGUI NotificationText;
    public Transform logContent;          // UI 내의 Content 오브젝트
    public ScrollRect scrollRect;         // UI 내의 Scroll View 오브젝트 자체

    [Header("설정")]
    public int maxLogCount = 20;
    private List<GameObject> activeLogs = new List<GameObject>();

    private void Awake()
    {
        // 🛠️ 싱글톤 초기화 로직
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // 🎮 게임이 처음 시작될 때는 UI 창을 보이지 않게 꺼둡니다.
        if (logCanvasWindow != null)
        {
            logCanvasWindow.SetActive(false);
        }

        Debug.Log("[시스템] LogUIManager가 정상적으로 유니티 세상에 소환되었습니다.");
    }

    private void Update()
    {
        // ⌨️ O 키를 누르면 로그 창 토글(Toggle)
        if (Input.GetKeyDown(KeyCode.O))
        {
            ToggleLogWindow();
        }
    }

    // 🔄 창을 열고 닫는 함수
    public void ToggleLogWindow()
    {
        if (logCanvasWindow != null)
        {
            bool currentState = logCanvasWindow.activeSelf;
            logCanvasWindow.SetActive(!currentState);

            // 창이 새로 켜질 때 스크롤바를 맨 아래로 리셋
            if (!currentState == true)
            {
                Canvas.ForceUpdateCanvases();
                if (scrollRect != null) scrollRect.verticalNormalizedPosition = 0f;
            }
        }
    }

    // 💬 텍스트 윈도우와 유니티 콘솔 창 양쪽에 로그를 새기는 핵심 함수
    public void AddLog(string message, Color textColor)
    {
        Debug.Log(message);

        if (logTextPrefab == null || logContent == null)
        {
            Debug.LogWarning("LogUIManager: 프리팹이나 Content 오브젝트 연결이 누락되었습니다!");
            return;
        }

        // ✨ [수정] 생성할 때 부모(logContent)를 넣어주는 것 하나만으로 충분합니다!
        // 중복으로 들어가 있던 SetParent 라인을 지웠습니다.
        TextMeshProUGUI newLog = Instantiate(logTextPrefab, logContent);
        NotificationText.text = message; 

        newLog.text = message;
        newLog.color = textColor;
        activeLogs.Add(newLog.gameObject);

        // 4. 최대 개수를 넘어가면 가장 오래된 로그 삭제
        if (activeLogs.Count > maxLogCount)
        {
            Destroy(activeLogs[0]);
            activeLogs.RemoveAt(0);
        }

        // 5. 자동 스크롤 하단 고정
        Canvas.ForceUpdateCanvases();
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}