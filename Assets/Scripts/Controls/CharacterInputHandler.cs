using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputHandler: MonoBehaviour
{
    private Vector2 movementInput = Vector2.zero;
    private InputAction inputAction;
    private void Awake()
    {
        inputAction = InputSystem.actions.FindAction("Player/Move");  
    }

    void Update()
    {
        movementInput = inputAction.ReadValue<Vector2>();
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();
        networkInputData.movementInput = movementInput;
        return networkInputData;
    }
}
