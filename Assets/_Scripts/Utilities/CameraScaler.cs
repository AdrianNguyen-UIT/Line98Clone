using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    [SerializeField] private BoardConfig boardConfig = null;
    [SerializeField] private float xOffset = 0.5f;
    [SerializeField] private float yOffset = 0.5f;

    void Start()
    {
        var camera = Camera.main;
        camera.orthographicSize = (boardConfig.GetBoundSize().x + xOffset) * Screen.height / Screen.width * 0.5f;
        camera.transform.position = new Vector3(
            -0.5f + boardConfig.GetBoundSize().x / 2.0f,
            (-0.5f + boardConfig.GetBoundSize().y + yOffset) / 2.0f,
            camera.transform.position.z);
    }
}
