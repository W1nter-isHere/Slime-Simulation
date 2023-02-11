using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Initializers
{
    public class InitializeSpeed : MonoBehaviour
    {
        [SerializeField] private Simulation simulation;

        private void Start()
        {
            GetComponent<TMP_InputField>().text = simulation.speed.ToString("F1");
        }
    }
}