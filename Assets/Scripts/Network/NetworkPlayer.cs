using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    
    public static NetworkPlayer Local {get; set;}
    public int PlayerID {get; set;}
    
    void Start()
    {
        
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            PlayerID = Object.InputAuthority.PlayerId;
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

    public void RegisterOwnedUnit(UnitMovement unit)
    {
        if (unit == null) return;

        var clickToMove = GetComponent<SimpleClickToMove>();
        if (clickToMove == null) return;

        if (!clickToMove.ownedUnits.Contains(unit))
        {
            clickToMove.ownedUnits.Add(unit);
        }
    }

    public void UnregisterOwnedUnit(UnitMovement unit)
    {
        if (unit == null) return;

        var clickToMove = GetComponent<SimpleClickToMove>();
        if (clickToMove == null) return;

        clickToMove.ownedUnits.Remove(unit);
    }
}
