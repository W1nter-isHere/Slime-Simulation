using System;
using UnityEngine;

public class SlimeEmitter : MonoBehaviour
{
    [SerializeField] private Simulation simulation;
    [SerializeField] private float spawnRate;

    private float _timer;
    private Vector2 _position;

    private void Start()
    {
        _position = Camera.main.WorldToScreenPoint(transform.position);
    }

    private void Update()
    {
        if (_timer >= spawnRate)
        {
            simulation.AddSlime(_position);
            _timer = 0;
        }

        _timer += Time.deltaTime;
    }
}