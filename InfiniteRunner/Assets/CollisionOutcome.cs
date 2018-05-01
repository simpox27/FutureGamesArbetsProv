public struct CollisionOutcome
{
    public float obstacleHeight;
    public float previousHeaight;
    public int outcome;

    public CollisionOutcome(float obstacleHeight, float previousHeaight, int outcome)
    {
        this.obstacleHeight = obstacleHeight;
        this.previousHeaight = previousHeaight;
        this.outcome = outcome;
    }
}