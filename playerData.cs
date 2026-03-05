using UnityEngine;

[System.Serializable]
public class playerData {
    public float level;
    public float xp;
    public int money;

    public playerData(playerStats PlayerStats){
        level = PlayerStats.playerLevel_();
        xp = PlayerStats.playerXp_();
        money = PlayerStats.playerMoney_();
    }
}