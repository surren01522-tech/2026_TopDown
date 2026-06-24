using UnityEngine;
using TMPro;

public class TitleUIController : MonoBehaviour
{
    [Header("연동할 게임 세팅 데이터")]
    public GameSettingData gameSetting; 

    [Header("조작할 패널 오브젝트 (Esc용)")]
    public GameObject settingPanel; // 하이어라키의 'Panel' 오브젝트를 꼭 연결해 주세요!

    [Header("입력 필드 UI")]
    public TMP_InputField hpInputField;    
    public TMP_InputField atkInputField;   
    public TMP_InputField speedInputField; 

    private void Start()
    {
        RefreshInputFieldValues();
    }

    private void Update()
    {
        // 키보드 Esc 키 입력을 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapeKey();
        }
    }

    // 🚪 Esc 키 처리 로직 (오직 패널만 종료)
    private void HandleEscapeKey()
    {
        if (settingPanel == null) return;

        // [안전장치 1] 만약 인풋필드 타이핑 중에 Esc를 누른 거라면, 입력창 포커스부터 먼저 풀어줍니다.
        if (hpInputField != null && hpInputField.isFocused) { hpInputField.DeactivateInputField(); return; }
        if (atkInputField != null && atkInputField.isFocused) { atkInputField.DeactivateInputField(); return; }
        if (speedInputField != null && speedInputField.isFocused) { speedInputField.DeactivateInputField(); return; }

        // 패널이 활성화되어(켜져) 있다면 패널을 비활성화(종료)합니다.
        if (settingPanel.activeSelf == true)
        {
            settingPanel.SetActive(false);
            Debug.Log("Esc 입력: 세팅 패널을 성공적으로 닫았습니다.");
        }
    }

    // 🔓 [새로 추가] 메인 화면의 'Player setting' 버튼을 누르면 패널을 열어주는 함수
    public void OpenSettingPanel()
    {
        if (settingPanel != null)
        {
            // 1. 숨겨져 있던 패널을 화면에 보이게 켭니다.
            settingPanel.SetActive(true);
            
            // 2. 패널이 열릴 때 ScriptableObject의 최신 수치를 인풋필드에 한 번 더 새로고침해 줍니다.
            RefreshInputFieldValues(); 
            Debug.Log("버튼 클릭: 세팅 패널을 열었습니다.");
        }
    }

    // 🔒 패널 내부의 'esc' 버튼을 누르면 패널을 닫아주는 함수
    public void CloseSettingPanel()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
            Debug.Log("버튼 클릭: 세팅 패널을 닫았습니다.");
        }
    }

    // 🔄 데이터 동기화
    public void RefreshInputFieldValues()
    {
        if (gameSetting == null) return;

        if (hpInputField != null) hpInputField.text = gameSetting.StartHP.ToString();
        if (atkInputField != null) atkInputField.text = gameSetting.StartAttack.ToString();
        if (speedInputField != null) speedInputField.text = gameSetting.PlayerMoveSpeed.ToString("F1"); 
    }

    public void OnHPInputEnd(string rawText)
    {
        if (gameSetting == null) return;
        if (int.TryParse(rawText, out int parsedValue))
        {
            gameSetting.StartHP = Mathf.Max(10, parsedValue);
        }
        RefreshInputFieldValues();
        SaveSetting();
    }

    public void OnAtkInputEnd(string rawText)
    {
        if (gameSetting == null) return;
        if (int.TryParse(rawText, out int parsedValue))
        {
            gameSetting.StartAttack = Mathf.Max(1, parsedValue);
        }
        RefreshInputFieldValues();
        SaveSetting();
    }

    public void OnSpeedInputEnd(string rawText)
    {
        if (gameSetting == null) return;
        if (float.TryParse(rawText, out float parsedValue))
        {
            gameSetting.PlayerMoveSpeed = Mathf.Max(1f, parsedValue);
        }
        RefreshInputFieldValues();
        SaveSetting();
    }

    private void SaveSetting()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameSetting);
#endif
    }
}