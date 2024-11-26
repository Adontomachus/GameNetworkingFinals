using Fusion;
using UnityEngine;

public class RaycastAttack : NetworkBehaviour
{
    public float Damage = 10;
    public GameObject TargetHit;
    public GameObject Hitmarker;
    private float timer = 0f;

    public PlayerMovement PlayerMovement;

    public const float fireDelay = .2f;
    public float delayTimeLeft;

    private void Awake()
    {

        Hitmarker = GameObject.FindGameObjectWithTag("Hitmarker");
    }
    public override void Spawned()
    {
        Hitmarker = GameObject.FindGameObjectWithTag("Hitmarker");
    }
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        RaycastHit hit;
        var ray = PlayerMovement.mainCamera.ScreenPointToRay(Input.mousePosition);
        ray.origin += PlayerMovement.mainCamera.transform.forward;

        delayTimeLeft -= Runner.DeltaTime;
        if (!Input.GetMouseButton(0)) return;
        if (delayTimeLeft > 0) return;
        delayTimeLeft = fireDelay;
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 1f);
        Debug.Log("Firing");
        
        if (!Physics.Raycast(ray.origin, ray.direction, out hit)) return;
        Debug.Log($"Firing and hit {hit.collider.gameObject.name}");

        NetworkObject.Instantiate(TargetHit, hit.point, Quaternion.LookRotation(hit.normal));
        if (!hit.collider.TryGetComponent<PlayerHealth>(out var health)) return;
        PlayerMovement.playerScore += 20;
        Hitmarker.SetActive(true);
        timer = 0.15f;
        Debug.Log("Hit and dealing Damage");
        health.DealDamageRpc(Damage);

    }

    private void Update()
    {

            timer -= Time.deltaTime;

        if (timer < 0)
        {
            Hitmarker.SetActive(false);
        }
    }
}