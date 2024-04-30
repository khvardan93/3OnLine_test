using UnityEngine;
using UnityEngine.Tilemaps;

namespace Utils
{
    [RequireComponent(typeof(Tilemap))]
    public class InputUtil : MonoBehaviour
    {
        private Camera MainCamera;
        private Tilemap Tilemap;
        private LayerMask LayerMask;

        private bool IsMouseDown;

        private Vector3Int ClickedTile;
        
        private void Start()
        {
            MainCamera = Camera.main;
            LayerMask = 1 << LayerMask.NameToLayer("Tilemap");
            Tilemap = GetComponent<Tilemap>();
        }

        private void Update()
        {
            if(DataUtil.Instance.AreInputsLocked)
                return;
            
            if (Input.GetMouseButtonDown(0))
            {
                OnButtonDown();
            }
            
            if (IsMouseDown)
            {
                OnButtonMove();
            }
            
            if (IsMouseDown && Input.GetMouseButtonUp(0))
            {
                OnButtonUp();
            }
        }

        private void OnButtonDown()
        {            
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, int.MaxValue, LayerMask);

            if (hit.collider != null)
            {
                ClickedTile = Tilemap.WorldToCell(hit.point);

                IsMouseDown = true;
            }
        }

        private void OnButtonMove()
        {
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, int.MaxValue, LayerMask);

            if (hit.collider != null)
            {
                Vector3Int gridPosition = Tilemap.WorldToCell(hit.point);

                if (Vector3Int.Distance(gridPosition, ClickedTile) == 1)
                {
                    IsMouseDown = false;
                    EventUtil.Instance.OnReplaceTile(gridPosition, ClickedTile);
                }
            }
        }

        private void OnButtonUp()
        {
            IsMouseDown = false;
        }
    }
}
