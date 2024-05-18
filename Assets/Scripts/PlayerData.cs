[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int bestScore;
    public float bestTime;
    public int currentScore;
    public float currentTime;

    public PlayerData(string playerName)
    {
        this.playerName = playerName;
        this.bestScore = 0;
        this.bestTime = 0f;
        this.currentScore = 0;
        this.currentTime = 0f;
    }

    public void ResetCurrentScore()
    {
        this.currentScore = 0;
        this.currentTime = 0f;
    }
}
