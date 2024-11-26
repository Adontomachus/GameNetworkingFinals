using System;
using Fusion;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(HealthChanged))]
    private float NetworkedHealth { get; set; }
    public GameObject team1Spawn, team2Spawn;
    private float timer = 0f;
    public float currentHealthAmount;
    private const float MaxHealth = 200f;
    public PlayerMovement playerMovement;

    public event Action<float> OnDamageEvent;

    private void Start()
    {

        team1Spawn = GameObject.FindGameObjectWithTag("Team1Spawn");
        team2Spawn = GameObject.FindGameObjectWithTag("Team2Spawn");
    }
    public override void Spawned()
    {

        NetworkedHealth = MaxHealth;
    }

    private void Update()
    {
        currentHealthAmount = NetworkedHealth;
    }
    void HealthChanged()
    {
        Debug.Log($"Health changed to: {NetworkedHealth}");
        OnDamageEvent?.Invoke(NetworkedHealth/MaxHealth);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealDamageRpc(float damage)
    {

        Debug.Log("Received DealDamageRpc on StateAuthority, modifying Networked variable");
        NetworkedHealth -= damage;
        
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RespawnRpc()
    {

        Debug.Log("Player has been killed!");
        NetworkedHealth = MaxHealth;

    }
    public override void FixedUpdateNetwork()
        
    {
        /*
        transform.position = team1Spawn.transform.position;
        if (NetworkedHealth < 0)
        {
            var a = playerMovement.GetComponent<PlayerMovement>();
            a._controller.enabled = false;
            
            RespawnRpc();
        }
        var b = playerMovement.GetComponent<PlayerMovement>();
        if (b._controller.enabled = false)
        {
            //timer for enable
            timer += Time.deltaTime;
        }
        if (timer > 0.25f)
        {
            b._controller.enabled = true;
        }
        */
    }

}