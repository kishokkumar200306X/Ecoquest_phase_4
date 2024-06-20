using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class buildGrid : MonoBehaviour
{

    private int _rows = 45;
    private int _columns = 45;
    private float _cellSize = 1f; public float cellSize { get { return _cellSize; } }

    public List<building> buildings = new List<building>();

    private bool _isNotinRoad = false; // Private backing field

    public Vector3 GetStartPosition(int x, int y)
    {
        Vector3 position = transform.position;
        position += (transform.right.normalized * x * _cellSize) + (transform.forward.normalized * y * _cellSize);
        return position;
    }

    public Vector3 GetCenterPosition(int x, int y, int rows, int columns)
    {
        Vector3 position = GetStartPosition(x, y);
        position += (transform.right.normalized * columns * _cellSize / 2f) + (transform.forward.normalized * rows * _cellSize / 2f);
        return position;
    }

    public Vector3 GetEndPosition(int x, int y, int rows, int columns)
    {
        Vector3 position = GetStartPosition(x, y);
        position += (transform.right.normalized * columns * _cellSize) + (transform.forward.normalized * rows * _cellSize);
        return position;
    }

    public Vector3 GetEndPosition(building building)
    {
        return GetEndPosition(building.currentX, building.currentY, building.columns, building.rows);
    }

    // To check world position lies within a specified rectangular region on a plane
    public bool IsWorldPositionIsOnPlane(Vector3 position, int x, int y, int rows, int columns)
    {
        // To expand the click area for a better gameplay when moving the buidling
        int expandBy = 2;

        position = transform.InverseTransformPoint(position);

        int newX = x - expandBy;
        int newY = y - expandBy;

        Rect rect = new Rect(newX, newY, columns + 2 * expandBy, rows + 2 * expandBy);

        if (rect.Contains(new Vector2(position.x, position.z)))
        {
            return true;
        }
        return false;
    }

    public bool CanPlaceBuilding(building building, int x, int y)
    {
        // if (building.currentX < 0 || building.currentY < 0 || building.currentX + building.columns > _columns || building.currentY + building.rows > _rows)
        // {
        //     return false;
        // }

        // Calculate half widths and heights of the building and grid
        float buildingHalfWidth = building.columns * 0.5f;
        float buildingHalfHeight = building.rows * 0.5f;
        float gridHalfWidth = _columns * 0.5f;
        float gridHalfHeight = _rows * 0.5f;

        // Calculate the actual position of the building's bottom-left corner
        float buildingBottomLeftX = x - buildingHalfWidth;
        float buildingBottomLeftY = y - buildingHalfHeight;

        // Check if the building exceeds the grid boundaries
        if (buildingBottomLeftX < -gridHalfWidth - buildingHalfWidth || buildingBottomLeftX + building.columns + buildingHalfWidth > gridHalfWidth ||
            buildingBottomLeftY < -gridHalfHeight - buildingHalfWidth || buildingBottomLeftY + building.rows + buildingHalfWidth > gridHalfHeight)
        {
            return false;
        }

        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i] != building)
            {
                Rect rect1 = new Rect(buildings[i].currentX, buildings[i].currentY, buildings[i].columns, buildings[i].rows);
                Rect rect2 = new Rect(building.currentX, building.currentY, building.columns, building.rows);

                if (rect2.Overlaps(rect1))
                {
                    return false;
                }
            }
        }

        /*
        if (RoadHoverDetector.instance.isnNotinRoad == false)
        {
            return false;
        }
        */

        Debug.Log("RoadHoverDetector.instance.isnNotinRoad: " + RoadHoverDetector.instance.isnNotinRoad);

        _isNotinRoad = RoadHoverDetector.instance.isnNotinRoad;

        if (_isNotinRoad == false)
        {
            return false;
        }

        return true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        for (int i = 0; i <= _rows; i++)
        {
            Vector3 point = transform.position + transform.forward.normalized * _cellSize * (float)i;
            Gizmos.DrawLine(point, point + transform.right.normalized * _cellSize * (float)_columns);
        }
        for (int i = 0; i <= _columns; i++)
        {
            Vector3 point = transform.position + transform.right.normalized * _cellSize * (float)i;
            Gizmos.DrawLine(point, point + transform.forward.normalized * _cellSize * (float)_rows);
        }
    }
#endif
}
