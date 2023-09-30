public readonly struct Coord
{
    public int X { get; }
    public int Y { get; }

    public Coord(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Coord Left => new(X - 1, Y);
    public Coord Right => new(X + 1, Y);
    public Coord Above => new(X, Y - 1);
    public Coord Below => new(X, Y + 1);
}
