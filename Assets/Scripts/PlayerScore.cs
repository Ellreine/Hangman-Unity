[System.Serializable]
public class PlayerScore
{
    public string playerName;
    public int score;
    public float time;

    public PlayerScore(string playerName, int score, float time)
    {
        this.playerName = playerName;
        this.score = score;
        this.time = time;
    }
}
