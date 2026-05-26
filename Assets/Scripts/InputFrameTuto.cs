/// <summary>
/// Represents a single recorded input frame.
/// We record INPUTS (not position) to ensure a physics-accurate replay.
/// </summary>
[System.Serializable]
public struct InputFrameTuto
{
    public float vertical;
    public float horizontal;
}
