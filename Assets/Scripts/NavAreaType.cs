public enum NavAreaType
{
    Walkable = 0,
    NotWalkable = 1,
    Jump = 2,
    BlackDoor = 3,
    WhiteDoor = 4,
}

public static class NavAreaTypeUtil
{

    public static int GetBitFlag(this NavAreaType navAreaType)
    {
        return 1 << (int)navAreaType;
    }
    public static int CreateMask(params NavAreaType[] navAreaTypes)
    {
        int mask = 0;
        foreach(var type in navAreaTypes)
        {
            mask |= type.GetBitFlag();
        }
        return mask;
    }
}
