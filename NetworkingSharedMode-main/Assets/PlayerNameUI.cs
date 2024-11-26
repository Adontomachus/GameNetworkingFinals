using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml.Linq;

public class PlayerNameUI : MonoBehaviour
{
    private const string USER1 = "https://reqres.in/api/users/1";
    private const string USER2 = "http://localhost:3000/players/";
    public string playernameText;
    public TextMeshProUGUI userUI;
    public TextMeshProUGUI killUI;
    public TextMeshProUGUI deathUI;
    public bool getUsers = false;
    bool getOnlyOnce = true;
    // Start is called before the first frame update
    private void Update()
    {
        if (getUsers && getOnlyOnce)
        {
            Debug.Log("Working as intended.");
            StartCoroutine(RequestPlayerData());
            getOnlyOnce = false;

        }
    }
    private IEnumerator RequestPlayerData()
    {
        yield return HttpGetRequest(USER2 + playernameText);
    }
    private IEnumerator HttpGetRequest(string uri)
    {
        Debug.Log("PLEASE WORK");
        var request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or
            UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            var users = JsonConvert.DeserializeObject<List<UserData>>(request.downloadHandler.text);
            Debug.Log("Response: " + request.downloadHandler.text);
            foreach (var user in users)
            {
                Debug.Log("Player Name: " + user.playername);
                userUI.text = "Player Name: " + user.playername;
                killUI.text = "Kills: " + user.kills;
                deathUI.text = "Deaths: " + user.deaths;
            }
        }
    }
    private IEnumerator RegisterAccount(string uri, string data)
    {
        Debug.Log("Posting data: " + data);
        var request = UnityWebRequest.Post(uri, data, "application/json");
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or
            UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            var auth = JsonConvert.DeserializeObject<Authentication>(request.downloadHandler.text);
        }
    }
}

[System.Serializable]
public struct Authentication
{
    public UserData data;
}

public struct UserData
{
    public string playername;
    public int kills;
    public int deaths;
}


public struct User
{
    public string id;
    public string email;
    public string username;
    public string password;
}
public struct User2
{
    public string id;
    public string email;
    public string username;
    public string password;
}

