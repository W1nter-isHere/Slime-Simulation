using TMPro;
using UnityEngine;

namespace Initializers
{
    public class InitializeMinLifespan : MonoBehaviour
    {
        [SerializeField] private Simulation simulation;

        private void Start()
        {
            GetComponent<TMP_InputField>().text = simulation.minimumLifetime.ToString("F2");
        }
    }
}