using UnityEngine;
using System;



[Serializable]
public class LevelDataRepresentation
{
    public Vector2 playerStartPosition;
    public CameraSettingsRepersentation cameraSettings;
    public LevelItemRepersentation[] levelItems;
}
