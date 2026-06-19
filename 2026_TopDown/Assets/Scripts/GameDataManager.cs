using System.IO;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [Header("Data References")]
    public GameSettingData gameSettingData;
    public SaveData saveData;

    [Header("Game Flags")]
    public int isTutorialFinished;

    // 👍 타이틀 UI 버튼 연동을 위해 추가된 세팅 변수들
    [Header("Settings")]
    public bool isColorMode = true;
    public bool isSoundOn = true;

    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Application.persistentDataPath + "/saveData.json";

            LoadJsonData();
            LoadPlayerPrefs();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- ScriptableObject & SaveData 연동 파트 ---
    public int GetPlayerHP()
    {
        if (gameSettingData == null || saveData == null) return 100; // 방어 코드

        int baseHP = gameSettingData.startHP;
        int bonusHP = gameSettingData.hpBounsPerDeath;

        return baseHP + bonusHP * saveData.deathCount;
    }

    public int GetPlayerAttack()
    {
        if (gameSettingData == null || saveData == null) return 10; // 방어 코드

        int baseAttack = gameSettingData.startAttack;
        int bonusAttack = gameSettingData.atkBonusPerDeath;
        return baseAttack + bonusAttack * saveData.deathCount;
    }

    public float GetPlayerMoveSpeed()
    {
        if (gameSettingData == null) return 5f; // 방어 코드

        return gameSettingData.playerMoveSpeed;
    }

    // --- Json 저장 파트 ---
    public void SaveGameResult()
    {
        if (saveData != null)
        {
            saveData.deathCount++;
        }
        SaveJsonData();
    }

    public void SaveJsonData()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);

        Debug.Log("Json 저장 완료: " + savePath);
    }

    public void LoadJsonData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            saveData = new SaveData();
            SaveJsonData();
        }
    }

    public void DeleteJsonData()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        saveData = new SaveData();
        SaveJsonData();

        Debug.Log("JSON 저장 데이터 삭제");
    }

    // --- PlayerPrefs 파트 ---
    public void LoadPlayerPrefs()
    {
        isTutorialFinished = PlayerPrefs.GetInt("TUTORIAL", 0);
    }

    public void SavePlayerPrefs()
    {
        // 💡 기존 코드의 오타("TUTORAL")를 "TUTORIAL"로 통일하여 수정했습니다.
        PlayerPrefs.SetInt("TUTORIAL", isTutorialFinished);
        PlayerPrefs.Save(); // 명시적 저장 추가

        Debug.Log("PlayerPrefs 저장 완료");
    }
}