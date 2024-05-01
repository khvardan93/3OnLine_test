using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Contains the gameplay data
    /// </summary>
    public class DataUtil
    {
        #region Singleton
        private static DataUtil _instance;

        public static DataUtil Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new();
                }

                return _instance;
            }
        }

        private DataUtil()
        {

        }
        #endregion

        private const string XDimensionKey = "x_dimension_size";
        private const string YDimensionKey = "y_dimension_size";
        private const string ColorCountKey = "color_count_size";

        public int XDimension
        {
            get => PlayerPrefs.GetInt(XDimensionKey, Configs.MinXDimension);
            set => PlayerPrefs.SetInt(XDimensionKey, value);
        }
        
        public int YDimension
        {
            get => PlayerPrefs.GetInt(YDimensionKey, Configs.MinYDimension);
            set => PlayerPrefs.SetInt(YDimensionKey, value);
        }
        
        public int ColorCount
        {
            get => PlayerPrefs.GetInt(ColorCountKey, Configs.MinColorCount);
            set => PlayerPrefs.SetInt(ColorCountKey, value);
        }

        public int GetBiggerDimension()
        {
            return XDimension > YDimension ? XDimension : YDimension;
        }

        private bool Lock;
        public bool AreInputsLocked
        {
            set
            {
                Lock = value;
                EventUtil.Instance.OnLockInputs?.Invoke(value);
            }
            get => Lock;
        }

        private int ScoreCached;
        public int Score
        {
            set
            {
                ScoreCached = value;
                EventUtil.Instance.OnUpdateScore?.Invoke(value);
            }
            get => ScoreCached;
        }

        private bool Simulation;
        public bool SimulationStatus
        {
            set
            {
                Simulation = value;
                EventUtil.Instance.OnSimulationStatus?.Invoke(value);
            }
            get => Simulation;
        }
    }
}