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
        Debug.Log("Select action found: " + (selectAction != null));
        Debug.Log("Camera assigned: " + (mainCamera != null));
    }
    
    public override void Spawned()
    {
        if (selectAction == null)
        {
            selectAction = InputSystem.actions.FindAction("Select");
        }
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
                for (int i = ownedUnits.Count - 1; i >= 0; i--)
                {
                    UnitMovement unit = ownedUnits[i];
                    if (unit == null || unit.Object == null)
                    {
                        ownedUnits.RemoveAt(i);
                        continue;
                    }

                    if (unit.Object.InputAuthority != Object.InputAuthority) continue;
                    
                    RPC_RequestMove(unit.Object, FromRaycastToVector3(hit, unit));
                }
            }
            else
            {
                Debug.Log("No ground hit detected.");
            }
        }
    }

    private Vector3 FromRaycastToVector3(RaycastHit hit, UnitMovement unit)
    {
        return new Vector3(hit.point.x, unit.transform.position.y, hit.point.z);
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
