using UnityEngine;
using System.Collections.Generic;

public class BoxSelector : MonoBehaviour
{
    public List<UnitMovement> selectedUnits = new List<UnitMovement>();
    
    void OnTriggerEnter(Collider other)
    {
        UnitMovement unit = other.GetComponentInParent<UnitMovement>();
        if (unit != null)
        {
            selectedUnits.Add(unit);
        }
    }

    void OnTriggerExit(Collider other)
    {
        UnitMovement unit = other.GetComponentInParent<UnitMovement>();
        if (unit != null)
        {
            selectedUnits.Remove(unit);
        }
    }
}
