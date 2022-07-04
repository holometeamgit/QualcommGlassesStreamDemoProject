using UnityEngine;

namespace HoloMeSDK
{
    public class ScaleByPinch : HoloMono
    {
        [SerializeField] private float _min = 0.5f;
        [SerializeField] private float _max = 1.8f;

        private const float ZoomInFactor = 1.01f;
        private const float ZoomOutFactor = 0.99f;

        public void SetMinScale(float minScale)
        {
            _min = minScale;
        }

        public void SetMaxScale(float maxScale)
        {
            _max = maxScale;
        }

        protected override void OnUpdate()
        {
            if (Input.touchCount == 2)
            {
                var touch0 = Input.GetTouch(0);
                var touch1 = Input.GetTouch(1);

                if (touch0.phase != TouchPhase.Moved && touch1.phase != TouchPhase.Moved)
                {
                    return;
                }

                var touchZeroPrevPos = touch0.position - touch0.deltaPosition;
                var touchOnePrevPos = touch1.position - touch1.deltaPosition;

                var prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                var touchDeltaMag = (touch0.position - touch1.position).magnitude;

                var deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
                Scale(deltaMagnitudeDiff);
            }
        }

        private void Scale(float value)
        {
            if (value < 0)
            {
                transform.localScale = transform.localScale * ZoomInFactor;
            }
            else if (value > 0)
            {
                transform.localScale = transform.localScale * ZoomOutFactor;
            }
            else
            {
                return;
            }

            transform.localScale = new Vector3(Mathf.Clamp(transform.localScale.x, _min, _max),
                Mathf.Clamp(transform.localScale.y, _min, _max),
                Mathf.Clamp(transform.localScale.z, _min, _max));
        }
    }
}