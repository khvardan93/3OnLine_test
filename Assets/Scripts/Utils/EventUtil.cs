using System;
using UnityEngine;

namespace Utils
{
    public class EventUtil
    {
        #region Singleton
        private static EventUtil _instance;

        public static EventUtil Instance
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

        private EventUtil()
        {

        }
        #endregion

        public Action OnResetMap;

        public Action<Vector3Int, Vector3Int> OnReplaceTile;

        public Action<int> OnUpdateScore;
        
        public Action<int> OnUpdateSimulationStep;

        public Action<bool> OnSimulationStatus;

        public Action<bool> OnLockInputs;
    }
}