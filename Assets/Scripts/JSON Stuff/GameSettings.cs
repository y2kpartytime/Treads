using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/GameSettings")]
public class GameSettings : ScriptableObject
{
    public int defaultHealth;
    public float playerSpeed;
}