using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utils
{
    /// <summary>
    /// The calss contains the logic which simulate a players oves
    /// </summary>
    public class SimulationUtil : MonoBehaviour
    {
        private const int StepCount = 1000000;

        private int StepsLeft;

        private Vector2Int[] Steps = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        private void Start()
        {
            EventUtil.Instance.OnSimulationStatus += OnChangeStatus;
        }

        private void OnDestroy()
        {
            EventUtil.Instance.OnSimulationStatus -= OnChangeStatus;
        }

        private void Update()
        {
            DataUtil data = DataUtil.Instance;

            if (!data.SimulationStatus || data.AreInputsLocked || StepsLeft == 0)
                return;

            Vector2Int pos1 = new Vector2Int(Random.Range(0, data.XDimension),
                Random.Range(0, data.YDimension));
            
            EventUtil.Instance.OnReplaceTileSim?.Invoke(pos1, GetSecondPosition(pos1));
            
            StepsLeft--;
            EventUtil.Instance.OnUpdateSimulationStep?.Invoke(StepsLeft);
        }

        private Vector2Int GetSecondPosition(Vector2Int pos1)
        {
            Vector2Int pos2 = pos1 + Steps[Random.Range(0, Steps.Length)];

            DataUtil data = DataUtil.Instance;

            if (pos2.x >= 0 && pos2.x < data.XDimension && pos2.y >= 0 && pos2.y < data.YDimension)
                return pos2;
            return GetSecondPosition(pos1);
        }

        private void OnChangeStatus(bool status)
        {
            if (status)
            {
                StepsLeft = StepCount;
                EventUtil.Instance.OnUpdateSimulationStep?.Invoke(StepsLeft);
            }
            else
            {
                
            }
        }
    }
}