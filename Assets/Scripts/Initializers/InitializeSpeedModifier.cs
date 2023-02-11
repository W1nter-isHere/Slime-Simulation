using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Initializers
{
    public class InitializeSpeedModifier : MonoBehaviour
    {
        [SerializeField] private Simulation simulation;
        [SerializeField] private TextMeshProUGUI speedModiferText;

        private void Start()
        {
            GetComponent<Slider>().value = simulation.speedModifier;
            speedModiferText.text = simulation.speedModifier.ToString("F1");
        }
    }
}