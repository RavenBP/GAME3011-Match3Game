using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point
{
    public int x;
    public int y;

    public Point(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    public void Multiply(int value)
    {
        x *= value;
        y *= value;
    }

    public void Add(Point point)
    {
        x += point.x;
        y += point.y;
    }

    public Vector2 ToVector()
    {
        return new Vector2(x, y);
    }

    public bool Equals(Point point)
    {
        return (x == point.x && y == point.y);
    }

    public static Point FromVector(Vector2 vector)
    {
        return new Point((int)vector.x, (int)vector.y);
    }

    public static Point FromVector(Vector3 vector)
    {
        return new Point((int)vector.x, (int)vector.y);
    }

    public static Point Multipy(Point point, int value)
    {
        return new Point(point.x * value, point.y * value);
    }

    public static Point Add(Point a, Point b)
    {
        return new Point(a.x + b.x, a.y + b.y);
    }

    public static Point Clone(Point point)
    {
        return new Point(point.x, point.y);
    }

    public static Point Zero
    {
        get { return new Point(0, 0); }
    }

    public static Point One
    {
        get { return new Point(1, 1); }
    }

    public static Point Up
    {
        get { return new Point(0, 1); }
    }

    public static Point Down
    {
        get { return new Point(0, -1); }
    }

    public static Point Left
    {
        get { return new Point(-1, 0); }
    }

    public static Point Right
    {
        get { return new Point(1, 0); }
    }
}
