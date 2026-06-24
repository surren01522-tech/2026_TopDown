using UnityEngine;
using TMPro; // TextMeshPro 제어용

public class GameUIController : MonoBehaviour
{
    [Header("연동할 게임 세팅 데이터")]
    public GameSettingData gameSetting; // 타이틀에서 수정한 데이터 에셋을 연결

    [Header("인게임 하단 상태창 텍스트")]
    public TextMeshProUGUI statusText; // 하단에 배치할 한 줄짜리 텍스트 UI

    // 예시용 인게임 실시간 데이터 (원하는 대로 확장 가능)
    private int currentLevel = 1;
    private int currentHP;

    private void Start()
    {
        if (gameSetting == null)
        {
            Debug.LogError("GameSettingData 에셋이 연결되지 않았습니다!");
            return;
        }

        // 시작 체력은 타이틀 세팅 패널에서 설정한 StartHP 값으로 세팅됩니다.
        currentHP = gameSetting.StartHP;

        // 화면 UI 텍스트 업데이트
        UpdateStatusUI();
    }

    // 🖥️ 하단 한 줄 상태창을 클래식 스타일로 업데이트하는 함수
    public void UpdateStatusUI()
    {
        if (gameSetting == null || statusText == null) return;

        // 참고 이미지 스타일: Lev:2  HP:29(31)  Str:16(16)
        // StartAttack과 PlayerMoveSpeed를 활용해 한 줄로 구성합니다.
        statusText.text = $"Lev:{currentLevel}   HP:{currentHP}({gameSetting.StartHP})   Atk:{gameSetting.StartAttack}   Str:16(16)   Spd:{gameSetting.PlayerMoveSpeed:F1}   Adventurer";
    }

    // 외부(플레이어 스크립트 등)에서 체력이 변했을 때 호출할 수 있는 함수 예시
    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
        UpdateStatusUI();
    }
}
