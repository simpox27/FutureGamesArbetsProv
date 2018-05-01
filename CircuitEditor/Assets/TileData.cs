public struct TileData
{
    public enum Type
    {
        
        Empty,
        Wire,
        NotGate,
        BufferGate
    }

    public int rotation;
    public readonly Type type;

    public TileData(Type type, int rotation)
    {
        this.type = type;
        this.rotation = rotation;
    }
}
