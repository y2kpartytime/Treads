[System.Serializable]
public struct GamePlayData
{
    public int score;
    public float playerX;
    public float playerY;
}

[System.Serializable]
public struct StatisticalData
{
    public int highScore;
    public int lastScore;
}

[System.Serializable]
public struct EnvironmentData
{
    public float objectX;
    public float objectY;
}