namespace Project147.GameCore.Grid
{
    public enum TowerPlacementFailureReason
    {
        None = 0,
        AlreadyBlocked = 1,
        SpawnCell = 2,
        GoalCell = 3,
        BlocksAllPaths = 4
    }
}

