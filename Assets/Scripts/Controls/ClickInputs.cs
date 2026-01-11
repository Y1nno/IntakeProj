using Fusion;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SimpleClickToMove : NetworkBehaviour
{
    private Camera mainCamera;
    private LayerMask groundMask;

    private InputAction selectAction;

    public List<UnitMovement> ownedUnits = new List<UnitMovement>();

    private void Start()
    {
        selectAction = InputSystem.actions.FindAction("Select");
    }
    
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            if (mainCamera == null) mainCamera = Camera.main;
            if (groundMask == 0) groundMask = LayerMask.GetMask("Ground");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;
        if (ownedUnits.Count == 0) return;

        if (selectAction.WasPerformedThisFrame())
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 500f, groundMask))
            {
                UnitMovement unit = ownedUnits[0];
                if (unit == null || unit.Object == null) return;
                if (unit.Object.InputAuthority != Object.InputAuthority) return;
                RPC_RequestMove(unit.Object, hit.point);
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestMove(NetworkObject unitObj, Vector3 target)
    {
        if (unitObj == null) return;

        var unit = unitObj.GetComponent<UnitMovement>();
        if (unit == null) return;

        if (unitObj.InputAuthority != Object.InputAuthority) return;

        unit.RPC_RequestMove(target);
    }
}
