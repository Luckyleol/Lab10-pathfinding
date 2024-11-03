using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private List<Vector2Int> path = new List<Vector2Int>();
    [SerializeField] private Vector2Int start = new Vector2Int(0, 1);
    [SerializeField] private Vector2Int goal = new Vector2Int(4, 4);
    private Vector2Int next;
    private Vector2Int current;

    [SerializeField] private Vector2Int gridDimensions;

    [Range(0f, 1f)]
    [SerializeField] private float obstacleProbability;
    

    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    private int[,] grid = new int[,]
    {
        { 0, 1, 0, 0, 0 },
        { 0, 1, 0, 1, 0 },
        { 0, 0, 0, 1, 0 },
        { 0, 1, 1, 1, 0 },
        { 0, 0, 0, 0, 0 }
    };

    private void Start()
    {

        if (!IsInBounds(start) || !IsInBounds(goal))
        {
            Debug.LogError("Start or End point is out of bounds of grid. Please choose inbound value.");
            return;
        }
        this.transform.position = new Vector3(gridDimensions.x / 2, this.transform.position.y, gridDimensions.y / 2);
        this.GetComponent<Camera>().orthographicSize = gridDimensions.x / 2 + 1;
        int attempts = 0;
        do
        {
            
            GenerateRandomGrid(gridDimensions.x, gridDimensions.y, obstacleProbability);
            attempts++;
            if (attempts >= 50)
            {
                break;
            }

        } while (!FindPath(start, goal));

        Debug.Log("Number of attempts: " + attempts);
       

    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPoint = new Vector2Int(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.z));
            
            AddObstacle(gridPoint);
        }
    }

    public void GenerateRandomGrid(int width, int height, float obstacleProbability)
    {
        float rollingProbability = obstacleProbability;
        grid = new int[width, height];

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {

                Vector2Int nextPosition = new Vector2Int(x, y);
                if (nextPosition == start || nextPosition == goal)
                {
                    continue;
                }
                if(rollingProbability >= Random.Range(0f, 1f))
                {
                    AddObstacle(nextPosition);
                    rollingProbability = obstacleProbability;
                }
                else
                {
                    rollingProbability += .01f;
                    grid[y, x] = 0;
                }

            }
        }
    }

    public void AddObstacle(Vector2Int position)
    {
        //Add an obstacble at the position in the grid
        grid[position.y, position.x] = 1;
       
        
        // Populate oldList...

        var oldPath = path.GetRange(0, path.Count);
        if (!FindPath(start, goal))
        {
            grid[position.y, position.x] = 0;
            path = oldPath.GetRange(0, oldPath.Count);
        }
    }

    private void OnDrawGizmos()
    {
        float cellSize = 1f;

        // Draw grid cells
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Vector3 cellPosition = new Vector3(x * cellSize, 0, y * cellSize);
                Gizmos.color = grid[y, x] == 1 ? Color.black : Color.white;
                Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
            }
        }

        // Draw path
        foreach (var step in path)
        {
            Vector3 cellPosition = new Vector3(step.x * cellSize, 0, step.y * cellSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
        }

        // Draw start and goal
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(start.x * cellSize, 0, start.y * cellSize), new Vector3(cellSize, 0.1f, cellSize));

        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(goal.x * cellSize, 0, goal.y * cellSize), new Vector3(cellSize, 0.1f, cellSize));
    }



    private bool IsInBounds(Vector2Int point)
    {
        return point.x >= 0 && point.x < gridDimensions.x && point.y >= 0 && point.y < gridDimensions.y;
    }

    private bool FindPath(Vector2Int start, Vector2Int goal)
    {
        if (DoesPathWork())
        {
            return true;
        }
        path.Clear();
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();

            if (current == goal)
            {
                break;
            }

            foreach (Vector2Int direction in directions)
            {
                next = current + direction;

                if (IsInBounds(next) && grid[next.y, next.x] == 0 && !cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goal))
        {
            Debug.Log("Path not found.");
            return false;
        }

        // Trace path from goal to start
        Vector2Int step = goal;
        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Add(start);
        path.Reverse();
        return true;
    }

    bool DoesPathWork() 
    { 
        if (path.Count == 0)
        {
            return false;
        }
        foreach(var point in path)
        {
            if (grid[point.y, point.x] == 1)
            {
                return false;
            }
        }
        return true;
        
    }
}
