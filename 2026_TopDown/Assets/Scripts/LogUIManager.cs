using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LogUIManager : MonoBehaviour
{
    public static LogUIManager Instance { get; private set; }

    [Header("UI 요소 연결")]
    public TextMeshProUGUI logTextPrefab; // 한 줄의 로그가 될 텍스트 프리팹
    public Transform logContent;          // Scroll View의 Content 오브젝트
    public ScrollRect scrollRect;         // Scroll View 자체

    [Header("설정")]
    public int maxLogCount = 20;          // 화면에 유지할 최대 로그 개수 (너무 많으면 느려짐)
    private List<GameObject> activeLogs = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 💬 어디서나 호출할 수 있는 메시지 추가 함수!
    public void AddLog(string message, Color textColor)
    {
        // 1. 새로운 텍스트 오브젝트 생성
        TextMeshProUGUI newLog = Instantiate(logTextPrefab, logContent);
        newLog.text = message;
        newLog.color = textColor;
        activeLogs.Add(newLog.gameObject);

        // 2. 최대 로그 개수를 초과하면 오래된 로그부터 삭제
        if (activeLogs.Count > maxLogCount)
        {
            Destroy(activeLogs[0]);
            activeLogs.RemoveAt(0);
        }

        // 3. 새 로그가 추가되었으니 스크롤바를 자동으로 맨 아래로 내리기
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}