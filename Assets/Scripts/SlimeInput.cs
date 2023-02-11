using UnityEngine;

public class SlimeInput : MonoBehaviour
{
    [SerializeField] private Simulation simulation;
    
    private void Update()
    {
        if (!Input.GetMouseButton(0)) return;
        simulation.AddSlime(Input.mousePosition);
    }
}