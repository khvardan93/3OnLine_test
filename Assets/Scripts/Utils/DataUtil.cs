using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
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
            get => PlayerPrefs.GetInt(XDimensionKey, 5);
            set => PlayerPrefs.SetInt(XDimensionKey, value);
        }
        
        public int YDimension
        {
            get => PlayerPrefs.GetInt(YDimensionKey, 5);
            set => PlayerPrefs.SetInt(YDimensionKey, value);
        }
        
        public int ColorCount
        {
            get => PlayerPrefs.GetInt(ColorCountKey, 2);
            set => PlayerPrefs.SetInt(ColorCountKey, value);
        }

        public int GetBiggerDimension()
        {
            return XDimension > YDimension ? XDimension : YDimension;
        }
    }
}