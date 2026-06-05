using UnityEngine;
using Roller.Core;

namespace Roller.Grid
{
    /// <summary>
    /// 터치(모바일)와 마우스(에디터) 입력 통합 핸들러.
    /// 탭과 드래그를 구분하여 그리드 셀 탭 감지.
    /// </summary>
    [RequireComponent(typeof(GridMap))]
    public class GridInput : MonoBehaviour
    {
        public Camera Cam;
        public float DragThresholdPx = 10f;

        GridMap _map;
        Vector2 _touchStartPos;
        bool _touchValid;
        Vector2 _mouseStartPos;
        bool _mouseTracking;

        void Awake()
        {
            _map = GetComponent<GridMap>();
            if (Cam == null) Cam = Camera.main;
        }

        public bool TryGetTappedCell(out Coord cell)
        {
            cell = default;

            // 멀티터치는 카메라용 — 셀 탭 감지 무시
            if (Input.touchCount >= 2)
            {
                _touchValid = false;
                return false;
            }

            // 모바일 터치
            if (Input.touchCount == 1)
            {
                var t = Input.GetTouch(0);
                switch (t.phase)
                {
                    case TouchPhase.Began:
                        _touchStartPos = t.position;
                        _touchValid = true;
                        return false;
                    case TouchPhase.Moved:
                        if (Vector2.Distance(t.position, _touchStartPos) > DragThresholdPx)
                            _touchValid = false;
                        return false;
                    case TouchPhase.Ended:
                        if (_touchValid &&
                            Vector2.Distance(t.position, _touchStartPos) <= DragThresholdPx)
                        {
                            cell = ScreenToCell(t.position);
                            _touchValid = false;
                            return _map.InBounds(cell);
                        }
                        _touchValid = false;
                        return false;
                }
                return false;
            }

            // 에디터 마우스 (실기에선 터치만 사용)
            if (Application.isEditor)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _mouseStartPos = Input.mousePosition;
                    _mouseTracking = true;
                }
                if (_mouseTracking && Input.GetMouseButtonUp(0))
                {
                    _mouseTracking = false;
                    if (Vector2.Distance((Vector2)Input.mousePosition, _mouseStartPos) <= DragThresholdPx)
                    {
                        cell = ScreenToCell(Input.mousePosition);
                        return _map.InBounds(cell);
                    }
                }
            }
            return false;
        }

        Coord ScreenToCell(Vector2 screenPos)
        {
            var worldPos = Cam.ScreenToWorldPoint(screenPos);
            return _map.WorldToCell(worldPos);
        }
    }
}
