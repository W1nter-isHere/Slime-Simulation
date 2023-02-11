using TMPro;
using UnityEngine;

namespace Initializers
{
    public class InitializeSlimeColor : MonoBehaviour
    {
        [SerializeField] private Simulation simulation;
        [SerializeField] private TMP_InputField r;
        [SerializeField] private TMP_InputField g;
        [SerializeField] private TMP_InputField b;
        
        private void Start()
        {
            r.text = simulation.slimeColor.r.ToString();
            g.text = simulation.slimeColor.g.ToString();
            b.text = simulation.slimeColor.b.ToString();
        }
    }
}