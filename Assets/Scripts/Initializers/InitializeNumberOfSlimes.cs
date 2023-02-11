using TMPro;
using UnityEngine;

namespace Initializers
{
    public class InitializeNumberOfSlimes : MonoBehaviour
    {
        [SerializeField] private Simulation simulation;

        private void Start()
        {
            GetComponent<TMP_InputField>().text = simulation.numberOfSlimes.ToString();
        }
    }
}