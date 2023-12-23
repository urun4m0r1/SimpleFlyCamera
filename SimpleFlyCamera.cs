/* ------------------------------------------------
 * SimpleFlyCamera.cs
 *
 * MIT License - http://www.opensource.org/licenses/mit-license.php
 * Copyright 2024. Rekorn. All Rights Reserved.
 * ------------------------------------------------ */

#nullable enable

using System.Collections;
using UnityEngine;

namespace RekornTools.SimpleFlyCamera
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Scripts/[Rekorn] SimpleFlyCamera")]
    public sealed class SimpleFlyCamera : MonoBehaviour
    {
        // @formatter:off

        #region System

        [Header("[System] Basic")]
        [SerializeField] private Camera? _camera;
        [SerializeField] private Transform? _transform;

        [Header("[System] Lerping")]
        [SerializeField] private bool _isLerpingEnabled = true;
        [SerializeField, Range(0f, 2f)] private float _lerpDuration = 0.5f;

        [Header("[System] Quit")]
        [SerializeField] private bool _useQuitFeature = true;
        [SerializeField] private KeyCode _quitKey = KeyCode.Escape;

        #endregion // System

        #region Debug

        [Header("[Debug] Basic")]
        [SerializeField] private bool _enableLogging;
        [SerializeField] private bool _showGizmos = true;

        [Header("[Debug] Selection")]
        [SerializeField, Range(0f, 10f)] private float _selectionDuration = 1f;
        [SerializeField] private Color _selectionColor = Color.red;
        [SerializeField, Range(0f, 100f)] private float _selectionThickness = 50f;

        [Header("[Debug] Speed Text")]
        [SerializeField, Range(0f, 10f)] private float _speedTextDuration = 1f;
        [SerializeField] private Color _speedTextColor = Color.white;
        [SerializeField, Range(0f, 100f)] private int _speedTextSize = 60;
        [SerializeField] private Vector2 _speedTextPosition = new(50f, 50f);

        #endregion // Debug

        #region Camera

        [Header("[Camera] Basic")]
        [SerializeField] private bool _useResetFeature = true;
        [SerializeField] private KeyCode _resetKey = KeyCode.Space;

        [Header("[Camera] Selection & Focus")]
        [SerializeField] private bool _useFocusFeature = true;
        [SerializeField] private KeyCode _selectKey = KeyCode.Mouse0;
        [SerializeField] private KeyCode _selectionModifier = KeyCode.LeftControl;
        [SerializeField] private KeyCode _focusKey = KeyCode.F;
        [SerializeField, Range(0f, 10f)] private float _focusDistance = 3f;

        [Header("[Camera] FOV")]
        [SerializeField] private bool _useFovControl = true;
        [SerializeField] private KeyCode _fovChangeTrigger = KeyCode.LeftControl;
        [SerializeField] private KeyCode _fovResetKey = KeyCode.R;
        [SerializeField,Range(float.Epsilon, 179f)] private float _minFov = 1f;
        [SerializeField,Range(float.Epsilon, 179f)] private float _maxFov = 100f;
        [SerializeField,Range(0f, 10f)] private float _fovChangeStep = 5f;

        #endregion // Camera

        #region Zoom

        [Header("[Zoom] Basic")]
        [SerializeField] private bool _useZoomFeature = true;
        [SerializeField, Range(0f, 1f)] private float _zoomSpeed = 0.1f;

        #endregion // Zoom

        #region Rotation

        [Header("[Rotation] Basic")]
        [SerializeField] private bool _useRotationFeature = true;
        [SerializeField] private KeyCode _rotationModifier = KeyCode.Mouse1;
        [SerializeField] private KeyCode _rotationAxisModifier = KeyCode.LeftAlt;
        [SerializeField] private Vector3 _rotationSpeed = new(-200f, 200f, 200f);
        [SerializeField] private string _yawAxisName = "Mouse X";
        [SerializeField] private string _pitchAxisName = "Mouse Y";
        [SerializeField] private string _rollAxisName = "Mouse X";

        #endregion // Rotation

        #region Drag

        [Header("[Drag] Basic")]
        [SerializeField] private bool _useDragFeature = true;
        [SerializeField] private KeyCode _dragModifier = KeyCode.Mouse2;
        [SerializeField] private KeyCode _dragAxisModifier = KeyCode.LeftAlt;
        [SerializeField] private Vector3 _dragSpeed = new(-3f, -3f, 3f);
        [SerializeField] private string _dragRightAxisName = "Mouse X";
        [SerializeField] private string _dragUpAxisName = "Mouse Y";
        [SerializeField] private string _dragForwardAxisName = "Mouse Y";

        #endregion // Drag

        #region Movement

        [Header("[Movement] Basic")]
        [SerializeField] private bool _useMovementFeature = true;
        [SerializeField] private KeyCode _movementModifier = KeyCode.Mouse1;
        [SerializeField] private Vector3 _movementSensitivity = new(5f, 3f, 5f);
        [SerializeField] private string _rightAxisName = "Horizontal";
        [SerializeField] private string _forwardAxisName = "Vertical";
        [SerializeField] private KeyCode _upKey = KeyCode.E;
        [SerializeField] private KeyCode _downKey = KeyCode.Q;

        [Header("[Movement] Speed Change")]
        [SerializeField] private bool _useSpeedChangeFeature = true;
        [SerializeField] private KeyCode _speedChangeModifier = KeyCode.Mouse1;
        [SerializeField, Range(0f, 5f)] private float _currentSpeed = 0.25f;
        [SerializeField, Range(0f, 5f)] private float _minSpeed = 0.01f;
        [SerializeField, Range(0f, 5f)] private float _maxSpeed = 2f;
        [SerializeField, Range(0f, 1f)] private float _speedChangeStep = 0.05f;

        [Header("[Movement] Speed Shift")]
        [SerializeField] private bool _useSpeedShiftFeature = true;
        [SerializeField] private KeyCode _speedUpModifier = KeyCode.LeftShift;
        [SerializeField] private KeyCode _speedDownModifier = KeyCode.LeftControl;
        [SerializeField, Range(0f, 5f)] private float _speedUpMultiplier = 2f;
        [SerializeField, Range(0f, 5f)] private float _speedDownMultiplier = 0.5f;

        #endregion // Movement

        // @formatter:on

        private Vector3 _initialPosition;
        private Quaternion _initialRotation;

        private float _initialFov;

        private readonly RaycastHit[] _raycastHits = new RaycastHit[1];
        private Transform? _selectedTransform;

        private float _speedMultiplier = 1f;

        private bool _isSelectionShown;
        private bool _isSpeedTextShown;

#if UNITY_EDITOR
        private void Reset()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_transform == null && _camera != null)
            {
                _transform = _camera.transform;
            }
        }

        private void OnValidate()
        {
            #region System

            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_transform == null && _camera != null)
            {
                _transform = _camera.transform;
            }

            #endregion // System

            #region Debug

            _speedTextPosition = new Vector2(
                Mathf.Clamp(_speedTextPosition.x, 0f, Screen.width),
                Mathf.Clamp(_speedTextPosition.y, 0f, Screen.height)
            );

            #endregion // Debug

            #region Camera

            var minFov = Mathf.Clamp(_minFov, float.Epsilon, _maxFov);
            var maxFov = Mathf.Clamp(_maxFov, _minFov, 179f);
            _minFov = minFov;
            _maxFov = maxFov;

            #endregion // Camera

            #region Rotation

            _rotationSpeed = new Vector3(
                Mathf.Clamp(_rotationSpeed.x, -1000f, 1000f),
                Mathf.Clamp(_rotationSpeed.y, -1000f, 1000f),
                Mathf.Clamp(_rotationSpeed.z, -1000f, 1000f)
            );

            #endregion // Rotation

            #region Drag

            _dragSpeed = new Vector3(
                Mathf.Clamp(_dragSpeed.x, -100f, 100f),
                Mathf.Clamp(_dragSpeed.y, -100f, 100f),
                Mathf.Clamp(_dragSpeed.z, -100f, 100f)
            );

            #endregion // Drag

            #region Movement

            _movementSensitivity = new Vector3(
                Mathf.Clamp(_movementSensitivity.x, -100f, 100f),
                Mathf.Clamp(_movementSensitivity.y, -100f, 100f),
                Mathf.Clamp(_movementSensitivity.z, -100f, 100f)
            );


            var currentSpeed = Mathf.Clamp(_currentSpeed, _minSpeed, _maxSpeed);
            var minSpeed = Mathf.Clamp(_minSpeed, 0f, _currentSpeed);
            var maxSpeed = Mathf.Clamp(_maxSpeed, _currentSpeed, 5f);
            _currentSpeed = currentSpeed;
            _minSpeed = minSpeed;
            _maxSpeed = maxSpeed;

            #endregion // Movement
        }

        [ContextMenu("Reset Camera")]
        private void ResetCamera()
        {
            if (_transform == null)
            {
                return;
            }

            _transform.position = new Vector3(0f, 1f, 1.5f);
            _transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            _transform.localScale = Vector3.one;

            if (_camera == null)
            {
                return;
            }

            _camera.fieldOfView = 45f;
            _camera.nearClipPlane = 0.01f;
        }
