using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileNode;
public abstract class BoardConfig : ScriptableObject
{
    [SerializeField] protected BaseNode nodePrefab = null;
    public abstract Dictionary<Vector2, BaseNode> GenerateBoard();
    public abstract uint TotalNodeCount { get;}
    public abstract Vector2 GetBoundSize();
}