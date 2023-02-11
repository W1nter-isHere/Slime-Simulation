using UnityEngine;
using UnityEngine.EventSystems;

public class SlimeInput : MonoBehaviour
{
    [SerializeField] private Simulation simulation;
    [SerializeField] private GameObject prefab;

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

        if (Input.GetMouseButtonDown(1))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                var pos = _camera.ScreenToWorldPoint(mousePosition);
                pos.z = 0;
                Instantiate(prefab, pos, Quaternion.identity);
            }
            else
            {
                simulation.AddPoi(mousePosition);
            }
        }
    }
}