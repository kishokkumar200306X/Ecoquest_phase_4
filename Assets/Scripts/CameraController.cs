using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{

    private static CameraController _instance = null; public static CameraController instance { get { return _instance; } }

    [SerializeField] private Camera _camera = null;
    [SerializeField] private float _moveSpeed = 50;
    [SerializeField] private float _moveSmooth = 5;

    [SerializeField] private float _zoomSmooth = 5;
    [SerializeField] private float _zoomSpeed = 5f;

    private Controls _inputs = null;

    private bool _zooming = false;
    private bool _moving = false;
    private Vector3 _center = Vector3.zero;
    private float _right = 10;
    private float _left = 10;
    private float _up = 10;
    private float _down = 10;
    private float _angle = 45;

    private float _zoom = 5;
    private float _zoomMax = 10;
    private float _zoomMin = 1;
    private Vector2 _zoomPositionOnScreen = Vector2.zero;
    private Vector3 _zoomPositionInWorld = Vector3.zero;
    private float _zoomBaseValue = 0;
    private float _zoomBaseDistance = 0;

    private Transform _root = null;
    private Transform _pivot = null;
    private Transform _target = null;

    private bool _building = false; public bool isPlacingBuilding { get { return _building; } set { _building = value; } }
    private Vector3 _buildBasePosition = Vector3.zero;
    private bool _movingBuilding = false;

    private bool _replacing = false; public bool isReplacingBuilding { get { return _replacing; } set { _replacing = value; } }
    private Vector3 _replaceBasePosition = Vector3.zero;
    private bool _replacingBuilding = false;

    public Vector3 planDownLeft = Vector3.zero;
    public Vector3 planTopRight = Vector3.zero;

    private void Awake()
    {
        _instance = this;
        _inputs = new Controls();
        _root = new GameObject("CameraHelper").transform;
        _pivot=new GameObject("CameraPivot").transform;
        _target = new GameObject("CameraTarget").transform;
        _camera.orthographic = true;
        _camera.nearClipPlane = 0;
    }

    private void Start()
    {
        Initialize(Vector3.zero, 50, 50, 40, 40, 45, 10, 5, 20);
    }
    public void Initialize(Vector3 center, float right, float left, float up, float down, float angle, float zoom, float zoomMin, float zoomMax)
    {
        _center = center;
        _right = right;
        _left = left;
        _up = up;
        _down = down;
        _angle = angle;

        _zoom = zoom;
        _zoomMin = zoomMin;
        _zoomMax = zoomMax;

        _camera.orthographicSize = _zoom;

        _zooming = false;
        _moving = false;
        _pivot.SetParent(_root);
        _target.SetParent(_pivot);

        _root.position = center;
        _root.localEulerAngles = Vector3.zero;

        _pivot.localPosition = Vector3.zero;
        _pivot.localEulerAngles = new Vector3(_angle, 0, 0);

        _target.localPosition = new Vector3(0, 0, -100);
        _target.localEulerAngles = Vector3.zero;
    }
    private void OnEnable()
    {
        _inputs.Enable();

        _inputs.Main.Move.started +=_=> MoveStarted();
        _inputs.Main.Move.canceled +=_=> MoveCanceled();

        _inputs.Main.TouchZoom.started += _ => ZoomStarted();
        _inputs.Main.TouchZoom.canceled += _ => ZoomCanceled();

        _inputs.Main.PointerClick.performed += _ => ScreenClicked();
    }

    private void OnDisable()
    {
        _inputs.Main.Move.started -= _ => MoveStarted();
        _inputs.Main.Move.canceled -= _ => MoveCanceled();

        _inputs.Main.TouchZoom.started -= _ => ZoomStarted();
        _inputs.Main.TouchZoom.canceled -= _ => ZoomCanceled();

        _inputs.Main.PointerClick.performed -= _ => ScreenClicked();
        _inputs.Disable();
    }

    private void ScreenClicked()
    {
        Vector2 position = _inputs.Main.PointerPosition.ReadValue<Vector2>();

        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);


        if (results.Count <= 0)
        {
            bool found = false;
            Vector3 planePosition = CameraScreenPositionToPlanePosition(position);
            for (int i = 0; i < UI_Main.instance._grid.buildings.Count; i++)
            {
                if (UI_Main.instance._grid.IsWorldPositionIsOnPlane(planePosition, UI_Main.instance._grid.buildings[i].currentX, UI_Main.instance._grid.buildings[i].currentY, UI_Main.instance._grid.buildings[i].rows, UI_Main.instance._grid.buildings[i].columns))
                {
                    found = true;
                    UI_Main.instance._grid.buildings[i].Selected();
                    break;
                }
            }
            if (!found)
            {
                if (building.selectedInstance != null)
                {
                    building.selectedInstance.Deselected();
                }
            }
        }
        else
        {
            if (building.selectedInstance != null)
            {
                //bool handled = false;
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].gameObject == UI_BuildingOptions.instance.infoButton.gameObject)
                    {
                        UI_Info.instance.OpenInfo();
                        //handled = true;
                        //Todo : show info
                    }
                    else if (results[i].gameObject == UI_BuildingOptions.instance.upgradeButton.gameObject)
                    {
                        //handled = true;
                        UI_BuildingUpgrade.instance.Open();
                    }
                }
                //if (handled)
                //{
                //    return;
                //}
                building.selectedInstance.Deselected();
            }
        }
    }


    private bool IsScreenPointOverUI(Vector2 position)
    {
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        return results.Count > 0;
    }


    private void MoveStarted()
    {
        if (UI_Main.instance.isActive)
        {
            if (_building)
            {
                _buildBasePosition = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
                if (UI_Main.instance._grid.IsWorldPositionIsOnPlane(_buildBasePosition, building.buildInstance.currentX, building.buildInstance.currentY, building.buildInstance.rows, building.buildInstance.columns))
                {
                    building.buildInstance.StartMovingOnGrid();
                    _movingBuilding = true;
                }
                
            }

            if (building.selectedInstance != null)
            {
                _replaceBasePosition = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
                if (UI_Main.instance._grid.IsWorldPositionIsOnPlane(_replaceBasePosition, building.selectedInstance.currentX, building.selectedInstance.currentY, building.selectedInstance.rows, building.selectedInstance.columns))
                {
                    building.selectedInstance.ShowBase();
                    if (!_replacing)
                    {
                        _replacing = true;
                    }
                    building.selectedInstance.StartMovingOnGrid();
                    
                    _replacingBuilding = true;
                }
            }

            if (_movingBuilding == false && _replacingBuilding == false)
            {
                _moving = true;
            }
            
        }
        
    }

    private void MoveCanceled()
    {
        _moving = false;
        _movingBuilding = false;
        _replacingBuilding = false;
    }

    private void ZoomStarted()
    {
        if (UI_Main.instance.isActive)
        {
            Vector2 touch0 = _inputs.Main.TouchPosition0.ReadValue<Vector2>();
            Vector2 touch1 = _inputs.Main.TouchPosition1.ReadValue<Vector2>();
            _zoomPositionOnScreen = Vector2.Lerp(touch0, touch1, 0.5f);
            _zoomPositionInWorld = CameraScreenPositionToPlanePosition(_zoomPositionOnScreen);
            _zoomBaseValue = _zoom;

            touch0.x /= Screen.width;
            touch1.x /= Screen.width;

            touch0.y /= Screen.height;
            touch1.y /= Screen.height;

            _zoomBaseDistance = Vector2.Distance(touch0, touch1);
            _zooming = true;
        }
        
    }

    private void ZoomCanceled()
    {
        _zooming = false;
    }

    private void Update()
    {
        if (Input.touchSupported == false)
        {
            float mouseScroll = _inputs.Main.MouseScroll.ReadValue<float>();
            if(mouseScroll > 0)
            {
                _zoom -= 3f * Time.deltaTime;
            }
            else if (mouseScroll < 0)
            {
                _zoom += 3f * Time.deltaTime;
            }
        }

        if (_zooming)
        {
            Vector2 touch0 = _inputs.Main.TouchPosition0.ReadValue<Vector2>();
            Vector2 touch1 = _inputs.Main.TouchPosition1.ReadValue<Vector2>();

            touch0.x /= Screen.width;
            touch1.x /= Screen.width;

            touch0.y /= Screen.height;
            touch1.y /= Screen.height;

            float currentDistance = Vector2.Distance(touch0, touch1);
            float deltaDistance = currentDistance - _zoomBaseDistance;
            _zoom = _zoomBaseValue - (deltaDistance * _zoomSpeed);

            Vector3 zoomCenter = CameraScreenPositionToPlanePosition(_zoomPositionOnScreen);
            _root.position += (_zoomPositionInWorld - zoomCenter);
        }

        else if (_moving)
        {
            Vector2 move = _inputs.Main.MoveDelta.ReadValue<Vector2>();
            if (move != Vector2.zero)
            {
                move.x /= Screen.width;
                move.y /= Screen.height;
                _root.position -= _root.right.normalized * move.x * _moveSpeed * _zoom / _zoomMax;
                _root.position -= _root.forward.normalized * move.y * _moveSpeed * _zoom / _zoomMax;
            }
        }

        // Limiting the camera movements
        AdjustBounds();

        if (_camera.orthographicSize != _zoom)
        {
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _zoom, _zoomSmooth * Time.deltaTime);
        }

        if (_camera.transform.position != _target.position)
        {
            Vector3 velocity = Vector3.zero;
            _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, _target.position, ref velocity, _moveSmooth * Time.deltaTime);
        }
        if (_camera.transform.rotation != _target.rotation)
        {
            _camera.transform.rotation = _target.rotation;
        }

        // add the (_buidling && _movingBuilding)
        if (_building && _movingBuilding)
        {
            // base position
            Vector3 pos = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
            building.buildInstance.UpdateGridPosition(_buildBasePosition, pos);
        }

        if (_replacing && _replacingBuilding)
        {
            // base position
            Vector3 pos = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
            building.selectedInstance.UpdateGridPosition(_replaceBasePosition, pos);
        }

        planDownLeft = CameraScreenPositionToPlanePosition(Vector2.zero);
        planTopRight = CameraScreenPositionToPlanePosition(new Vector2(Screen.width, Screen.height));


    }

    private void AdjustBounds()
    {
        if(_zoom < _zoomMin)
        {
            _zoom = _zoomMin;
        }
        if(_zoom > _zoomMax)
        {
            _zoom = _zoomMax;
        }

        float h = PlaneOrthographicSize();
        float w = h * _camera.aspect;

        if(h > (_up + _down) / 2f)
        {
            float n = (_up + _down) / 2f;
            _zoom = n * Mathf.Sin(_angle * Mathf.Deg2Rad);
        }
        if (w > (_right + _left) / 2f)
        {
            float n = (_right + _left) / 2f;
            _zoom = n * Mathf.Sin(_angle * Mathf.Deg2Rad) / _camera.aspect;
        }

        h = PlaneOrthographicSize();
        w = h * _camera.aspect;

        // top right = tr ,top left = tr ,down right = tr ,down left = tr ,
        Vector3 tr = _root.position + _root.right.normalized * w + _root.forward.normalized * h;
        Vector3 tl = _root.position - _root.right.normalized * w + _root.forward.normalized * h;
        Vector3 dr = _root.position + _root.right.normalized * w - _root.forward.normalized * h;
        Vector3 dl = _root.position - _root.right.normalized * w - _root.forward.normalized * h;

        if(tr.x > _center.x + _right)
        {
            _root.position += Vector3.left * Mathf.Abs(tr.x - (_center.x + _right));
        }
        if (tl.x < _center.x - _left)
        {
            _root.position += Vector3.right * Mathf.Abs((_center.x - _left) - tl.x);
        }

        if (tr.z > _center.z + _up)
        {
            _root.position += Vector3.back * Mathf.Abs(tr.z - (_center.z + _up));
        }
        if (dl.z < _center.z - _down)
        {
            _root.position += Vector3.forward * Mathf.Abs((_center.z - _down) - dl.z);
        }

    }

    private float PlaneOrthographicSize()
    {
        float h = _zoom * 2f;
        return h / Mathf.Sin(_angle * Mathf.Deg2Rad) / 2f;
    }

    private Vector3 CameraScreenPositionToWorldPosition(Vector2 position)
    {
        float h = _camera.orthographicSize * 2f;
        float w = _camera.aspect * h;

        Vector3 ancher = _camera.transform.position - (_camera.transform.right.normalized * w / 2f) - (_camera.transform.up.normalized * h / 2f);
        return ancher + (_camera.transform.right.normalized * position.x / Screen.width * w) + (_camera.transform.up.normalized * position.y / Screen.height * h);
    }

    public Vector3 CameraScreenPositionToPlanePosition(Vector2 position)
    {
        Vector3 point = CameraScreenPositionToWorldPosition(position);
        float h = point.y - _root.position.y;
        float x = h / Mathf.Sin(_angle * Mathf.Deg2Rad);
        return point + _camera.transform.forward.normalized * x;
    }
}