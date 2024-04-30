using UnityEngine;
using Utils;

namespace Map
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private Camera Camera;
        
        private void Start()
        {
            Camera = GetComponent<Camera>();
            
            SetSize();

            EventUtil.Instance.OnResetMap += SetSize;
        }

        private void OnDestroy()
        {
            EventUtil.Instance.OnResetMap -= SetSize;
        }

        private void SetSize()
        {
            Camera.orthographicSize = DataUtil.Instance.GetBiggerDimension() * 0.5f + 2f;
        }
    }
}