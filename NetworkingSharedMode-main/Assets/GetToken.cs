using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Networking;

public class GetToken : MonoBehaviour
{
    private const string TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwbGF5ZXJJZCI6ImFzZGFzZCIsInBhc3N3b3JkIjoiYXNkYXNkIiwiaWF0IjoxNzMyNDYyNzA2LCJleHAiOjE3MzI1NDkxMDZ9.K-jFdvihnf6zF5hBXxfE073LONJM0_TT5FSKUxK9_pQ";

    // Start is called before the first frame update
    IEnumerator Start()
    {
        using (var request = new UnityWebRequest("http://localhost:3000/players", "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {TOKEN}");
            yield return request.SendWebRequest();
            if (request.result is UnityWebRequest.Result.Success)
            {
                Debug.Log($"Request Success: {request.downloadHandler.text}");
            }
            else
            {
                Debug.Log($"Request Error: {request.error}");
            }
        }
    }

}
