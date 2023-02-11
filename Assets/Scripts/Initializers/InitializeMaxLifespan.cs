using TMPro;
using UnityEngine;

namespace Initializers
{
    public class InitializeMaxLifespan : MonoBehaviour
    {
        [SerializeField] private Simulation simulation;

        private void Start()
        {
            GetComponent<TMP_InputField>().text = simulation.maximumLifetime.ToString("F2");
        }
    }
}