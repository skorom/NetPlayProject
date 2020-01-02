using AIMoba.Data;
using AIMoba.Models;
public class Robot : IPlayer
{
    public int difficulty { get; private set; } //a robot nehézségi szintje később állítható lesz
    public string ID { get; set; }

    public FieldState Mark { get; set; }

    public bool IsComputer => true;

    public int Turn { get; set; }

    public Robot(FieldState mark, string id = "0", int diff = 1)
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