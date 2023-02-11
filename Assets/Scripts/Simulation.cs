using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class Simulation : MonoBehaviour
{
    [SerializeField] private ComputeShader simulationShader;
    [SerializeField] private ComputeShader fadeoutShader;
    
    private RenderTexture _mainTexture;
    private List<Slime> _slimes;

    private void Start()
    {
        _mainTexture = new RenderTexture(256, 256, 24, GraphicsFormat.R16G16B16A16_SFloat);
        _mainTexture.enableRandomWrite = true;
        _mainTexture.Create();

        _slimes = new List<Slime>();
        _slimes.Add(new Slime {Angle = Mathf.Deg2Rad * 45, Position = new int2(128, 128), Speed = 10});
        _slimes.Add(new Slime {Angle = Mathf.Deg2Rad * 25, Position = new int2(128, 128), Speed = 10});
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        var buffer = new ComputeBuffer(_slimes.Count, sizeof(int) * 2 + sizeof(float) * 2);
        buffer.SetData(_slimes);
        
        simulationShader.SetBuffer(0, "slimes", buffer);
        simulationShader.SetTexture(0, "result", _mainTexture);
        simulationShader.SetInt("width", _mainTexture.width);
        simulationShader.SetInt("height", _mainTexture.height);
        simulationShader.SetInt("sensory_range", _mainTexture.width / 10);
        simulationShader.Dispatch(0, Mathf.CeilToInt(_slimes.Count / 16f), 1, 1);

        var newSlimes = new Slime[_slimes.Count];
        buffer.GetData(newSlimes);
        _slimes.Clear();
        _slimes.AddRange(newSlimes);
        
        fadeoutShader.SetTexture(0, "result", _mainTexture);
        fadeoutShader.Dispatch(0, _mainTexture.width / 8, _mainTexture.height / 8, 1);

        buffer.Dispose();
        Graphics.Blit(_mainTexture, dest);
    }
}