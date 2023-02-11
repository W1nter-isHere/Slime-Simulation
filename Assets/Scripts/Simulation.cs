using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Random = UnityEngine.Random;
using Screen = UnityEngine.Device.Screen;

public class Simulation : MonoBehaviour
{
    public int sensoryRange = 20;
    public float speedModifier = 1;
    public float minimumSteerRandomnessFactor = 0.75f;
    public float maximumSteerRandomnessFactor = 1.5f;
    public float minimumLifetime = 45;
    public float maximumLifetime = 90;

    [NonSerialized] public bool DirtySteerFactor;
    [NonSerialized] public bool DirtyLifetime;

    public int Population => _slimes.Count(s => s.Valid == 1);

    [SerializeField] private ComputeShader simulationShader;
    [SerializeField] private ComputeShader postProcessingShader;
    [SerializeField] private ComputeShader clearScreenShader;
    [SerializeField, Min(1)] private int updateInterval;

    private int _tick;
    private RenderTexture _mainTexture;

    private List<Slime> _slimes;
    private List<float> _steerRandoms;
    private List<float> _lifetimeRandoms;

    private void Start()
    {
        _mainTexture = new RenderTexture(Screen.width, Screen.height, 24, GraphicsFormat.R16G16B16A16_SFloat);
        _mainTexture.enableRandomWrite = true;
        _mainTexture.Create();

        _slimes = new List<Slime>();
        _steerRandoms = new List<float>();
        _lifetimeRandoms = new List<float>();

        // for (var a = 0f; a <= 360; a += 0.01f)
        // {
        //     SpawnSlime(new Slime(new int2(_mainTexture.width / 2, _mainTexture.height / 2), a));
        // }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        var deltaTime = Time.deltaTime;

        if (_tick % updateInterval == 0 && _slimes.Count > 0)
        {
            var slimesBuffer = new ComputeBuffer(_slimes.Count, Marshal.SizeOf<Slime>());
            var steerRandomsBuffer = new ComputeBuffer(_steerRandoms.Count, Marshal.SizeOf<float>());
            var lifetimeRandomsBuffer = new ComputeBuffer(_lifetimeRandoms.Count, Marshal.SizeOf<float>());

            slimesBuffer.SetData(_slimes);
            steerRandomsBuffer.SetData(_steerRandoms);
            lifetimeRandomsBuffer.SetData(_lifetimeRandoms);

            simulationShader.SetBuffer(0, "slimes", slimesBuffer);
            simulationShader.SetBuffer(0, "steer_randoms", steerRandomsBuffer);
            simulationShader.SetBuffer(0, "lifetime_randoms", lifetimeRandomsBuffer);
            simulationShader.SetTexture(0, "result", _mainTexture);
            simulationShader.SetInt("width", _mainTexture.width);
            simulationShader.SetInt("height", _mainTexture.height);
            simulationShader.SetInt("sensory_range", sensoryRange);
            simulationShader.SetFloat("speed_modifier", speedModifier);
            simulationShader.SetFloat("delta_time", deltaTime);
            simulationShader.Dispatch(0, Mathf.CeilToInt(_slimes.Count / 16f), 1, 1);

            var startingCount = _slimes.Count;
            var arr = new Slime[startingCount];
            slimesBuffer.GetData(arr);
            _slimes = arr.Where(s => s.Valid == 1).ToList();
            _steerRandoms.RemoveRange(0, startingCount - _slimes.Count);
            _lifetimeRandoms.RemoveRange(0, startingCount - _slimes.Count);

            postProcessingShader.SetTexture(0, "result", _mainTexture);
            postProcessingShader.SetFloat("delta_time", deltaTime);
            postProcessingShader.SetInt("width", _mainTexture.width);
            postProcessingShader.SetInt("height", _mainTexture.height);
            postProcessingShader.Dispatch(0, Mathf.CeilToInt(_mainTexture.width / 8f),
                Mathf.CeilToInt(_mainTexture.height / 8f), 1);

            slimesBuffer.Dispose();
            steerRandomsBuffer.Dispose();
            lifetimeRandomsBuffer.Dispose();
        }
        else
        {
            clearScreenShader.SetTexture(0, "result", _mainTexture);
            clearScreenShader.Dispatch(0, Mathf.CeilToInt(_mainTexture.width / 8f),
                Mathf.CeilToInt(_mainTexture.height / 8f), 1);
        }

        UpdateRandoms();

        _tick++;
        Graphics.Blit(_mainTexture, dest);
    }

    public void AddSlime(Vector2 screenSpace)
    {
        SpawnSlime(new Slime(new int2((int)screenSpace.x, (int)screenSpace.y), Random.Range(0, 361)));
    }

    public void Reset()
    {
        _slimes.Clear();
        _steerRandoms.Clear();
        _lifetimeRandoms.Clear();
        
        for (var a = 0f; a <= 360; a += 0.01f)
        {
            SpawnSlime(new Slime(new int2(_mainTexture.width / 2, _mainTexture.height / 2), a));
        }
    }

    private void UpdateRandoms()
    {
        if (DirtySteerFactor)
        {
            for (var i = 0; i < _steerRandoms.Count; i++)
            {
                _steerRandoms[i] = Random.Range(minimumSteerRandomnessFactor, maximumSteerRandomnessFactor);
            }
        }

        if (!DirtyLifetime) return;

        for (var i = 0; i < _lifetimeRandoms.Count; i++)
        {
            _lifetimeRandoms[i] = Random.Range(minimumLifetime, maximumLifetime + 1);
        }
    }

    private void SpawnSlime(Slime slime)
    {
        _slimes.Add(slime);
        _steerRandoms.Add(Random.Range(minimumSteerRandomnessFactor, maximumSteerRandomnessFactor));
        _lifetimeRandoms.Add(Random.Range(minimumLifetime, maximumLifetime + 1));
    }
}