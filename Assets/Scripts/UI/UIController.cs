using TMPro;
using UnityEngine;
using Utils;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject OptionsPanel;
        
        [SerializeField] private TMP_InputField XDimension;
        [SerializeField] private TMP_InputField YDimension;
        [SerializeField] private TMP_InputField ColorsCount;

        [SerializeField] private GameObject StartSimulationButton;
        [SerializeField] private GameObject StopSimulationButton;
        
        [Space] 
        [SerializeField] private TMP_Text ScoreText;
        [SerializeField] private TMP_Text SimulationText;

        private void Start()
        {
            SetFields();
        }

        private void SetFields()
        {
            var data = DataUtil.Instance;
            XDimension.text = data.XDimension.ToString();
            YDimension.text = data.YDimension.ToString();
            ColorsCount.text = data.ColorCount.ToString();

            ScoreText.text = string.Empty;
            SimulationText.text = string.Empty;
            
            StartSimulationButton.SetActive(true);
            StopSimulationButton.SetActive(false);
        }

        public void OnReset()
        {
            var data = DataUtil.Instance;

            if (int.TryParse(XDimension.text, out int parsedValue))
            {
                if (parsedValue < 5)
                {
                    XDimension.text = "5";
                    parsedValue = 5;
                }

                data.XDimension = parsedValue;
            }
            
            if (int.TryParse(YDimension.text, out parsedValue))
            {
                if (parsedValue < 5)
                {
                    YDimension.text = "5";
                    parsedValue = 5;
                }

                data.YDimension = parsedValue;
            }
            
            if (int.TryParse(ColorsCount.text, out parsedValue))
            {
                if (parsedValue < 2)
                {
                    ColorsCount.text = "2";
                    parsedValue = 2;
                }

                data.ColorCount = parsedValue;
            }
            
            EventUtil.Instance.OnResetMap?.Invoke();
        }

        public void OnStartSimulation()
        {
            StartSimulationButton.SetActive(false);
            StopSimulationButton.SetActive(true);
        }
        
        public void OnStopSimulation()
        {
            StartSimulationButton.SetActive(true);
            StopSimulationButton.SetActive(false);
        }

        public void OnClosePanel()
        {
            OptionsPanel.SetActive(false);
        }
        
        public void OnOpenPanel()
        {
            OptionsPanel.SetActive(true);
        }
    }
}