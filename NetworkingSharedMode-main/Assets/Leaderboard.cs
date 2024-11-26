using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class PlayerDataDisplay : MonoBehaviour
{
    [System.Serializable]
    public class Player
    {
        public string playername;
        public string password;
        public int kills;
        public int deaths; 
    }

    public TMP_Text playerDataText;

    private string url = "http://localhost:3000/players";

    void Start()
    {
        StartCoroutine(FetchPlayerData());
    }
    IEnumerator FetchPlayerData()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = www.downloadHandler.text;
            Player[] players = JsonHelper.FromJson<Player>(jsonResponse);

            DisplayPlayerData(players);
        }
        else
        {
            Debug.LogError("Error fetching player data: " + www.error);
        }
    }

    void DisplayPlayerData(Player[] players)
    {
        string displayText = "";

        foreach (var player in players)
        {
            displayText += $"{player.playername} Kills: { player.kills} \n";
        }

        playerDataText.text = displayText;
    }
}
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrappedJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}