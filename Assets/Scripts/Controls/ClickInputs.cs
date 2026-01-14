using Fusion;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SimpleClickToMove : NetworkBehaviour
{
    public Camera mainCamera;
    public LayerMask groundMask;
    public LayerMask unitMask;

    public GameObject boxSelector;

    private InputAction selectAction;
    private InputAction targetAction;
    private InputAction targetingAction;

    public List<UnitMovement> ownedUnits = new List<UnitMovement>();

    public List<UnitMovement> selectedUnits = new List<UnitMovement>();
    private Ray lastClickRay;
    private bool hasLastClickRay;
    private bool lastClickRayHit;
    private Vector3 lastClickHitPoint;
    private const float MaxClickRayDistance = 500f;
    private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

    private bool isTargeting = false;

    private BoxSelector boxSelectorInstance;

    private Vector3 startBoxHit;

    private void Start()
    {
        selectAction = InputSystem.actions.FindAction("Select");
        targetAction = InputSystem.actions.FindAction("Target");
        targetingAction = InputSystem.actions.FindAction("Targeting");
    }
    
    public override void Spawned()
    {
        if (selectAction == null)
        {
            selectAction = InputSystem.actions.FindAction("Select");
            targetAction = InputSystem.actions.FindAction("Target");
            targetingAction = InputSystem.actions.FindAction("Targeting");
        }

    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;
        if (ownedUnits.Count == 0) return;

        if (selectAction.WasPerformedThisFrame())
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            lastClickRay = ray;
            hasLastClickRay = true;
            RaycastHit hit;
            bool hasHit;
            var physicsScene = Runner.GetPhysicsScene();
            if (physicsScene.IsValid())
            {
                hasHit = physicsScene.Raycast(ray.origin, ray.direction, out hit, 500f, groundMask, triggerInteraction);
            }
            else
            {
                hasHit = Physics.Raycast(ray, out hit, 500f, groundMask, triggerInteraction);
            }

            if (hasHit)
            {
                lastClickRayHit = true;
                lastClickHitPoint = hit.point;
                for (int i = selectedUnits.Count - 1; i >= 0; i--)
                {
                    UnitMovement unit = selectedUnits[i];
                    if (unit == null || unit.Object == null)
                    {
                        selectedUnits.RemoveAt(i);
                        continue;
                    }

                    if (unit.Object.InputAuthority != Object.InputAuthority) continue;
                    
                    RPC_RequestMove(unit.Object, FromRaycastToVector3(hit, unit));
                }
            }
            else
            {
                lastClickRayHit = false;
                Debug.Log("No ground hit detected.");
            }
        }
    }

    void Update()
    {
        if (targetAction.WasPerformedThisFrame())
        {
            //Debug.Log("Target action performed");
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            lastClickRay = ray;
            hasLastClickRay = true;
            RaycastHit hit;
            bool hasHit;
            var physicsScene = Runner.GetPhysicsScene();
            
            if (physicsScene.IsValid())
            {
                hasHit = physicsScene.Raycast(ray.origin, ray.direction, out hit, 500f, unitMask, triggerInteraction);
            }
            else
            {
                hasHit = Physics.Raycast(ray, out hit, 500f, unitMask, triggerInteraction);
            }

            if (hasHit)
            {
                lastClickRayHit = true;
                lastClickHitPoint = hit.point;
                UnitMovement unit = hit.collider.gameObject.GetComponentInParent<UnitMovement>();
                Debug.Log(unit != null);
                if (unit != null)
                {
                    selectedUnits.Clear();
                    selectedUnits.Add(unit);
                }
            }
            else
            {
                selectedUnits.Clear();
                lastClickRayHit = false;
                //Debug.Log("No unit hit detected.");
            }
        }

        if (targetingAction.WasPerformedThisFrame())
        {
            Debug.Log("Targeting action performed");
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            lastClickRay = ray;
            hasLastClickRay = true;
            RaycastHit hit;
            bool hasHit;
            var physicsScene = Runner.GetPhysicsScene();
            if (physicsScene.IsValid())
            {
                hasHit = physicsScene.Raycast(ray.origin, ray.direction, out hit, 500f, groundMask, triggerInteraction);
            }
            else
            {
                hasHit = Physics.Raycast(ray, out hit, 500f, groundMask, triggerInteraction);
            }
            if (hasHit)
            {
                lastClickRayHit = true;
                lastClickHitPoint = hit.point;
                isTargeting = true;

            }
            else
            {
                lastClickRayHit = false;
                //Debug.Log("No unit hit detected.");
                isTargeting = false;
            }
        }

        if (targetingAction.WasReleasedThisFrame())
        {
            isTargeting = false;
            DestroyBoxSelector();
        }

        if (isTargeting)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            lastClickRay = ray;
            hasLastClickRay = true;
            RaycastHit hit;
            bool hasHit;
            var physicsScene = Runner.GetPhysicsScene();
            if (physicsScene.IsValid())
            {
                hasHit = physicsScene.Raycast(ray.origin, ray.direction, out hit, 500f, groundMask, triggerInteraction);
            }
            else
            {
                hasHit = Physics.Raycast(ray, out hit, 500f, groundMask, triggerInteraction);
            }
            if (hasHit)
            {
                lastClickRayHit = true;
                lastClickHitPoint = hit.point;
                if (boxSelectorInstance == null)
                {
                    boxSelectorInstance = CreateBoxSelector(hit.point);
                }
                else
                {
                    float minX = Mathf.Min(startBoxHit.x, hit.point.x);
                    float maxX = Mathf.Max(startBoxHit.x, hit.point.x);
                    float minZ = Mathf.Min(startBoxHit.z, hit.point.z);
                    float maxZ = Mathf.Max(startBoxHit.z, hit.point.z);
                    boxSelectorInstance.transform.localScale = new Vector3(Mathf.Max(0.01f, maxX - minX), 1f, Mathf.Max(0.01f, maxZ - minZ));
                    boxSelectorInstance.gameObject.transform.parent.position = new Vector3((minX + maxX) * 0.5f, startBoxHit.y, (minZ + maxZ) * 0.5f);;
                }
            }   
            else
            {
                lastClickRayHit = false;
                DestroyBoxSelector();
            }
        }  
    }

    private BoxSelector CreateBoxSelector(Vector3 position)
    {
        if (boxSelectorInstance != null)
        {
            Destroy(boxSelectorInstance.gameObject);
            boxSelectorInstance = null;
        }

        startBoxHit = position;
        GameObject boxSelectorObj = Instantiate(boxSelector);
        BoxSelector selector = boxSelectorObj.transform.GetChild(0).GetComponent<BoxSelector>();
        if (position != null)
        {
            boxSelectorObj.transform.position = position;
            return selector;
        }
        boxSelectorObj.transform.position = Vector3.zero;
        return selector;
        
    }

    private void DestroyBoxSelector()
    {
        if (boxSelectorInstance != null)
        {
            foreach (var unit in boxSelectorInstance.selectedUnits)
            {
                if (!selectedUnits.Contains(unit))
                {
                    selectedUnits.Add(unit);
                }
            }
            Destroy(boxSelectorInstance.gameObject);
            boxSelectorInstance = null;
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

    private void OnDrawGizmos()
    {
        if (!hasLastClickRay) return;

        Gizmos.color = lastClickRayHit ? Color.green : Color.red;
        Vector3 start = lastClickRay.origin;
        Vector3 end = lastClickRayHit
            ? lastClickHitPoint
            : lastClickRay.origin + lastClickRay.direction * MaxClickRayDistance;
        Gizmos.DrawLine(start, end);

        if (lastClickRayHit)
        {
            Gizmos.DrawSphere(lastClickHitPoint, 0.2f);
        }
    }
}
