using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Screen = UnityEngine.Device.Screen;

public class Simulation : MonoBehaviour
{
    [SerializeField] private ComputeShader simulationShader;
    [SerializeField] private ComputeShader postProcessingShader;
    [SerializeField, Min(1)] private int updateInterval;

    private int _tick;
    private RenderTexture _mainTexture;
    private Slime[] _slimes;
    
    private void Start()
    {
        _mainTexture = new RenderTexture(Screen.width, Screen.height, 24, GraphicsFormat.R16G16B16A16_SFloat);
        _mainTexture.enableRandomWrite = true;
        _mainTexture.Create();

        _slimes = new Slime[36000];
        // _slimes[0] = new Slime(new int2(128, 128), Mathf.Deg2Rad * 25);
        // _slimes[1] = new Slime(new int2(128, 128), Mathf.Deg2Rad * 45);

        var i = 0;
        for (var a = 0f; a < 360; a += 0.01f)
        {
            _slimes[i] = new Slime(new int2(_mainTexture.width / 2, _mainTexture.height / 2), Mathf.Deg2Rad * a);
            i++;
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        var deltaTime = Time.deltaTime;

        if (_tick % updateInterval == 0)
        {
            var slimesBuffer = new ComputeBuffer(_slimes.Length, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Slime)));
            slimesBuffer.SetData(_slimes);
            simulationShader.SetBuffer(0, "slimes", slimesBuffer);
            simulationShader.SetTexture(0, "result", _mainTexture);
            simulationShader.SetInt("width", _mainTexture.width);
            simulationShader.SetInt("height", _mainTexture.height);
            simulationShader.SetInt("sensory_range", _mainTexture.width / 10);
            simulationShader.SetFloat("delta_time", deltaTime);
            simulationShader.Dispatch(0, Mathf.CeilToInt(_slimes.Length / 16f), 1, 1);
            slimesBuffer.GetData(_slimes);
        
            postProcessingShader.SetTexture(0, "result", _mainTexture);
            postProcessingShader.SetFloat("delta_time", deltaTime);
            postProcessingShader.SetInt("width", _mainTexture.width);
            postProcessingShader.SetInt("height", _mainTexture.height);
            postProcessingShader.Dispatch(0, Mathf.CeilToInt(_mainTexture.width / 8f), Mathf.CeilToInt(_mainTexture.height / 8f), 1);
            slimesBuffer.Dispose();
        }
        
        _tick++;
        Graphics.Blit(_mainTexture, dest);
    }
}