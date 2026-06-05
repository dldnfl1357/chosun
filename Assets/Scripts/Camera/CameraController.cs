using UnityEngine;

namespace Roller.CameraCtrl
{
    /// <summary>
    /// 모바일 카메라 컨트롤러:
    /// - 두 손가락 핀치 줌
    /// - 두 손가락 드래그로 카메라 팬
    /// - 에디터에서 마우스 휠 줌 (개발 편의)
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [Tooltip("최소 정사영 사이즈 (가장 확대)")]
        public float MinSize = 2.5f;
        [Tooltip("최대 정사영 사이즈 (가장 축소)")]
        public float MaxSize = 6f;
        [Tooltip("핀치 줌 민감도")]
        public float PinchSensitivity = 0.01f;
        [Tooltip("두 손가락 팬 민감도")]
        public float TwoFingerPanSensitivity = 0.01f;
        [Tooltip("에디터 마우스 휠 줌 속도")]
        public float WheelZoomSpeed = 2f;

        Camera _cam;
        Vector2 _prevMidpoint;
        float _prevPinchDistance;
        bool _twoFingerActive;

        void Awake()
        {
            _cam = GetComponent<Camera>();
        }

        void Update()
        {
            HandleTouchCameraControl();
            if (Application.isEditor) HandleEditorWheelZoom();
        }

        void HandleTouchCameraControl()
        {
            if (Input.touchCount != 2)
            {
                _twoFingerActive = false;
                return;
            }

            var t0 = Input.GetTouch(0);
            var t1 = Input.GetTouch(1);
            Vector2 midpoint = (t0.position + t1.position) * 0.5f;
            float distance = Vector2.Distance(t0.position, t1.position);

            if (!_twoFingerActive ||
                t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
            {
                _twoFingerActive = true;
                _prevMidpoint = midpoint;
                _prevPinchDistance = distance;
                return;
            }

            // 줌
            float deltaDist = distance - _prevPinchDistance;
            float newSize = _cam.orthographicSize - deltaDist * PinchSensitivity;
            _cam.orthographicSize = Mathf.Clamp(newSize, MinSize, MaxSize);

            // 팬
            Vector2 deltaMid = midpoint - _prevMidpoint;
            float worldPerPx = _cam.orthographicSize * TwoFingerPanSensitivity;
            transform.position -= new Vector3(
                deltaMid.x * worldPerPx,
                deltaMid.y * worldPerPx,
                0f);

            _prevMidpoint = midpoint;
            _prevPinchDistance = distance;
        }

        void HandleEditorWheelZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.001f)
            {
                float newSize = _cam.orthographicSize - scroll * WheelZoomSpeed;
                _cam.orthographicSize = Mathf.Clamp(newSize, MinSize, MaxSize);
            }
        }
    }
}
