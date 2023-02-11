using TMPro;
using UnityEngine;

namespace Initializers
{
    public class InitializeSensoryRange : MonoBehaviour
    {
        [SerializeField] private Simulation simulation;

        private void Start()
        {
            GetComponent<TMP_InputField>().text = simulation.sensoryRange.ToString();
        }
    }
}