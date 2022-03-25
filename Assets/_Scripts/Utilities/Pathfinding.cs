using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileNode;
public static class Pathfinding
{
    public static List<Vector2> FindPath(BaseNode startNode, BaseNode targetNode, uint totalNodeCount)
    {
        Heap<BaseNode> openSet = new Heap<BaseNode>(totalNodeCount);
        HashSet<BaseNode> closedSet = new HashSet<BaseNode>();
        bool ghostWalker = startNode.NodeType == BaseNode.Type.GHOST;

        openSet.Add(startNode);
        while (openSet.Count != 0)
        {
            var currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                List<Vector2> path = new List<Vector2>();
                while (currentNode != startNode)
                {
                    path.Add(currentNode.Pos);
                    currentNode = currentNode.Connection;
                }
                path.Add(currentNode.Pos);
                path.Reverse();
                return path;
            }

            foreach (var neighbor in currentNode.Neighbors.Values)
            {
                if (ghostWalker)
                {
                    if (closedSet.Contains(neighbor))
                        continue;
                }
                else if (!neighbor.Walkable || closedSet.Contains(neighbor))
                    continue;

                bool inOpenSet = openSet.Contains(neighbor);
                var newCostToNeighbor = currentNode.G + currentNode.DistanceFrom(neighbor);
                if (!inOpenSet || newCostToNeighbor < neighbor.G)
                {
                    neighbor.G = newCostToNeighbor;
                    neighbor.Connection = currentNode;

                    if (!inOpenSet)
                    {
                        neighbor.H = neighbor.DistanceFrom(targetNode);
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }
}