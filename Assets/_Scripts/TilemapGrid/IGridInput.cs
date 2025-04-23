using UnityEngine;

public abstract class IGridInput
{
    protected Camera mainCamera;
    public Vector3Int CellPosition { get; protected set; }

    public abstract Vector3Int? GetCurrentPosition(Grid grid);

}