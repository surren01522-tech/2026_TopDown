using UnityEngine;

public class TitleManager : MonoBehaviour
{
    public void  GameStartButton()
    {
        GameManager.Instance.StartGame();
    }
}
