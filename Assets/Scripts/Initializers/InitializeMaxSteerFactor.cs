using TMPro;
using UnityEngine;

namespace Initializers
{
    public class InitializeMaxSteerFactor : MonoBehaviour
    {
        [SerializeField] private Simulation simulation;

        private void Start()
        {
            GetComponent<TMP_InputField>().text = simulation.maximumSteerRandomnessFactor.ToString("F2");
        }
    }
}