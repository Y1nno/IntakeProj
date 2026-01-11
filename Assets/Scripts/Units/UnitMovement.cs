using Fusion;
using UnityEngine;

public class UnitMovement : NetworkBehaviour
{
    public Vector3 TargetPos { get; private set; }
    public bool HasTarget { get; private set; }

    public float moveSpeed = 5f;
    public float arriveDistance = 0.1f;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            TargetPos = transform.position;
            HasTarget = false;
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
            return;
        }

        Vector3 dir = to / dist;
        float step = moveSpeed * Runner.DeltaTime;

        transform.position = pos + dir * Mathf.Min(step, dist);
    }
}
