using TMPro;
using UnityEngine;

namespace Initializers
{
    public class InitializeMinSteerFactor : MonoBehaviour
    {
        [SerializeField] private Simulation simulation;

        private void Start()
        {
            GetComponent<TMP_InputField>().text = simulation.minimumSteerRandomnessFactor.ToString("F2");
        }
    }
}