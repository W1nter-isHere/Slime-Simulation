using UnityEngine;
using UnityEngine.EventSystems;

public class SlimeInput : MonoBehaviour
{
    [SerializeField] private Simulation simulation;

    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        var mousePosition = Input.mousePosition;
        
        if (Input.GetMouseButton(0))
        {
            simulation.AddSlime(mousePosition);
        }

        if (!Input.GetMouseButtonDown(1)) return;
        
        var pos = _camera.ScreenToWorldPoint(mousePosition);
        pos.z = 0;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            simulation.AddSpawner(pos);
        }
        else
        {
            simulation.AddPoi(mousePosition, pos);
        }
    }
}