using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;

public class PlayerMovementHandler : NetworkBehaviour
{
    NetworkCharacterControllerPrototypeCustom characterController;

    private void Awake()
    {
        characterController = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData NewtworkInputData))
        {
            //move
            Vector3 moveDirection = transform.forward * NewtworkInputData.movementInput.y + transform.right * NewtworkInputData.movementInput.x;
            moveDirection.Normalize();
            characterController.Move(moveDirection);
        }
        
    }
}
