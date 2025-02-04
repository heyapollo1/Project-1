
/*using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Obstacle))]
public class ObstacleEditor : Editor
{

    private void OnSceneGUI()
    {
        Obstacle obstacle = (Obstacle)target;

        if (obstacle == null || GridManager.Instance == null || GridManager.Instance.GetGridInstance() == null)
            return;

        Grid grid = GridManager.Instance.GetGridInstance();
        Vector3 snappedPosition = grid.GetSnappedPosition(obstacle.transform.position, obstacle.obstacleSize);
        // Only apply snapping if we have a grid and the obstacle is selected

        if (obstacle.transform.position != snappedPosition)
        {
            obstacle.AlignToGrid();
            obstacle.UpdateWalkability(true);
            // Record the position change for Undo/Redo
            Undo.RecordObject(obstacle.transform, "Snap Obstacle to Grid");

            // Snap the position
            obstacle.transform.position = snappedPosition;

            // Update walkability on the grid
            obstacle.UpdateWalkability(true);
        }
        //DrawObstacleBounds(obstacle, grid);
    }*/

    /*private void DrawObstacleBounds(Obstacle obstacle, Grid grid)
    {
        // Calculate the bottom-left position of the obstacle on the grid
        Vector3 bottomLeft = obstacle.transform.position - new Vector3(
            (obstacle.obstacleSize.x - 1) * grid.nodeDiameter / 2,
            (obstacle.obstacleSize.y - 1) * grid.nodeDiameter / 2,
            0);

        // Draw each node the obstacle occupies
        Handles.color = new Color(1, 0, 0, 0.25f); // Semi-transparent red for occupied nodes
        for (int x = 0; x < obstacle.obstacleSize.x; x++)
        {
            for (int y = 0; y < obstacle.obstacleSize.y; y++)
            {
                Vector3 nodePosition = bottomLeft + new Vector3(x * grid.nodeDiameter, y * grid.nodeDiameter, 0);
                Handles.DrawSolidRectangleWithOutline(
                    new Rect(nodePosition - Vector3.one * grid.nodeDiameter / 2, Vector2.one * grid.nodeDiameter),
                    new Color(1, 0, 0, 0.25f), // Fill color
                    Color.red // Outline color
                );
            }
        }
    }*/
//}