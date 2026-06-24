using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Game Setting Data")]
public class GameSettingData : ScriptableObject
{
    public int StartHP = 100;
    public int StartAttack = 10;
    public float PlayerMoveSpeed = 5f;

    public int HpBounsPerDeath = 5;
    public int AtkBonusPerDeath = 1;
}
