using Fusion;
using UnityEngine;

public class UnitMovement : NetworkBehaviour
{
    public Vector3 TargetPos { get; private set; }
    public bool HasTarget;

    public float moveSpeed = 5f;
    public float arriveDistance = 1f;

    public float movingThisStep = 0f;

    private Rigidbody cachedRigidbody;

    private FactionManager factionManager;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            TargetPos = transform.position;
            HasTarget = false;
        }

        var localPlayer = NetworkPlayer.Local;
        if (localPlayer != null && Object.InputAuthority == localPlayer.Object.InputAuthority)
        {
            localPlayer.RegisterOwnedUnit(this);
        }

        if (cachedRigidbody == null)
        {
            cachedRigidbody = GetComponent<Rigidbody>();
        }
        factionManager = GetComponentInChildren<FactionManager>();
        factionManager.SetFaction(Object.InputAuthority);

    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        var localPlayer = NetworkPlayer.Local;
        if (localPlayer != null && Object.InputAuthority == localPlayer.Object.InputAuthority)
        {
            localPlayer.UnregisterOwnedUnit(this);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestMove(Vector3 target)
    {
        TargetPos = target;
        HasTarget = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        if (!HasTarget) return;

        Vector3 pos = transform.position;
        Vector3 to = TargetPos - pos;

        float dist = to.magnitude;
        if (dist <= arriveDistance)
        {
            transform.position = TargetPos;
            HasTarget = false;
            movingThisStep = 0f;
            if (cachedRigidbody != null)
            {
                cachedRigidbody.linearVelocity = Vector3.zero;
                cachedRigidbody.angularVelocity = Vector3.zero;
            }
            return;
        }

        Vector3 dir = to / dist;
        float step = moveSpeed * Runner.DeltaTime;

        movingThisStep = step;

        transform.position = pos + dir * Mathf.Min(step, dist);
    }
}
