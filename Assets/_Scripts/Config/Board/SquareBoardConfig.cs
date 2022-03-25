using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileNode;
[CreateAssetMenu(menuName = "Configurations/Board/Square", fileName = "New Square Board Config")]
public class SquareBoardConfig : BoardConfig
{
    [SerializeField] private uint horiCount = 9;
    [SerializeField] private uint vertCount = 9;

    public override Vector2 GetBoundSize()
    {
        return new Vector2(horiCount, vertCount);
    }

    public override uint TotalNodeCount { get => horiCount * vertCount;}

    public override Dictionary<Vector2, BaseNode> GenerateBoard()
    {
        Dictionary<Vector2, BaseNode> nodes = new Dictionary<Vector2, BaseNode>();
        var board = new GameObject
        {
            name = "Board"
        };

        for (int yPos = 0; yPos < vertCount; yPos++)
        {
            for (int xPos = 0; xPos < horiCount; xPos++)
            {
                var tileNode = Instantiate(nodePrefab, board.transform);
                Vector2 pos = new Vector2(xPos, yPos);
                tileNode.Init(pos);
                nodes.Add(pos, tileNode);
            }
        }

        return nodes;
    }


}
