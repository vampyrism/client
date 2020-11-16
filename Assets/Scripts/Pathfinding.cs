using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private static Pathfinding _instance;
    public static Pathfinding Instance { get; private set; }

    private Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;
    private bool changedEndNodeIsWalkable = false;

    public Pathfinding (int width, int height) {
        Instance = this;
        grid = new Grid<PathNode>(width, height, 1f, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
        SetNodesIsWalkable();
        SetNeighbourLists();
    }

    public Grid<PathNode> GetGrid() {
        return grid;
    }
    
    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition) {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if (path == null) {
            return null;
        } else {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode pathNode  in path) {
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellsize() + Vector3.one * grid.GetCellsize() * .5f);
            }
            return vectorPath;
        }
    }

    public Vector3 FixCornerCollision(Vector3 targetPosition, Vector2 contactPosition) {
        Vector3 newPosition = new Vector3(0,0);
        return newPosition;
    }

    private List<PathNode> FindPath(int startX, int startY, int endX, int endY) {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        if (endNode.isWalkable == false) {
            endNode.isWalkable = true;
            changedEndNodeIsWalkable = true;
        }

        openList = new List<PathNode>{ startNode };
        closedList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                PathNode pathNode = grid.GetGridObject(x,y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while(openList.Count > 0) {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode) {
                // Reached final node
                if (changedEndNodeIsWalkable == true) {
                    endNode.isWalkable = false;
                    changedEndNodeIsWalkable = false;
                }
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in currentNode.neighbourList) {
                if (closedList.Contains(neighbourNode)) continue;
                if(!neighbourNode.isWalkable) {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost) {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if(!openList.Contains(neighbourNode)) {
                        openList.Add(neighbourNode);
                    }
                }
            }

        }

        // Out of nodes on the openList (Could not find a path)
        Debug.Log("Failed to find path");
        if (changedEndNodeIsWalkable == true) {
            endNode.isWalkable = false;
            changedEndNodeIsWalkable = false;
        }
        return null;
    }

    private PathNode GetNode(int x, int y) {
        return grid.GetGridObject(x, y);
    }

    private List<PathNode> CalculatePath(PathNode endNode) {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null) {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b) {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList) {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++) {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost) {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    private void SetNodesIsWalkable() {
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {

                //Check collision in the middle of the square
                Collider2D hit = Physics2D.OverlapPoint(new Vector2(x + 0.5f, y + 0.5f) * grid.GetCellsize());

                //Check collision in the bottom left part of the square
                if (hit == null) {
                    hit = Physics2D.OverlapPoint(new Vector2(x + 0.2f, y + 0.2f) * grid.GetCellsize());
                }

                //Check collision in the top left part of the square
                if (hit == null) {
                    hit = Physics2D.OverlapPoint(new Vector2(x + 0.2f, y + 0.8f) * grid.GetCellsize());
                }

                //Check collision in the top right part of the square
                if (hit == null) {
                    hit = Physics2D.OverlapPoint(new Vector2(x + 0.8f, y + 0.8f) * grid.GetCellsize());
                }

                //Check collision in the bottom right of the square
                if (hit == null) {
                    hit = Physics2D.OverlapPoint(new Vector2(x + 0.8f, y + 0.2f) * grid.GetCellsize());
                }

                if (hit != null) {
                    grid.GetGridObject(x, y).isWalkable = false;

                    //Debugging the collision detection on the grid.
                    //Debug.Log("Found Collision here: " + x + "," + y);
                    Debug.DrawLine(new Vector3(x, y + 0.5f) * grid.GetCellsize(), new Vector3(x + 1f, y + 0.5f) * grid.GetCellsize(), Color.red, 100f);
                    Debug.DrawLine(new Vector3(x + 0.5f, y) * grid.GetCellsize(), new Vector3(x + 0.5f, y + 1f) * grid.GetCellsize(), Color.red, 100f);
                }

            }
        }
    }

    private void SetNeighbourLists() {
        //Setting the neighbour lists for each PathNode
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.neighbourList = GetNeighbourList(pathNode);
            }
        }
    }

    public List<PathNode> GetNeighbourList(PathNode currentNode) {
        List<PathNode> neighbourList = new List<PathNode>();
        PathNode leftNode = GetNode(currentNode.x - 1, currentNode.y);
        PathNode rightNode = GetNode(currentNode.x + 1, currentNode.y);
        PathNode downNode = GetNode(currentNode.x, currentNode.y - 1);
        PathNode upNode = GetNode(currentNode.x, currentNode.y + 1);

        if (currentNode.x - 1 >= 0) {
            // Left
            if (leftNode.isWalkable == true) {
                neighbourList.Add(leftNode);
                // Left Down
                if (currentNode.y - 1 >= 0 && downNode.isWalkable == true) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
                // Left Up
                if (currentNode.y + 1 < grid.GetHeight() && upNode.isWalkable == true) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
            }
        }
        if (currentNode.x + 1 < grid.GetWidth()) {
            // Right
            if (rightNode.isWalkable == true) {
                neighbourList.Add(rightNode);
                // Right Down
                if (currentNode.y - 1 >= 0 && downNode.isWalkable == true) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
                // Right Up
                if (currentNode.y + 1 < grid.GetHeight() && upNode.isWalkable == true) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
            }
        }
        // Down
        if (currentNode.y - 1 >= 0 && downNode.isWalkable == true) neighbourList.Add(downNode);
        // Up
        if (currentNode.y + 1 < grid.GetHeight() && upNode.isWalkable == true) neighbourList.Add(upNode);

        return neighbourList;
    }

}
