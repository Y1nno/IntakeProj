using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    
    public static NetworkPlayer Local {get; set;}
    
    void Start()
    {
        
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            Debug.Log("Local player spawned: " + Object.InputAuthority);
        }
        else
        {
            Debug.Log("Remote player spawned: " + Object.InputAuthority);
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
        Debug.Log("Player left: " + player);
    }
}
