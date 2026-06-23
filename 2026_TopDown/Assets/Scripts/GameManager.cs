
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 👍 중복 선언을 정리하고 하나의 싱글톤 인스턴스로 통일했습니다.
    public static GameManager Instance { get; private set; }

    [Header("Scene References")]
    public string titleSceneName = "TitleScene";
    public string gameSceneName = "GameScene"; // 기존 고정 씬 이름

    [Header("Random Scene Settings")]
    // 🎲 여기에 인스펙터에서 10개의 맵 이름을 넣어주세요!
    public List<string> mapSceneNames = new List<string>();

    private void Awake()
    {
        // 👍 두 개로 나뉘어 있던 Awake 로직을 하나로 합치고 DontDestroyOnLoad를 보존했습니다.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🎲 10개의 맵 중 하나를 무작위로 골라 이동하는 핵심 함수
    public void LoadRandomMapScene()
    {
        if (mapSceneNames == null || mapSceneNames.Count == 0)
        {
            Debug.LogError("GameManager의 Map Scene Names 목록이 비어있습니다! 인스펙터에서 씬 이름을 적어주세요.");
            // 혹시 리스트가 비어있다면 에러 방지를 위해 기존 기본 게임 씬으로 보냅니다.
            SceneManager.LoadScene(gameSceneName);
            return;
        }

        // 0부터 리스트 개수 직전까지의 정수 중 랜덤 하나를 뽑습니다.
        int randomIndex = Random.Range(0, mapSceneNames.Count);
        string targetSceneName = mapSceneNames[randomIndex];

        Debug.Log($"🎲 랜덤 맵 매칭 성공! 이동할 씬: {targetSceneName}");

        // 해당 랜덤 씬으로 전환합니다.
        SceneManager.LoadScene(targetSceneName);
    }

    // 💡 기존의 StartGame 기능을 랜덤 시스템과 연결했습니다!
    public void StartGame()
    {
        // 이제 고정된 gameSceneName 대신 랜덤 함수를 호출하여 출발합니다.
        LoadRandomMapScene();
    }

    public void GameStartButton()
    {
        // 버튼 클릭 이벤트 함수도 정상 작동합니다.
        StartGame();
    }

    public void GameOver()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SaveGameResult();
        }
        GoTitle();
    }

    public void GoTitle()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}