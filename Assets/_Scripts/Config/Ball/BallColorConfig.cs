using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Configurations/Ball Color", fileName = "New Ball Color Config")]
public class BallColorConfig : ScriptableObject
{
    [SerializeField] private int colorID = 0;
    [SerializeField] private Sprite sprite = null;

    [ContextMenu("New ID")]
    private void GenNewColorID() => colorID =  Random.Range(0, 100);

    public int ColorID => colorID;
    public Sprite Sprite => sprite;
}
