using UnityEngine;
using TMPro; // TextMeshPro 기능을 사용하기 위해 필수 추가

public class TitleUIController : MonoBehaviour
{
    [Header("UI Text References")]
    public TextMeshProUGUI colorButtonText; // Color On/Off 텍스트 컴포넌트
    public TextMeshProUGUI soundButtonText; // Sound On/Off 텍스트 컴포넌트

    private void Start()
    {
        // 게임 시작 시 GameDataManager에 기록된 기존 세팅값에 맞춰 UI 텍스트 초기화
        UpdateColorUI();
        UpdateSoundUI();
    }

    // [Color] 버튼을 눌렀을 때 실행될 함수
    public void ToggleColorMode()
    {
        if (GameDataManager.Instance == null) return;

        // true ↔ false 토글(반전)
        GameDataManager.Instance.isColorMode = !GameDataManager.Instance.isColorMode;

        // 텍스트 UI 업데이트
        UpdateColorUI();

        // 💡 (선택 사항) 여기에 카메라 흑백 전환 프리셋이나 포스트 프로세싱 연동 코드를 넣을 수 있습니다.
        Debug.Log($"컬러 모드 변경됨: {GameDataManager.Instance.isColorMode}");
    }

    // [Sound] 버튼을 눌렀을 때 실행될 함수
    public void ToggleSound()
    {
        if (GameDataManager.Instance == null) return;

        // true ↔ false 토글(반전)
        GameDataManager.Instance.isSoundOn = !GameDataManager.Instance.isSoundOn;

        // 텍스트 UI 업데이트
        UpdateSoundUI();

        // 유니티 마스터 볼륨을 켜고(1) 끄기(0)
        AudioListener.volume = GameDataManager.Instance.isSoundOn ? 1f : 0f;

        Debug.Log($"사운드 상태 변경됨: {GameDataManager.Instance.isSoundOn}");
    }

    private void UpdateColorUI()
    {
        if (GameDataManager.Instance == null || colorButtonText == null) return;
        colorButtonText.text = GameDataManager.Instance.isColorMode ? "Color On" : "Color Off";
    }

    private void UpdateSoundUI()
    {
        if (GameDataManager.Instance == null || soundButtonText == null) return;
        soundButtonText.text = GameDataManager.Instance.isSoundOn ? "Sound On" : "Sound Off";
    }
}