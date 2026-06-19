using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // URP 볼륨 제어를 위해 필수

public class ColorModeController : MonoBehaviour
{
    private Volume volume;
    private ColorAdjustments colorAdjustments;

    private void Awake()
    {
        volume = GetComponent<Volume>();

        // 볼륨 컴포넌트에서 ColorAdjustments(색조 조절) 프로파일을 찾습니다.
        if (volume != null && volume.profile != null)
        {
            volume.profile.TryGet(out colorAdjustments);
        }
    }

    private void Start()
    {
        // 시작할 때와 데이터 매니저의 설정이 바뀔 때 화면을 업데이트합니다.
        UpdateScreenColor();
    }

    private void Update()
    {
        // 실시간으로 GameDataManager의 상태를 감시하여 적용합니다.
        UpdateScreenColor();
    }

    private void UpdateScreenColor()
    {
        if (GameDataManager.Instance == null || colorAdjustments == null) return;

        // 컬러 모드이면 채도를 원래대로(0), 흑백 모드이면 채도를 바닥으로(-100) 내립니다.
        float targetSaturation = GameDataManager.Instance.isColorMode ? 0f : -100f;

        // URP 볼륨 값을 안전하게 변경합니다.
        colorAdjustments.saturation.Override(targetSaturation);
    }
}
