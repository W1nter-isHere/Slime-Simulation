using TMPro;
using UnityEngine;

public class PopulationText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Simulation simulation;

    private void Update()
    {
        text.text = "Population: " + simulation.Population;
    }
}