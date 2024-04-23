using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
    public class MapCamera : MapComponent
    {
        public Camera CachedCamera
        {
            get
            {
                return this.cachedCamera;
            }
        }

        public bool LimitsEnabled { get; set; }

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<Camera> CameraRendered;

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<float> Moved;

        private Vector3 PersistedLocation
        {
            get
            {
                return new Vector3(PlayerPrefs.GetFloat("MapCamera.x"), PlayerPrefs.GetFloat("MapCamera.y"), PlayerPrefs.GetFloat("MapCamera.z"));
            }
            set
            {
                PlayerPrefs.SetFloat("MapCamera.x", value.x);
                PlayerPrefs.SetFloat("MapCamera.y", value.y);
                PlayerPrefs.SetFloat("MapCamera.z", value.z);
            }
        }

        private float PersistedNormalizedZoom
        {
            get
            {
                return PlayerPrefs.GetFloat("PersistedNormalizedZoom", 0f);
            }
            set
            {
                PlayerPrefs.SetFloat("PersistedNormalizedZoom", value);
            }
        }

        public void Initialize()
        {
            GameObject gameObject = new GameObject("Camera");
            gameObject.transform.parent = base.transform;
            gameObject.transform.localPosition = Vector3.back * 500f;
            Camera camera = gameObject.AddComponent<Camera>();
            int cullingMask = 524288;
            camera.clearFlags = CameraClearFlags.Depth;
            camera.allowHDR = false;
            camera.allowMSAA = false;
            camera.depth = 0f;
            camera.cullingMask = cullingMask;
            camera.orthographic = true;
            camera.farClipPlane = 600f;
            UICamera uicamera = gameObject.AddComponent<UICamera>();
            uicamera.eventReceiverMask = camera.cullingMask;
            uicamera.useTouch = false;
            this.cachedCamera = camera;
            this.cachedCamera.orthographicSize = this.defaultOrthoSize;
            this.minOrthoSize = GardenGameSetup.Get.camMinOrthoSize;
            this.defaultOrthoSize = GardenGameSetup.Get.camDefaultOrthoSize;
            this.maxOrthoSize = GardenGameSetup.Get.camMaxOrthoSize;
            this.OrthoSize = this.defaultOrthoSize;
            this.LimitsEnabled = true;
            this.RestoreLocation();
        }

        public override void Destroy()
        {
            this.StoreLocation();
        }

        private void RestoreLocation()
        {
            base.transform.localPosition = this.PersistedLocation;
            this.NormalizedZoom = this.PersistedNormalizedZoom;
        }

        private void StoreLocation()
        {
            this.PersistedLocation = base.transform.localPosition;
            this.PersistedNormalizedZoom = this.NormalizedZoom;
        }

        private float GetDuration(float duration)
        {
            return (duration >= 0.0001f) ? duration : GardenGameSetup.Get.camZoomDuration;
        }

        public IEnumerator ZoomIn(float duration)
        {
            float current = this.cachedCamera.orthographicSize;
            yield return new Fiber.OnExit(delegate ()
            {
                this.cachedCamera.orthographicSize = this.minOrthoSize;
            });
            yield return FiberAnimation.Animate(this.GetDuration(duration), GardenGameSetup.Get.camZoomCurve, delegate (float t)
            {
                this.cachedCamera.orthographicSize = Mathf.Lerp(current, this.minOrthoSize, t);
            }, false);
            yield break;
        }

        public IEnumerator ZoomOut(float duration)
        {
            float current = this.cachedCamera.orthographicSize;
            yield return new Fiber.OnExit(delegate ()
            {
                this.cachedCamera.orthographicSize = this.defaultOrthoSize;
            });
            yield return FiberAnimation.Animate(this.GetDuration(duration), GardenGameSetup.Get.camZoomCurve, delegate (float t)
            {
                this.cachedCamera.orthographicSize = Mathf.Lerp(current, this.defaultOrthoSize, t);
            }, false);
            yield break;
        }

        public IEnumerator ZoomCustom(float duration, Rect boundingBox)
        {
            float current = this.cachedCamera.orthographicSize;
            duration = this.GetDuration(duration);
            yield return new Fiber.OnExit(delegate ()
            {
                this.cachedCamera.orthographicSize = boundingBox.height * 0.5f;
                this.transform.position = boundingBox.center;
            });
            yield return FiberHelper.RunParallel(new IEnumerator[]
            {
                FiberAnimation.Animate(duration, GardenGameSetup.Get.camZoomCurve, delegate(float t)
                {
                    this.cachedCamera.orthographicSize = Mathf.Lerp(current, boundingBox.height * 0.5f, t);
                }, false),
                FiberAnimation.MoveTransform(base.transform, base.transform.position, boundingBox.center, GardenGameSetup.Get.camZoomCurve, duration)
            });
            yield break;
        }

        public void ClampZoom()
        {
            if (!this.LimitsEnabled)
            {
                return;
            }
            this.cachedCamera.orthographicSize = Mathf.Clamp(this.cachedCamera.orthographicSize, this.minOrthoSize, this.maxOrthoSize);
        }

        public void SmoothMoveToDefaultDefaultZoom(float smoothness)
        {
            if (this.cachedCamera.orthographicSize > this.defaultOrthoSize)
            {
                this.cachedCamera.orthographicSize = Mathf.Lerp(this.cachedCamera.orthographicSize, this.defaultOrthoSize, smoothness);
            }
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                this.ClampZoom();
            }
            base.transform.position += (Vector3)this.velocity * Time.deltaTime;
            this.velocity *= 0.9f;
            if (base.OwnerMap != null && base.OwnerMap.InputArea != null)
            {
                Bounds bounds = base.OwnerMap.InputArea.GetComponent<BoxCollider>().bounds;
                float num = (float)Screen.width / (float)Screen.height;
                float val = bounds.max.y - this.cachedCamera.orthographicSize;
                float val2 = bounds.min.y + this.cachedCamera.orthographicSize;
                float val3 = bounds.max.x - this.cachedCamera.orthographicSize * num;
                float val4 = bounds.min.x + this.cachedCamera.orthographicSize * num;
                Vector3 position = base.transform.position;
                position.y = Math.Min(position.y, val);
                position.y = Math.Max(position.y, val2);
                position.x = Math.Min(position.x, val3);
                position.x = Math.Max(position.x, val4);
                base.transform.position = position;
            }
        }

        public Bounds VisibleAreaInWorldSpace()
        {
            float num = (float)Screen.width / (float)Screen.height;
            Vector3 size;
            size.x = 2f * this.cachedCamera.orthographicSize * num;
            size.y = 2f * this.cachedCamera.orthographicSize;
            size.z = float.MaxValue;
            return new Bounds(base.transform.position, size);
        }

        public IEnumerator AnimateToBounds(Bounds bounds, float duration, AnimationCurve zoomCurve, AnimationCurve movementCurve)
        {
            Vector3 startPos = base.transform.position;
            Vector3 endPos = bounds.center;
            endPos.z = startPos.z;
            float startZoom = this.cachedCamera.orthographicSize;
            float endZoom = bounds.size.y * 0.5f;
            endZoom = Mathf.Clamp(endZoom, this.minOrthoSize, this.maxOrthoSize);
            yield return FiberHelper.RunParallel(new IEnumerator[]
            {
                FiberAnimation.Animate(duration, zoomCurve, delegate(float t)
                {
                    this.cachedCamera.orthographicSize = Mathf.Lerp(startZoom, endZoom, t);
                }, false),
                FiberAnimation.MoveTransform(base.transform, startPos, endPos, movementCurve, duration)
            });
            yield break;
        }

        public void ResetZoom()
        {
            this.cachedCamera.orthographicSize = this.defaultOrthoSize;
        }

        public float OrthoSize
        {
            get
            {
                return this.cachedCamera.orthographicSize;
            }
            set
            {
                this.cachedCamera.orthographicSize = value;
                this.ClampZoom();
            }
        }

        public float NormalizedZoom
        {
            get
            {
                return Mathf.InverseLerp(this.maxOrthoSize, this.minOrthoSize, this.cachedCamera.orthographicSize);
            }
            set
            {
                this.cachedCamera.orthographicSize = Mathf.Lerp(this.maxOrthoSize, this.minOrthoSize, value);
            }
        }

        private void StoreChanges()
        {
            if (!this.LimitsEnabled)
            {
                return;
            }
            Vector3 localPosition = base.transform.localPosition;
            float orthographicSize = this.cachedCamera.orthographicSize;
            bool flag = Mathf.Abs(localPosition.x - this.lastLocalPosition.x) > 0.01f || Mathf.Abs(localPosition.y - this.lastLocalPosition.y) > 0.01f || Mathf.Abs(localPosition.z - this.lastLocalPosition.z) > 0.01f || Mathf.Abs(orthographicSize - this.lastOrthoSize) > 0.01f;
            if (flag != this.wasCameraChanged)
            {
                this.StoreLocation();
            }
            if (flag && this.Moved != null)
            {
                this.Moved(Vector3.Magnitude(this.lastLocalPosition - localPosition));
            }
            this.lastLocalPosition = localPosition;
            this.lastOrthoSize = orthographicSize;
            this.wasCameraChanged = flag;
        }

        private void LateUpdate()
        {
            this.StoreChanges();
            if (this.CameraRendered != null)
            {
                this.CameraRendered(this.cachedCamera);
            }
        }

        public const string CAMERA_ID = "camera";

        [NonSerialized]
        public float minOrthoSize = 50f;

        [NonSerialized]
        public float defaultOrthoSize = 100f;

        [NonSerialized]
        public float maxOrthoSize = 150f;

        public Vector2 velocity;

        private Camera cachedCamera;

        private const int MAP_LAYER = 19;

        private Vector3 lastLocalPosition;

        private float lastOrthoSize;

        private bool wasCameraChanged;
    }
}