#endif // UNITY_EDITOR

        private void Awake()
        {
            if (_camera == null)
            {
                Debug.LogError($"[{nameof(SimpleFlyCamera)}] Camera not found");
                return;
            }

            if (_transform == null)
            {
                Debug.LogError($"[{nameof(SimpleFlyCamera)}] Transform not found");
                return;
            }

            _initialPosition = _transform.position;
            _initialRotation = _transform.rotation;

            _initialFov = _camera.fieldOfView;
        }

        private void Update()
        {
            var scrollDelta = Input.mouseScrollDelta.y;

            if (_camera == null || _transform == null)
            {
                return;
            }

            #region System

            if (_useQuitFeature &&
                Input.GetKeyDown(_quitKey))
            {
                QuitApplication();
                return;
            }

            #endregion // System

            #region Camera

            if (_useResetFeature &&
                Input.GetKeyDown(_resetKey))
            {
                if (_isLerpingEnabled)
                {
                    StartCoroutine(ResetTransformCoroutine());
                }
                else
                {
                    ResetTransform(_transform);
                }
            }

            if (_useFocusFeature &&
                Input.GetKeyDown(_selectKey))
            {
                if (_showGizmos)
                {
                    StartCoroutine(ShowSelectionCoroutine());
                }

                var withObjectSelection = Input.GetKey(_selectionModifier);
                SelectObject(_camera, withObjectSelection);
            }

            if (_useFocusFeature &&
                Input.GetKeyDown(_focusKey))
            {
                if (_showGizmos)
                {
                    StartCoroutine(ShowSelectionCoroutine());
                }

                if (_isLerpingEnabled)
                {
                    StartCoroutine(FocusOnSelectedObjectCoroutine());
                }
                else
                {
                    FocusOnSelectedObject(_transform, _selectedTransform);
                }
            }

            if (_useFovControl &&
                Input.GetKeyDown(_fovResetKey))
            {
                if (_isLerpingEnabled)
                {
                    StartCoroutine(ResetFovCoroutine());
                }
                else
                {
                    ResetFov(_camera);
                }
            }

            if (_useFovControl &&
                scrollDelta != 0f &&
                Input.GetKey(_fovChangeTrigger))
            {
                ChangeFov(_camera, scrollDelta);
            }

            #endregion // Camera

            #region Zoom

            if (_useZoomFeature &&
                scrollDelta != 0f &&
                !Input.GetKey(_fovChangeTrigger) && !Input.GetKey(_speedChangeModifier) &&
                Input.mousePosition.x >= 0f && Input.mousePosition.x <= Screen.width &&
                Input.mousePosition.y >= 0f && Input.mousePosition.y <= Screen.height)
            {
                AdjustZoom(_transform, scrollDelta);
            }

            #endregion // Zoom

            #region Rotation

            if (_useRotationFeature &&
                Input.GetKey(_rotationModifier))
            {
                var rotateDirection = new Vector3(
                    Input.GetAxis(_pitchAxisName),
                    Input.GetAxis(_yawAxisName),
                    Input.GetAxis(_rollAxisName)
                );

                var withAxisModifier = Input.GetKey(_rotationAxisModifier);
                RotateCamera(_transform, rotateDirection, withAxisModifier);
            }

            #endregion // Rotation

            #region Drag

            if (_useDragFeature &&
                Input.GetKey(_dragModifier))
            {
                var dragDirection = new Vector3(
                    Input.GetAxis(_dragRightAxisName),
                    Input.GetAxis(_dragUpAxisName),
                    Input.GetAxis(_dragForwardAxisName)
                );

                var withAxisModifier = Input.GetKey(_dragAxisModifier);
                DragPosition(_transform, dragDirection, withAxisModifier);
            }

            #endregion // Drag

            #region Movement

            if (_useSpeedChangeFeature &&
                scrollDelta != 0f &&
                Input.GetKey(_speedChangeModifier))
            {
                ChangeSpeed(scrollDelta);
            }

            if (_useSpeedShiftFeature)
            {
                if (Input.GetKey(_speedUpModifier))
                {
                    _speedMultiplier = _speedUpMultiplier;
                }
                else if (Input.GetKey(_speedDownModifier))
                {
                    _speedMultiplier = _speedDownMultiplier;
                }
                else
                {
                    _speedMultiplier = 1f;
                }
            }

            if (_useMovementFeature &&
                Input.GetKey(_movementModifier))
            {
                var upDirection = 0f;

                if (Input.GetKey(_upKey))
                {
                    upDirection = 1f;
                }
                else if (Input.GetKey(_downKey))
                {
                    upDirection = -1f;
                }

                var moveDirection = new Vector3(
                    Input.GetAxis(_rightAxisName),
                    upDirection,
                    Input.GetAxis(_forwardAxisName)
                );

                MoveCamera(_transform, moveDirection);
            }

            #endregion // Movement
        }

        #region System

        private static void QuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #endregion // System

        #region Camera

        private IEnumerator ResetTransformCoroutine()
        {
            var time = 0f;
            while (time < _lerpDuration && _transform != null)
            {
                var t = time / _lerpDuration;
                _transform.position = Vector3.Lerp(_transform.position, _initialPosition, t);
                _transform.rotation = Quaternion.Lerp(_transform.rotation, _initialRotation, t);

                time += Time.deltaTime;
                yield return null;
            }

            if (_transform == null)
            {
                yield break;
            }

            ResetTransform(_transform);
        }

        private void ResetTransform(Transform target)
        {
            target.position = _initialPosition;
            target.rotation = _initialRotation;
        }

        private void SelectObject(Camera source, bool withObjectSelection)
        {
            _selectedTransform = null;

            var ray = source.ScreenPointToRay(Input.mousePosition);
            var size = Physics.RaycastNonAlloc(ray, _raycastHits);
            if (size == 0)
            {
                return;
            }

            _selectedTransform = _raycastHits[0].transform;

            if (_enableLogging)
            {
                Debug.Log($"Object selected: {_selectedTransform.name}");
            }

#if UNITY_EDITOR
            if (withObjectSelection)
            {
                UnityEditor.Selection.activeGameObject = _selectedTransform.gameObject;
                UnityEditor.SceneView.lastActiveSceneView.FrameSelected();
            }
#endif
        }

        private IEnumerator FocusOnSelectedObjectCoroutine()
        {
            var time = 0f;
            while (time < _lerpDuration && _selectedTransform != null && _transform != null)
            {
                var position = _transform.position;
                var targetPosition = _selectedTransform.position;

                var t = time / _lerpDuration;

                _transform.position = Vector3.Lerp(position,
                    targetPosition - _transform.forward * _focusDistance, t);

                _transform.rotation = Quaternion.Lerp(_transform.rotation,
                    Quaternion.LookRotation(targetPosition - position), t);

                time += Time.deltaTime;
                yield return null;
            }

            if (_transform == null)
            {
                yield break;
            }

            FocusOnSelectedObject(_transform, _selectedTransform);
        }

        private void FocusOnSelectedObject(Transform target, Transform? selectedTransform)
        {
            if (selectedTransform == null)
            {
                return;
            }

            target.position = selectedTransform.position - target.forward * _focusDistance;
            target.LookAt(selectedTransform);
        }

        private IEnumerator ResetFovCoroutine()
        {
            var time = 0f;
            while (time < _lerpDuration && _camera != null)
            {
                var t = time / _lerpDuration;
                _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _initialFov, t);

                time += Time.deltaTime;
                yield return null;
            }

            if (_camera == null)
            {
                yield break;
            }

            ResetFov(_camera);
        }

        private void ResetFov(Camera target)
        {
            target.fieldOfView = _initialFov;
        }

        private void ChangeFov(Camera target, float scrollDelta)
        {
            var fov = target.fieldOfView;
            fov += -scrollDelta * _fovChangeStep;
            fov = Mathf.Clamp(fov, _minFov, _maxFov);
            target.fieldOfView = fov;
        }

        #endregion // Camera

        #region Zoom

        private void AdjustZoom(Transform target, float zoomDirection)
        {
            var deltaPosition = target.forward * (zoomDirection * _zoomSpeed);
            target.position += deltaPosition;
        }

        #endregion // Zoom

        #region Rotation

        private void RotateCamera(Transform target, Vector3 rotateDirection, bool withAxisModifier)
        {
            if (withAxisModifier)
            {
                rotateDirection.x = 0f;
                rotateDirection.y = 0f;
            }
            else
            {
                rotateDirection.z = 0f;
            }

            var deltaRotation = Vector3.Scale(rotateDirection, _rotationSpeed);
            target.eulerAngles += deltaRotation * Time.deltaTime;
        }

        #endregion // Rotation

        #region Drag

        private void DragPosition(Transform target, Vector3 dragDirection, bool withAxisModifier)
        {
            if (withAxisModifier)
            {
                dragDirection.y = 0f;
            }
            else
            {
                dragDirection.z = 0f;
            }

            var deltaPosition = Vector3.Scale(dragDirection, _dragSpeed);
            deltaPosition = target.TransformDirection(deltaPosition);
            target.position += deltaPosition * Time.deltaTime;
        }

        #endregion // Drag

        #region Movement

        private void ChangeSpeed(float scrollDelta)
        {
            var speed = _currentSpeed;
            speed += scrollDelta * _speedChangeStep;
            speed = Mathf.Clamp(speed, _minSpeed, _maxSpeed);
            _currentSpeed = speed;

            if (_enableLogging)
            {
                Debug.Log($"Camera speed: {_currentSpeed}");
            }

            if (_showGizmos)
            {
                StartCoroutine(ShowSpeedTextCoroutine());
            }
        }

        private void MoveCamera(Transform target, Vector3 moveDirection)
        {
            var movementSpeed = _movementSensitivity * (_currentSpeed * _speedMultiplier);
            var deltaPosition = Vector3.Scale(moveDirection, movementSpeed);
            deltaPosition = target.TransformDirection(deltaPosition);
            target.position += deltaPosition * Time.deltaTime;
        }

        #endregion // Movement

        #region Debug

        private IEnumerator ShowSelectionCoroutine()
        {
            if (_isSelectionShown)
            {
                yield break;
            }

            _isSelectionShown = true;

            var time = 0f;
            while (time < _selectionDuration)
            {
                if (_transform != null && _selectedTransform != null)
                {
                    Debug.DrawLine(
                        _transform.position,
                        _selectedTransform.position,
                        _selectionColor
                    );
                }

                time += Time.deltaTime;
                yield return null;
            }

            _isSelectionShown = false;
        }

        private IEnumerator ShowSpeedTextCoroutine()
        {
            if (_isSpeedTextShown)
            {
                yield break;
            }

            _isSpeedTextShown = true;

            var time = 0f;
            while (time < _speedTextDuration)
            {
                time += Time.deltaTime;
                yield return null;
            }

            _isSpeedTextShown = false;
        }

        private void OnGUI()
        {
            if (!_showGizmos)
            {
                return;
            }

            if (_isSelectionShown)
            {
                DrawSelectionOnGUI();
            }

            if (_isSpeedTextShown)
            {
                DrawCurrentSpeedOnGUI();
            }
        }

        private void DrawSelectionOnGUI()
        {
            if (_transform == null || _selectedTransform == null || _camera == null)
            {
                return;
            }

            var screenPosition = _camera.WorldToScreenPoint(_selectedTransform.position);
            var rect = new Rect(
                screenPosition.x - _selectionThickness / 2f,
                Screen.height - screenPosition.y - _selectionThickness / 2f,
                _selectionThickness, _selectionThickness);

            GUI.DrawTexture(
                rect,
                Texture2D.whiteTexture,
                ScaleMode.ScaleToFit,
                true,
                0f,
                _selectionColor,
                0f,
                0f
            );
        }

        private void DrawCurrentSpeedOnGUI()
        {
            var rect = new Rect(
                _speedTextPosition.x,
                _speedTextPosition.y,
                Screen.width - _speedTextPosition.x,
                Screen.height - _speedTextPosition.y
            );

            GUI.Label(rect, $"Camera speed: {_currentSpeed:F3}", new GUIStyle
            {
                fontSize = _speedTextSize,
                fontStyle = FontStyle.Bold,
                normal = { textColor = _speedTextColor }
            });
        }

        #endregion // Debug

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private void OnApplicationQuit()
        {
            StopAllCoroutines();
        }
    }
}