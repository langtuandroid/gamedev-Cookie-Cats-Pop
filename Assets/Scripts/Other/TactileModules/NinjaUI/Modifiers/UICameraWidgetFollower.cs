using System;
using UnityEngine;

namespace TactileModules.NinjaUI.Modifiers
{
    public class UICameraWidgetFollower : UIWidgetModifier
    {
        public Camera TargetCamera
        {
            get
            {
                return this.targetCamera;
            }
            set
            {
                this.targetCamera = value;
            }
        }

        private void Start()
        {
            this.parentCamera = UIViewManager.Instance.FindCameraFromObjectLayer(base.gameObject.layer).cachedCamera;
        }

        protected override void Modify(MeshData mesh)
        {
            Vector2 minPos;
            Vector2 maxPos;
            this.GetWidgetMinMax(mesh, out minPos, out maxPos);
            float width;
            float height;
            this.GetWidgetWidthAndHeight(minPos, maxPos, out width, out height);
            Vector2 pivot = this.CalculateViewPortPivot(width, height);
            Rect rect = this.CreateViewPortRect(pivot, width, height);
            this.TargetCamera.rect = rect;
        }

        private void GetWidgetMinMax(MeshData mesh, out Vector2 minPos, out Vector2 maxPos)
        {
            minPos = (maxPos = Vector2.zero);
            base.GetMinMax(mesh.verts, ref minPos, ref maxPos);
            this.AddWidgetPositionToMinMax(ref minPos, ref maxPos);
            this.ConvertMinMaxToViewPortPoint(ref minPos, ref maxPos);
        }

        private void AddWidgetPositionToMinMax(ref Vector2 min, ref Vector2 max)
        {
            Vector3 position = base.transform.position;
            min += (Vector2)position;
            max += (Vector2)position;
        }

        private void ConvertMinMaxToViewPortPoint(ref Vector2 minPos, ref Vector2 maxPos)
        {
            minPos = this.parentCamera.WorldToViewportPoint(minPos);
            maxPos = this.parentCamera.WorldToViewportPoint(maxPos);
        }

        private void GetWidgetWidthAndHeight(Vector2 minPos, Vector2 maxPos, out float width, out float height)
        {
            width = maxPos.x - minPos.x;
            height = maxPos.y - minPos.y;
        }

        private Vector2 CalculateViewPortPivot(float width, float height)
        {
            Vector2 vector = this.WidgetPositionToViewportPoint();
            return new Vector2(vector.x - width / 2f, vector.y - height / 2f);
        }

        private Vector2 WidgetPositionToViewportPoint()
        {
            return this.parentCamera.WorldToViewportPoint(base.transform.position);
        }

        private Rect CreateViewPortRect(Vector2 pivot, float width, float height)
        {
            return new Rect
            {
                x = pivot.x,
                y = pivot.y,
                width = width,
                height = height
            };
        }

        [SerializeField]
        private Camera targetCamera;

        private Camera parentCamera;
    }
}
