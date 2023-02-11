using TMPro;
using UnityEngine;

public class ParameterUpdater : MonoBehaviour
{
    [SerializeField] private Simulation simulation;
    [SerializeField] private TextMeshProUGUI speedModiferText;
    
    public void SensoryRangeChanged(string value)
    {
        if (int.TryParse(value, out var v))
        {
            simulation.sensoryRange = v; 
        }
    }
    
    public void SpeedModifierChanged(float value)
    {
        simulation.speedModifier = value;
        speedModiferText.text = value.ToString("F1");
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

    public void Reset()
    {
        simulation.Reset();
    }
}