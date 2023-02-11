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
    public float speed = 10;
    public float minimumSteerRandomnessFactor = 0.75f;
    public float maximumSteerRandomnessFactor = 1.5f;
    public float minimumLifetime = 45;
    public float maximumLifetime = 90;
    public int numberOfSlimes = 360000;

    [NonSerialized] public bool DirtySteerFactor;
    [NonSerialized] public bool DirtyLifetime;

    public int Population => _slimes.Count(s => s.Valid == 1);

    [SerializeField] private ComputeShader simulationShader;
    [SerializeField] private ComputeShader postProcessingShader;
    [SerializeField] private ComputeShader clearScreenShader;
    [SerializeField] private ComputeShader pointRendererShader;
    [SerializeField, Min(1)] private int updateInterval;
    [SerializeField, Min(1)] private int poiVisualRadius;
    
    private int _tick;
    private RenderTexture _mainTexture;

    private List<Slime> _slimes;
    private List<int2> _poi;
    private List<float> _steerRandoms;
    private List<float> _lifetimeRandoms;

    private void Start()
    {
        _mainTexture = new RenderTexture(Screen.width, Screen.height, 24, GraphicsFormat.R16G16B16A16_SFloat);
        _mainTexture.enableRandomWrite = true;
        _mainTexture.Create();

        _slimes = new List<Slime>();
        _poi = new List<int2>();
        _steerRandoms = new List<float>();
        _lifetimeRandoms = new List<float>();

        sensoryRange = _mainTexture.width / 10;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        var deltaTime = Time.deltaTime;
        
        var poiCount = _poi.Count;
        ComputeBuffer poiBuffer = null;
        
        if (poiCount > 0)
        {
            poiBuffer = new ComputeBuffer(poiCount, Marshal.SizeOf<int2>());
            poiBuffer.SetData(_poi);
        }
        
        if (_tick % updateInterval == 0 && _slimes.Count > 0)
        {
            var slimesBuffer = new ComputeBuffer(_slimes.Count, Marshal.SizeOf<Slime>());

            var steerRandomsBuffer = new ComputeBuffer(_steerRandoms.Count, sizeof(float));
            var lifetimeRandomsBuffer = new ComputeBuffer(_lifetimeRandoms.Count, sizeof(float));

            slimesBuffer.SetData(_slimes);
            steerRandomsBuffer.SetData(_steerRandoms);
            lifetimeRandomsBuffer.SetData(_lifetimeRandoms);

            simulationShader.SetBuffer(0, "poi", poiBuffer);
            simulationShader.SetBuffer(0, "slimes", slimesBuffer);
            simulationShader.SetBuffer(0, "steer_randoms", steerRandomsBuffer);
            simulationShader.SetBuffer(0, "lifetime_randoms", lifetimeRandomsBuffer);
            simulationShader.SetInt("poi_count", poiCount);
            simulationShader.SetTexture(0, "result", _mainTexture);
            simulationShader.SetInt("width", _mainTexture.width);
            simulationShader.SetInt("height", _mainTexture.height);
            simulationShader.SetInt("sensory_range", sensoryRange);
            simulationShader.SetFloat("speed", speed);
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
        else if (poiCount <= 0)
        {
            clearScreenShader.SetTexture(0, "result", _mainTexture);
            clearScreenShader.Dispatch(0, Mathf.CeilToInt(_mainTexture.width / 8f),
                Mathf.CeilToInt(_mainTexture.height / 8f), 1);
        }

        if (poiCount > 0)
        {
            pointRendererShader.SetBuffer(0, "points", poiBuffer);
            pointRendererShader.SetTexture(0, "result", _mainTexture);
            pointRendererShader.SetInt("radius", poiVisualRadius);
            pointRendererShader.Dispatch(0, poiCount, 1, 1);
            poiBuffer?.Dispose();
        }

        UpdateRandoms();

        _tick++;
        Graphics.Blit(_mainTexture, dest);
    }

    public void AddSlime(Vector2 screenSpace)
    {
        SpawnSlime(new Slime(new int2((int)(screenSpace.x / Screen.width * _mainTexture.width), (int)(screenSpace.y / Screen.height * _mainTexture.height)), Random.Range(0, 361)));
    }

    public void AddPoi(Vector2 screenSpace)
    {
        var p = new int2((int)(screenSpace.x / Screen.width * _mainTexture.width), (int)(screenSpace.y / Screen.height * _mainTexture.height));
        if (!_poi.Contains(p))
        {
            _poi.Add(p);
        }
    }
    
    public void Reset()
    {
        _slimes.Clear();
        _steerRandoms.Clear();
        _lifetimeRandoms.Clear();

        var increment = 360f / numberOfSlimes;
        
        for (var a = 0f; a <= 360; a += increment)
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