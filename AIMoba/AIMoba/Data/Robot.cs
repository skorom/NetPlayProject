using AIMoba.Data;
using AIMoba.Models;
public class Robot : IPlayer
{
    public int difficulty { get; private set; } //a robot nehézségi szintje később állítható lesz
    public int ID { get; set; }

    public FieldState Mark { get; set; }

    public bool IsComputer => true;

    public int Index { get; set; }

    public Robot(FieldState mark, int id = 0, int diff = 1)
    {
        this.Mark = mark;
        this.ID = id;
        this.difficulty = diff;
    }

    public bool MakeMove(GridModel grid, Position pos = null)
    {
        return grid.MakeMove(pos.IPos, pos.JPos, this.Mark);
    }
}