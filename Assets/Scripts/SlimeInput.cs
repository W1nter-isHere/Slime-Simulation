using UnityEngine;
using UnityEngine.EventSystems;

public class SlimeInput : MonoBehaviour
{
    [SerializeField] private Simulation simulation;
    
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        var mousePosition = Input.mousePosition;
        
        if (Input.GetMouseButton(0))
        {
            simulation.AddSlime(mousePosition);
        }

        if (Input.GetMouseButtonDown(1))
        {
            simulation.AddPoi(mousePosition);
        }
    }
}