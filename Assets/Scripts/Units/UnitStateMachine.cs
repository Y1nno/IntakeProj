using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UnitStateName
{
    Idle,
    Moving,
    Attacking
}

public class UnitStateMachine : MonoBehaviour
{
    private Dictionary<UnitStateName, UnitState> states = new Dictionary<UnitStateName, UnitState>();
    
    private UnitState currentState;
    
    void Start()
    {
        states.Add(UnitStateName.Idle, new IdleState());
        states.Add(UnitStateName.Moving, new MovingState());
        states.Add(UnitStateName.Attacking, new AttackingState());
        currentState = states[UnitStateName.Idle];
    }

    // Update is called once per frame
    void Update()
    {
        currentState.Tick();
    }
    public void ChangeState(UnitStateName newStateName)
    {
        currentState.OnExit();
        currentState = states[newStateName];
        currentState.OnEnter();
    }

    public UnitState GetCurrentState()
    {
        return currentState;
    }
}

public abstract class UnitState
{
    public abstract void OnEnter();
    public abstract void Tick();
    public abstract void OnExit();
}

public class IdleState : UnitState
{
    public override void OnEnter()
    {
        Debug.Log("Entering Idle State");
    }

    public override void Tick()
    {
        // Idle behavior
    }

    public override void OnExit()
    {
        Debug.Log("Exiting Idle State");
    }
}

public class MovingState : UnitState
{
    public override void OnEnter()
    {
        Debug.Log("Entering Moving State");
    }

    public override void Tick()
    {
        // Moving behavior
    }

    public override void OnExit()
    {
        Debug.Log("Exiting Moving State");
    }
}

public class AttackingState : UnitState
{
    public override void OnEnter()
    {
        Debug.Log("Entering Attacking State");
    }

    public override void Tick()
    {
        // Attacking behavior
    }

    public override void OnExit()
    {
        Debug.Log("Exiting Attacking State");
    }
}
