using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class SimpleClickToMove : NetworkBehaviour
{
    private Camera mainCamera;
    private LayerMask groundMask;

    public List<UnitMovement> ownedUnits = new List<UnitMovement>();

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

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 500f, groundMask))
            {
                UnitMovement unit = ownedUnits[0];
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
    }
}
