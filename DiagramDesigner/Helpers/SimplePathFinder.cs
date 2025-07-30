using Avalonia;
using DiagramDesigner;
using System.Collections.Generic;

internal class SimplePathFinder : IPathFinder
{
    //Written by Copilot
    public List<Point> GetConnectionLine(ConnectorInfo source, ConnectorInfo sink, bool showLastLine)
    {
        var points = new List<Point>();
        var start = source.Position;
        var end = sink.Position;
        points.Add(start);

        double offset = 30;
        double firstX = start.X, firstY = start.Y;
        double lastX = end.X, lastY = end.Y;
 
        switch (source.Orientation)
        {
            case ConnectorOrientation.Left:
                firstX -= offset;
                break;
            case ConnectorOrientation.Right:
                firstX += offset;
                break;
            case ConnectorOrientation.Top:
                firstY -= offset;
                break;
            case ConnectorOrientation.Bottom:
                firstY += offset;
                break;
        }

        switch (sink.Orientation)
        {
            case ConnectorOrientation.Left:
                lastX -= offset;
                break;
            case ConnectorOrientation.Right:
                lastX += offset;
                break;
            case ConnectorOrientation.Top:
                lastY -= offset;
                break;
            case ConnectorOrientation.Bottom:
                lastY += offset;
                break;
        }

        // first point
        points.Add(new Point(firstX, firstY));

        if (firstX != lastX && firstY != lastY)
        {
            points.Add(new Point(lastX, firstY));
            points.Add(new Point(lastX, lastY));
        }
        else if (firstX != lastX)
        {
            points.Add(new Point(lastX, firstY));
        }
        else if (firstY != lastY)
        {
            points.Add(new Point(firstX, lastY));
        }

        points.Add(new Point(lastX, lastY));

        if (showLastLine)
            points.Add(end);

        return points;
    }

    public List<Point> GetConnectionLine(ConnectorInfo source, Point sinkPoint, ConnectorOrientation preferredOrientation)
    {
        var points = new List<Point>();
        var start = source.Position;
        var end = sinkPoint;

        if (source.Orientation == ConnectorOrientation.Left || source.Orientation == ConnectorOrientation.Right)
        {
            double midX = (start.X + end.X) / 2;
            points.Add(start);
            points.Add(new Point(midX, start.Y));
            points.Add(new Point(midX, end.Y));
            points.Add(end);
        }
        else
        {
            double midY = (start.Y + end.Y) / 2;
            points.Add(start);
            points.Add(new Point(start.X, midY));
            points.Add(new Point(end.X, midY));
            points.Add(end);
        }
        return points;
    }

}