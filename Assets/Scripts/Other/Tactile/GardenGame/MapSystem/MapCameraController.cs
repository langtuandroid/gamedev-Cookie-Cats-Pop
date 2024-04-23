using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
    public class MapCameraController
    {
        public MapCameraController(MapInputArea inputArea, MapCamera camera)
        {
            this.inputArea = inputArea;
            this.camera = camera;
            this.fiber = new Fiber(FiberBucket.Update);
            this.Enabled = true;
        }

        public void Destroy()
        {
            this.fiber.Terminate();
            this.Enabled = false;
        }

        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                this.enabled = value;
                this.inputArea.TouchDown -= this.InputAreaOnTouchDown;
                this.inputArea.TouchUp -= this.InputAreaOnTouchUp;
                if (!this.enabled)
                {
                    this.fiber.Terminate();
                }
                else
                {
                    this.inputArea.TouchDown += this.InputAreaOnTouchDown;
                    this.inputArea.TouchUp += this.InputAreaOnTouchUp;
                }
            }
        }

        private void InputAreaOnScrolled(float delta)
        {
            Vector2 touchPosition = this.inputArea.GetTouchPosition(0);
            float orthoSize = this.camera.OrthoSize;
            this.camera.NormalizedZoom += delta * 0.25f;
            float d = orthoSize - this.camera.OrthoSize;
            Vector2 vector = this.camera.CachedCamera.ScreenToViewportPoint(touchPosition);
            vector += new Vector2(-0.5f, -0.5f);
            vector = new Vector2(vector.x * 2f * this.camera.CachedCamera.aspect, vector.y * 2f);
            this.camera.transform.position += d * (Vector3)vector;
        }

        private void InputAreaOnTouchDown(MapInputArea.MouseButtonEvent e)
        {
            if (e.Index > 1)
            {
                return;
            }
            this.touchDown[e.Index] = true;
            this.RefreshLogic();
        }

        private void InputAreaOnTouchUp(MapInputArea.MouseButtonEvent e)
        {
            if (e.Index > 1)
            {
                return;
            }
            this.touchDown[e.Index] = false;
            this.RefreshLogic();
        }

        private void RefreshLogic()
        {
            if (this.touchDown[0] && this.touchDown[1])
            {
                this.fiber.Start(this.PinchZoom(this.inputArea.GetTouchPosition(0), this.inputArea.GetTouchPosition(1)));
            }
            else if (this.touchDown[0] && !this.touchDown[1])
            {
                this.fiber.Start(this.Panning(this.inputArea.GetTouchPosition(0), 0));
            }
            else if (!this.touchDown[0] && this.touchDown[1])
            {
                this.fiber.Start(this.Panning(this.inputArea.GetTouchPosition(1), 1));
            }
            else
            {
                this.fiber.Terminate();
                if (this.movedVelocities.Count > 0)
                {
                    Vector3 a = new Vector3(0f, 0f, 0f);
                    for (int i = 0; i < this.movedVelocities.Count; i++)
                    {
                        a += this.movedVelocities[i];
                    }
                    this.camera.velocity = a / (float)this.movedVelocities.Count;
                }
            }
        }

        private IEnumerator WaitForDragThreshold(Vector2 initialTouchPosition, int index, float threshold)
        {
            for (; ; )
            {
                Vector2 currentTouchPosition = this.inputArea.GetTouchPosition(index);
                if ((initialTouchPosition - currentTouchPosition).magnitude > threshold)
                {
                    break;
                }
                yield return null;
            }
            yield break;
        }

        private IEnumerator Panning(Vector2 initialTouchPosition, int touchIndex)
        {
            this.movedVelocities.Clear();
            yield return this.WaitForDragThreshold(initialTouchPosition, touchIndex, 10f);
            initialTouchPosition = this.inputArea.GetTouchPosition(touchIndex);
            Vector3 initialCameraPosition = this.camera.transform.position;
            float screenToWorldMultiplier = this.camera.CachedCamera.ScreenToWorldPoint(Vector3.zero).x - this.camera.CachedCamera.ScreenToWorldPoint(new Vector3(1f, 0f, 0f)).x;
            Vector3 prevPosition = initialCameraPosition;
            for (; ; )
            {
                Vector2 toTouch = this.inputArea.GetTouchPosition(touchIndex) - initialTouchPosition;
                Vector3 deltaToMove = toTouch * screenToWorldMultiplier;
                Vector3 targetPosition = initialCameraPosition + deltaToMove;
                this.camera.transform.position = targetPosition;
                this.movedVelocities.Add((targetPosition - prevPosition) / Time.deltaTime);
                if (this.movedVelocities.Count > 5)
                {
                    this.movedVelocities.RemoveAt(0);
                }
                prevPosition = targetPosition;
                yield return null;
            }
            yield break;
        }

        private IEnumerator PinchZoom(Vector2 touch1StartPos, Vector2 touch2StartPos)
        {
            this.movedVelocities.Clear();
            Fiber check = new Fiber(this.WaitForDragThreshold(touch1StartPos, 0, 10f), FiberBucket.Manual);
            Fiber check2 = new Fiber(this.WaitForDragThreshold(touch2StartPos, 1, 10f), FiberBucket.Manual);
            while (check.Step())
            {
                if (!check2.Step())
                {
                    touch1StartPos = this.inputArea.GetTouchPosition(0);
                    touch2StartPos = this.inputArea.GetTouchPosition(1);
                    Vector2 startMid = (touch1StartPos + touch2StartPos) * 0.5f;
                    float startZoom = this.camera.OrthoSize;
                    Vector3 startposition = this.camera.transform.position;
                    float startDistanceBetweenTouches = Vector3.Distance(touch1StartPos, touch2StartPos);
                    float startScreenToWorldMultiplier = this.camera.CachedCamera.ScreenToWorldPoint(Vector3.zero).x - this.camera.CachedCamera.ScreenToWorldPoint(new Vector3(1f, 0f, 0f)).x;
                    Vector3 prevPosition = startposition;
                    Vector2 startScreenSize = new Vector2(startZoom * this.camera.CachedCamera.aspect, startZoom);
                    Vector2 translateFromZooming = Vector2.zero;
                    for (; ; )
                    {
                        Vector2 touch1Pos = this.inputArea.GetTouchPosition(0);
                        Vector2 touch2Pos = this.inputArea.GetTouchPosition(1);
                        Vector2 mid = (touch1Pos + touch2Pos) * 0.5f;
                        float distanceBetweenTouches = Vector2.Distance(touch1Pos, touch2Pos);
                        float pinchRatio = startDistanceBetweenTouches / distanceBetweenTouches;
                        float preOrtho = this.camera.OrthoSize;
                        this.camera.OrthoSize = startZoom * pinchRatio;
                        float clampedPinchRatio = this.camera.OrthoSize / startZoom;
                        float clampedPinchRatioMinusOne = clampedPinchRatio - 1f;
                        float deltaZoom = preOrtho - this.camera.OrthoSize;
                        Vector2 viewportPositionOfMid = this.camera.CachedCamera.ScreenToViewportPoint(startMid);
                        viewportPositionOfMid += new Vector2(-0.5f, -0.5f);
                        viewportPositionOfMid = new Vector2(viewportPositionOfMid.x * 2f * this.camera.CachedCamera.aspect, viewportPositionOfMid.y * 2f);
                        translateFromZooming += deltaZoom * viewportPositionOfMid;
                        float screenToWorldMultiplier = this.camera.CachedCamera.ScreenToWorldPoint(Vector3.zero).x - this.camera.CachedCamera.ScreenToWorldPoint(new Vector3(1f, 0f, 0f)).x;
                        Vector2 translateFromMidpoint = (mid - startMid) * screenToWorldMultiplier;
                        Vector3 targetPosition = startposition + (Vector3)translateFromZooming + (Vector3)translateFromMidpoint;
                        this.camera.transform.position = targetPosition;
                        this.movedVelocities.Add((targetPosition - prevPosition) / Time.deltaTime);
                        if (this.movedVelocities.Count > 5)
                        {
                            this.movedVelocities.RemoveAt(0);
                        }
                        prevPosition = targetPosition;
                        yield return null;
                    }
                }
                else
                {
                    yield return null;
                }
            }
            yield break;
        }

        private readonly MapInputArea inputArea;

        private readonly MapCamera camera;

        private readonly Fiber fiber;

        private bool enabled;

        private readonly List<Vector3> movedVelocities = new List<Vector3>();

        private const int MAX_VELOCITY_DELTAS = 5;

        private bool[] touchDown = new bool[2];
    }
}
