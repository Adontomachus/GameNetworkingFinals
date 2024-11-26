using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class FirstPersonCamera : MonoBehaviour
{
    private bool win;
    public bool playerInGame = false;
    public GameObject loginUI;
    public TextMeshProUGUI text;
    public float healthAmount;
    public int totalScore, kills, deaths;
    public Transform Target;
    public GameObject Authority;

    public float MouseSensitivity = 10f;
    public string playerName;
    private float verticalRotation;
    private float horizontalRotation;

    private void Awake()
    {
    }
    public void InGameChecker()
    {
        playerInGame = true;
    }
    private void Update()
    {
        if (playerInGame) { loginUI.SetActive(false); }
        PlayerMovement b = Authority.GetComponent<PlayerMovement>();
        totalScore = b.playerScore;
        PlayerHealth a = Authority.GetComponent<PlayerHealth>();
        healthAmount = a.currentHealthAmount;
    }
    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        transform.position = Target.position;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        verticalRotation -= mouseY * MouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);
        horizontalRotation += mouseX * MouseSensitivity;

        transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
    }

}