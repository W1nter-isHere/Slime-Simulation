using TMPro;
using UnityEngine;

public class ParameterUpdater : MonoBehaviour
{
    [SerializeField] private Simulation simulation;

    [SerializeField] private TMP_InputField redChannel;
    [SerializeField] private TMP_InputField greenChannel;
    [SerializeField] private TMP_InputField blueChannel;
    
    public void SensoryRangeChanged(string value)
    {
        if (int.TryParse(value, out var v))
        {
            simulation.sensoryRange = v; 
        }
    }

    public void NumberOfSlimesChanged(string value)
    {
        if (int.TryParse(value, out var v))
        {
            simulation.numberOfSlimes = v; 
        }
    }
    
    public void SpeedChanged(string value)
    {
        if (!float.TryParse(value, out var v)) return;
        simulation.speed = v;
    }

    public void MinSteerChanged(string value)
    {
        if (!float.TryParse(value, out var v)) return;
        simulation.minimumSteerRandomnessFactor = v;
        simulation.DirtySteerFactor = true;
    }

    public void MaxSteerChanged(string value)
    {
        if (!float.TryParse(value, out var v)) return;
        simulation.maximumSteerRandomnessFactor = v;
        simulation.DirtySteerFactor = true;
    }
    
    public void MinLifeSpanChanged(string value)
    {
        if (!float.TryParse(value, out var v)) return;
        simulation.minimumLifetime = v;
        simulation.DirtyLifetime = true;
    }

    public void MaxLifeSpanChanged(string value)
    {
        if (!float.TryParse(value, out var v)) return;
        simulation.maximumLifetime = v;
        simulation.DirtyLifetime = true;
    }

    public void RValueChanged(string value)
    {
        if (byte.TryParse(value, out var v))
        {
            if (v == 0)
            {
                v = 1;
            }
            redChannel.SetTextWithoutNotify(v.ToString());
            
            var color = simulation.slimeColor;
            color.r = v;
            simulation.slimeColor = color;
        }
    }
    
    public void GValueChanged(string value)
    {
        if (byte.TryParse(value, out var v))
        {
            if (v == 0)
            {
                v = 1;
            }
            greenChannel.SetTextWithoutNotify(v.ToString());

            var color = simulation.slimeColor;
            color.g = v;
            simulation.slimeColor = color;
        }
    }
    
    public void BValueChanged(string value)
    {
        if (byte.TryParse(value, out var v))
        {
            if (v == 0)
            {
                v = 1;
            }
            blueChannel.SetTextWithoutNotify(v.ToString());

            var color = simulation.slimeColor;
            color.b = v;
            simulation.slimeColor = color;
        }
    }

    public void Reset()
    {
        simulation.Reset();
    }

    public void Add()
    {
        simulation.Add();
    }
}