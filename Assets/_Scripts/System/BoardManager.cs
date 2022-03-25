using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileNode;
public class BoardManager : Singleton<BoardManager>, ISaveable
{
    [SerializeField] private BoardConfig boardConfig = null;

    private Dictionary<Vector2, BaseNode> allNodes = null;
    private List<Vector2> activeNodeKeys = null;
    private Stack<Vector2> queueNodeKeys = null;
    private List<Vector2> growUpNodeKeys = null;
    private GameManager gameManager = null;
    public uint TotalNodeCount => boardConfig.TotalNodeCount;

    public void Init()
    {
        allNodes = boardConfig.GenerateBoard();
        gameManager = GameManager.Instance;
        foreach (var node in allNodes)
        {
            node.Value.CatchNeighbors();
        }
    }

    private BaseNode GetRandomActiveNode()
    {
        Vector2 randomActiveKey = activeNodeKeys[Random.Range(0, activeNodeKeys.Count - 1)];
        return allNodes.TryGetValue(randomActiveKey, out var node) ? node : null;
    }

    public BaseNode FindNode(Vector2 pos)
    {
        return allNodes.TryGetValue(pos, out var node) ? node : null;
    }

    public bool OutOfActiveNode()
    {
        if (activeNodeKeys.Count == 0)
        {
            Debug.Log("Out of Active Node");
            return true;
        }
        return false;
    }

    #region Node Pool Methods
    public void ReleaseActiveNode(Vector2 pos)
    {
        activeNodeKeys.Remove(pos);
    }

    public void AddActiveNode(Vector2 pos)
    {
        activeNodeKeys.Add(pos);
    }

    public Vector2 ReleaseQueueNode()
    {
        var key = queueNodeKeys.Pop();
        AddActiveNode(key);
        return key;
    }

    public void AddQueueNode(Vector2 pos)
    {
        ReleaseActiveNode(pos);
        queueNodeKeys.Push(pos);
    }

    public void ReleaseGrowUpNode(Vector2 pos)
    {
        growUpNodeKeys.Remove(pos);
        AddActiveNode(pos);
    }

    public void AddGrowUpNode(Vector2 pos)
    {
        ReleaseActiveNode(pos);
        growUpNodeKeys.Add(pos);
    }

    public void ChainExplode(Stack<Vector2> keys)
    {
        while (keys.Count != 0)
        {
            var key = keys.Pop();
            FindNode(key).Explode();
            gameManager.Score();
            ReleaseGrowUpNode(key);
        }
        gameManager.ClearExplodeKeys();
    }
    #endregion

    public void GrowQueueBalls()
    {
        int count = 0;
        while (queueNodeKeys.Count != 0)
        {
            count++;
            var key = queueNodeKeys.Pop();
            growUpNodeKeys.Add(key);
            var node = FindNode(key);
            node.Grow();
            node.CheckChainExplode();
        }
    }

    public void AttachQueueBallToRandomActiveNode(BallColorConfig config)
    {
        var randomActiveNode = GetRandomActiveNode();
        AddQueueNode(randomActiveNode.Pos);
        randomActiveNode.AttachQueue(config.ColorID, config.Sprite);
    }

    private void AttachGrowUpBallToRandomActiveNode(BallColorConfig config)
    {
        var randomActiveNode = GetRandomActiveNode();
        AddGrowUpNode(randomActiveNode.Pos);
        randomActiveNode.AttachGrowUp(config.ColorID, config.Sprite);
    }

    public void CreateNewBoard(uint queueCount, uint growUpCount)
    {
        ClearResources();

        for (int index = 0; index < queueCount; index++)
        {
            var randomColorConfig = gameManager.GetRandomColorConfig();
            AttachQueueBallToRandomActiveNode(randomColorConfig);
        }

        for (int index = 0; index < growUpCount; index++)
        {
            var randomColorConfig = gameManager.GetRandomColorConfig();
            AttachGrowUpBallToRandomActiveNode(randomColorConfig);
        }
    }

    public void PopulateSaveData(SaveData saveData)
    {
        foreach (var key in queueNodeKeys)
        {
            SaveData.NodeData nodeData = new SaveData.NodeData
            {
                Pos = key,
                ColorConfigID = FindNode(key).Ball.ColorID
            };
            saveData.QueueNodes.Add(nodeData);
        }

        foreach (var key in growUpNodeKeys)
        {
            var node = FindNode(key);
            SaveData.NodeData nodeData = new SaveData.NodeData
            {
                Pos = key,
                ColorConfigID = node.Ball.ColorID,
                IsGhost = (node.NodeType == BaseNode.Type.GHOST)
            };
            saveData.GrowUpNodes.Add(nodeData);
        }
    }

    public void LoadFromSaveData(SaveData saveData)
    {
        ClearResources();
        foreach (var data in saveData.QueueNodes)
        {
            var key = data.Pos;
            var colorID = data.ColorConfigID;
            AddQueueNode(key);
            FindNode(key).AttachQueue(colorID, gameManager.GetSprite(colorID));
        }

        foreach (var data in saveData.GrowUpNodes)
        {
            var key = data.Pos;
            var colorID = data.ColorConfigID;
            AddGrowUpNode(key);
            var node = FindNode(key);
            node.AttachGrowUp(colorID, gameManager.GetSprite(colorID));
            if (data.IsGhost) node.BecomeGhostBall();
        }
    }

    public void ClearResources()
    {
        activeNodeKeys = new List<Vector2>();
        queueNodeKeys = new Stack<Vector2>();
        growUpNodeKeys = new List<Vector2>();
        foreach (var node in allNodes)
        {
            activeNodeKeys.Add(node.Key);
            node.Value.ResetNode();
        }
    }
}
