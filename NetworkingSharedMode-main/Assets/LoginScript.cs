using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Converters;
using UnityEditor;
using UnityEngine.TextCore.Text;
using System.Linq;
using UnityEngine.Networking;


public class LogInScript : MonoBehaviour
{
    private const string LOCALHOST1 = "http://localhost:3000/players";
    private const string LOCALHOST2 = "http://localhost:3000/players/player4";
    private List<UserCredentials> players = new List<UserCredentials>();
    private const string TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwbGF5ZXJJZCI6ImFzZGFzZCIsInBhc3N3b3JkIjoiYXNkYXNkIiwiaWF0IjoxNzMyNDU3ODgyLCJleHAiOjE3MzI1NDQyODJ9.32QixiJIfr0oClHbe4vrb-p2DSaTSoDbBhFCftdTgYs";
    public TMP_InputField username;
    public TMP_InputField password;
    public TMP_InputField newEmail;
    public TMP_InputField newPassword;
    public string updatedEmail;
    public string updatedUsername;
    public string updatedPassword;
    public string existingemail;
    public string existingusername;
    public string existingpassword;
    public string inputtedPlayerName;
    string url = "http://localhost:3000/users/1";
    public bool loggedIn = false;
    public GameObject loginScreen, mainUI, currentPlayer, playerStat;

    //public TMP_InputField loginusername;
    //public TMP_InputField loginpassword;
    //public TMP_InputField loginconfirmPassword;
    public GameObject invalidPassword;
    private Dictionary<string, string> credentials = new Dictionary<string, string>();
    // Start is called before the first frame update

    private IEnumerator Start()
    {
        

        yield return LocalHostGetRequest(LOCALHOST1);

    }
    public void PlayerRegister()
    {
        var user = new UserCredentials()
        {
            playername = username.text,
            password = password.text,
        };
        if (password.text != existingpassword)
        {
            invalidPassword.SetActive(true);
            Debug.Log("Invalid username or password.");
        }
        else if (credentials.ContainsKey(user.playername))
        {
            Debug.Log("Username already exists.");
        }
        else if (password.text == existingpassword)
        {
            credentials.Add(user.playername, user.password);
            Debug.Log("Successfully Registered!");
            Debug.Log(JsonConvert.SerializeObject(user));
        }

    }
    public void PlayerLogin()
    {
        string loginUser = username.text;
        string loginPass = password.text;

        Debug.Log("User attempting to log in: " + loginUser);

        bool isValidLogin = false;

        foreach (var player in players)
        {
            if (player.playername == loginUser && player.password == loginPass)
            {
                isValidLogin = true;
                existingusername = player.playername;  
                existingpassword = player.password;  
                break;
            }
        }

        if (isValidLogin)
        {
            loginScreen.SetActive(false);
            loggedIn = true;
            Debug.Log("Login successful!");
        }
        else
        {
            invalidPassword.SetActive(true); 
            Debug.Log("Invalid credentials.");
        }
    }

    private void Update()
    {

        if (!loggedIn)
        {
            mainUI.SetActive(false);
        }
        if (loggedIn)
        {
            mainUI.SetActive(true);
        }
    }
    private IEnumerator LocalHostGetRequest(string uri)
    {
        Debug.Log("Request Delivered:");
        var request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or
            UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            var users = JsonConvert.DeserializeObject<List<UserCredentials>>(request.downloadHandler.text);
            players = users;
            Debug.Log("Existing Users: " + request.downloadHandler.text);
            foreach (var user in users)
            {
                // existingemail = user.email;
                existingusername = user.playername;
                existingpassword = user.password;
                Debug.Log("Existing email: " + existingemail);
                Debug.Log("Existing username: " + existingusername);
                Debug.Log("Existing password: " + existingpassword);
            }
        }
    }

    //UPDATING TABLES
    public void PlayerRegisterWithKillsAndDeaths()
    {
        var user = new UserCredentials()
        {
            playername = username.text,
            password = password.text,
            kills = 0,
            deaths = 0
        };

        StartCoroutine(PostUserRegistration(user));
    }
    public void DeleteUserCredentials()
    {
        var user = new UserCredentials()
        {
            playername = username.text,
            password = password.text,
            kills = 0,
            deaths = 0
        };

        StartCoroutine(HttpDeleteRequest(user));
    }
    private IEnumerator PostUserRegistration(UserCredentials user)
    {

        string checkUrl = "http://localhost:3000/players?playername=" + user.playername;

        using (UnityWebRequest checkRequest = UnityWebRequest.Get(checkUrl))
        {
            yield return checkRequest.SendWebRequest();

            // Handle the response
            if (checkRequest.result == UnityWebRequest.Result.Success)
            {
                var users = JsonConvert.DeserializeObject<List<User>>(checkRequest.downloadHandler.text);
                if (users.Count == 0)
                {
                    Debug.Log("Username not found.");
                    invalidPassword.SetActive(true);
                }
                var storedUser = users[0];
                if (storedUser.password != user.password) 
                {
                    Debug.Log("Incorrect password.");
                    invalidPassword.SetActive(true);
                }
                // Password is correct, log the user in
                currentPlayer.GetComponent<PlayerNameUI>().playernameText = username.text;
                currentPlayer.GetComponent<PlayerNameUI>().getUsers = true;
                Debug.Log("User logged in successfully.");
            }
            else
            {
                Debug.LogError("Error checking username: " + checkRequest.error);
            }
        }
    }
    private IEnumerator HttpDeleteRequest(UserCredentials user)
    {
        string checkUrl = "http://localhost:3000/players?playername=" + user.playername;

        using (UnityWebRequest checkRequest = UnityWebRequest.Get(checkUrl))  // Use GET to check if player exists
        {
            yield return checkRequest.SendWebRequest();

            if (checkRequest.result == UnityWebRequest.Result.Success)
            {
                var users = JsonConvert.DeserializeObject<List<UserCredentials>>(checkRequest.downloadHandler.text);
                if (users.Count == 0)
                {
                    Debug.Log("Username not found.");
                    invalidPassword.SetActive(true);
                }
                var storedUser = users[0];
                if (storedUser.password != user.password)
                {
                    Debug.Log("Incorrect password.");
                    invalidPassword.SetActive(true);
                }
                string deleteUrl = "http://localhost:3000/players/" + storedUser.playername; 
                using (UnityWebRequest deleteRequest = UnityWebRequest.Delete(deleteUrl))
                {
                    yield return deleteRequest.SendWebRequest();

                    if (deleteRequest.result == UnityWebRequest.Result.Success)
                    {
                        Debug.Log("User deleted successfully!");
                        currentPlayer.GetComponent<PlayerNameUI>().playernameText = string.Empty;
                        currentPlayer.GetComponent<PlayerNameUI>().getUsers = false;
                    }
                    else
                    {
                        Debug.LogError("Error deleting user: " + deleteRequest.error);
                    }
                }
            }
            else
            {
                Debug.LogError("Error checking username: " + checkRequest.error);
            }
        }
    }

    public struct UserCredentials
    {
        public string playername;
        public string password;
        public int kills;
        public int deaths;
    };
}
