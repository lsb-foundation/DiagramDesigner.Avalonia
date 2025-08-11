using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiagramDesigner;

public class ArcOrthogonalPathFinder : IPathFinder
{
    private const int const_arcRadius = 4;
    private const int const_arcPointsNumber = 6;

    public ConnectionViewModel Connection { get; set; }

    public static IDiagramViewModel DiagramViewModel { get; set; }

    public List<Point> GetConnectionLine(ConnectorInfo source, ConnectorInfo sink, bool showLastLine)
    {
        var route = GetRouteWithFullConnectionLine(source, sink);
        route = DoAddArc(route, Connection);
        return route.ToList();
    }

    public List<Point> GetConnectionLine(ConnectorInfo source, Point sinkPoint, ConnectorOrientation preferredOrientation)
    {
        return GetRouteWithPartConnectionLine(source, sinkPoint, preferredOrientation).ToList();
    }

    /// <summary>
    /// 根据旋转角度重新确定方向
    /// </summary>
    /// <param name="connector"></param>
    /// <returns></returns>
    private static ConnectorOrientation GetRotatedOrientation(FullyCreatedConnectorInfo connector)
    {
        var list = new ConnectorOrientation[4]
        {
                ConnectorOrientation.Top,
                ConnectorOrientation.Right,
                ConnectorOrientation.Bottom,
                ConnectorOrientation.Left
        };

        if (!list.Contains(connector.Orientation)) return connector.Orientation;

        var angle = connector.DataItem.Angle;
        if (angle % 90 != 0) return connector.Orientation;
        int index = (int)(Array.IndexOf(list, connector.Orientation) + angle / 90) % 4;
        return list[index];
    }

    /// <summary>
    /// 查找交点并替换弧线
    /// </summary>
    /// <param name="points"></param>
    /// <param name="diagram"></param>
    /// <param name="currentLink"></param>
    /// <returns></returns>
    private static Point[] DoAddArc(Point[] points, ConnectionViewModel currentLink)
    {
        var route = new List<Point>();
        var currentLines = new List<Line>();
        for (int i = 0; i < points.Length - 1; i++)
        {
            currentLines.Add(new Line(points[i], points[i + 1]));
        }

        var intersections = GetIntersections(currentLines, DiagramViewModel, currentLink);
        route.Add(currentLines[0].Start);
        foreach (var line in currentLines)
        {
            if (line.Start.X == line.End.X && line.Start.Y == line.End.Y) continue;

            bool isHorizon = line.Start.Y == line.End.Y;

            bool isS2L = isHorizon ? line.Start.X < line.End.X :
                line.Start.Y < line.End.Y;  //方向从小到达

            var inters = intersections.Where(v =>
                (v.X == line.Start.X && v.Y.IsBetween(line.Start.Y, line.End.Y)) ||
                (v.Y == line.Start.Y && v.X.IsBetween(line.Start.X, line.End.X)));

            inters = isHorizon ?
                (isS2L ? inters.OrderBy(v => v.X) : inters.OrderByDescending(v => v.X)) :
                (isS2L ? inters.OrderBy(v => v.Y) : inters.OrderByDescending(v => v.Y));

            foreach (var inter in inters)
            {
                if (isHorizon)   //线条是水平方向
                {
                    var arcPoints = HorizontalArc(inter);
                    if (line.Start.X < line.End.X)    //线条从左到右
                    {
                        route.Add(new Point(inter.X - const_arcRadius, inter.Y));
                        route.AddRange(arcPoints);
                        route.Add(new Point(inter.X + const_arcRadius, inter.Y));
                    }
                    else
                    {
                        arcPoints.Reverse();
                        route.Add(new Point(inter.X + const_arcRadius, inter.Y));
                        route.AddRange(arcPoints);
                        route.Add(new Point(inter.X - const_arcRadius, inter.Y));
                    }
                }
                else
                {
                    var arcPoints = VerticalArc(inter);
                    if (line.Start.Y < line.End.Y)    //线条从上到下
                    {
                        route.Add(new Point(inter.X, inter.Y - const_arcRadius));
                        route.AddRange(arcPoints);
                        route.Add(new Point(inter.X, inter.Y + const_arcRadius));
                    }
                    else
                    {
                        arcPoints.Reverse();
                        route.Add(new Point(inter.X, inter.Y + const_arcRadius));
                        route.AddRange(arcPoints);
                        route.Add(new Point(inter.X, inter.Y - const_arcRadius));
                    }
                }
            }
            route.Add(line.End);
        }
        return route.ToArray();
    }

    private static List<Point> GetIntersections(List<Line> currentLines, IDiagramViewModel diagram, ConnectionViewModel currentLink)
    {
        var intersections = new List<Point>();
        foreach (var link in diagram.Items.OfType<ConnectionViewModel>())
        {
            if (link == currentLink) continue;

            var linkLines = new List<Line>();
            for (int i = 0; i < link.ConnectionPoints.Count - 1; i++)
            {
                var startPoint = new Point(link.ConnectionPoints[i].X + link.Area.Left, link.ConnectionPoints[i].Y + link.Area.Top);
                var endPoint = new Point(link.ConnectionPoints[i + 1].X + link.Area.Left, link.ConnectionPoints[i + 1].Y + link.Area.Top);

                linkLines.Add(new Line(startPoint, endPoint));
            }

            foreach (var line in currentLines)
            {
                if (line.Start.X == line.End.X && line.Start.Y == line.End.Y) continue; //是一个点
                if (line.Start.X != line.End.X && line.Start.Y != line.End.Y) continue;  //非水平/垂直线，说明是弧线，需要跳过
                foreach (var linkLine in linkLines)
                {
                    if (linkLine.Start.X == linkLine.End.X && linkLine.Start.Y == linkLine.End.Y) continue; //是一个点
                    if (linkLine.Start.X != linkLine.End.X && linkLine.Start.Y != linkLine.End.Y) continue; //非水平/垂直线，说明是弧线，需要跳过

                    var intersection = line.GetIntersection(linkLine);
                    if (intersection == null) continue;
                    intersections.Add(intersection.Value);
                }
            }
        }
        return intersections;
    }

    private static List<Point> HorizontalArc(Point intersection, int radius = const_arcRadius)
    {
        var arcPoints = new List<Point>();

        double centerX = intersection.X;
        double centerY = intersection.Y;

        // Start angle: 180 degrees (left), end angle: 0 degrees (right)
        for (int i = 0; i <= const_arcPointsNumber; i++)
        {
            double angle = Math.PI + (Math.PI * i) / const_arcPointsNumber; // From 180 to 360 degrees
            double x = radius * Math.Cos(angle) + centerX;
            double y = radius * Math.Sin(angle) + centerY;
            arcPoints.Add(new Point(x, y));
        }

        return arcPoints;
    }

    private static List<Point> VerticalArc(Point intersection, int radius = const_arcRadius)
    {
        var arcPoints = new List<Point>();

        double centerX = intersection.X;
        double centerY = intersection.Y;

        // Start angle: 270 degrees (down), end angle: 90 degrees (up)
        for (int i = 0; i <= const_arcPointsNumber; i++)
        {
            double angle = 3 * Math.PI / 2 + (Math.PI * i) / const_arcPointsNumber; // From 270 to 450 degrees
            double x = centerX + (int)(radius * Math.Cos(angle));
            double y = centerY + (int)(radius * Math.Sin(angle));
            arcPoints.Add(new Point(x, y));
        }

        return arcPoints;
    }

    private static Point[] GetRouteWithFullConnectionLine(ConnectorInfo source, ConnectorInfo sink)
    {
        Point sourcePoint = source.Position;
        Point sinkPoint = sink.Position;

        ConnectorOrientation sourceOrientation = source.Orientation;
        ConnectorOrientation sinkOrientation = sink.Orientation;

        //Modify 2025.07.09 根据旋转角度调整Orientation
        //ConnectorOrientation sourceOrientation = GetRotatedOrientation(link.SourceConnectorInfoFully);
        //ConnectorOrientation sinkOrientation = GetRotatedOrientation(link.SinkConnectorInfoFully);

        List<Point> linePoints = new List<Point>();
        int margin1 = 0;
        int margin2 = const_margin;

        Rect rectSource = GetRectWithMargin(sourcePoint, margin1);
        Rect rectSink = GetRectWithMargin(sinkPoint, margin2);

        Point startPoint = GetOffsetPoint(sourcePoint, sourceOrientation, rectSource, isInnerPoint: true);
        Point endPoint = GetOffsetPoint(sinkPoint, sinkOrientation, rectSink);

        linePoints.Add(startPoint);
        Point currentPoint = startPoint;

        if (!rectSink.Contains(currentPoint) && !rectSource.Contains(endPoint))
        {
            while (true)
            {
                #region source node

                if (IsPointVisible(currentPoint, endPoint, new Rect[] { rectSource, rectSink }))
                {
                    linePoints.Add(endPoint);
                    currentPoint = endPoint;
                    break;
                }

                Point neighbour = GetNearestVisibleNeighborSink(currentPoint, endPoint, sinkOrientation, rectSource, rectSink);
                if (!double.IsNaN(neighbour.X))
                {
                    linePoints.Add(neighbour);
                    linePoints.Add(endPoint);
                    currentPoint = endPoint;
                    break;
                }

                if (currentPoint == startPoint)
                {
                    bool flag;
                    Point n = GetNearestNeighborSource(sourceOrientation, endPoint, rectSource, rectSink, out flag, isInnerPoint: true);
                    if (linePoints.Contains(n))
                    {
                        break;
                    }
                    linePoints.Add(n);
                    currentPoint = n;

                    if (!IsRectVisible(currentPoint, rectSink, new Rect[] { rectSource }))
                    {
                        Point n1, n2;
                        GetOppositeCorners(sourceOrientation, rectSource, out n1, out n2, isInnerPoint: true);
                        if (flag)
                        {
                            linePoints.Add(n1);
                            currentPoint = n1;
                        }
                        else
                        {
                            linePoints.Add(n2);
                            currentPoint = n2;
                        }
                        if (!IsRectVisible(currentPoint, rectSink, new Rect[] { rectSource }))
                        {
                            if (flag)
                            {
                                linePoints.Add(n2);
                                currentPoint = n2;
                            }
                            else
                            {
                                linePoints.Add(n1);
                                currentPoint = n1;
                            }
                        }
                    }
                }
                #endregion

                #region sink node

                else // from here on we jump to the sink node
                {
                    Point n1, n2; // neighbour corner
                    Point s1, s2; // opposite corner
                    GetNeighborCorners(sinkOrientation, rectSink, out s1, out s2);
                    GetOppositeCorners(sinkOrientation, rectSink, out n1, out n2);

                    bool n1Visible = IsPointVisible(currentPoint, n1, new Rect[] { rectSource, rectSink });
                    bool n2Visible = IsPointVisible(currentPoint, n2, new Rect[] { rectSource, rectSink });

                    if (n1Visible && n2Visible)
                    {
                        if (rectSource.Contains(n1))
                        {
                            linePoints.Add(n2);
                            if (rectSource.Contains(s2))
                            {
                                linePoints.Add(n1);
                                linePoints.Add(s1);
                            }
                            else
                                linePoints.Add(s2);

                            linePoints.Add(endPoint);
                            currentPoint = endPoint;
                            break;
                        }

                        if (rectSource.Contains(n2))
                        {
                            linePoints.Add(n1);
                            if (rectSource.Contains(s1))
                            {
                                linePoints.Add(n2);
                                linePoints.Add(s2);
                            }
                            else
                                linePoints.Add(s1);

                            linePoints.Add(endPoint);
                            currentPoint = endPoint;
                            break;
                        }

                        if ((Distance(n1, endPoint) <= Distance(n2, endPoint)))
                        {
                            linePoints.Add(n1);
                            if (rectSource.Contains(s1))
                            {
                                linePoints.Add(n2);
                                linePoints.Add(s2);
                            }
                            else
                                linePoints.Add(s1);
                            linePoints.Add(endPoint);
                            currentPoint = endPoint;
                            break;
                        }
                        else
                        {
                            linePoints.Add(n2);
                            if (rectSource.Contains(s2))
                            {
                                linePoints.Add(n1);
                                linePoints.Add(s1);
                            }
                            else
                                linePoints.Add(s2);
                            linePoints.Add(endPoint);
                            currentPoint = endPoint;
                            break;
                        }
                    }
                    else if (n1Visible)
                    {
                        linePoints.Add(n1);
                        if (rectSource.Contains(s1))
                        {
                            linePoints.Add(n2);
                            linePoints.Add(s2);
                        }
                        else
                            linePoints.Add(s1);
                        linePoints.Add(endPoint);
                        currentPoint = endPoint;
                        break;
                    }
                    else
                    {
                        linePoints.Add(n2);
                        if (rectSource.Contains(s2))
                        {
                            linePoints.Add(n1);
                            linePoints.Add(s1);
                        }
                        else
                            linePoints.Add(s2);
                        linePoints.Add(endPoint);
                        currentPoint = endPoint;
                        break;
                    }
                }
                #endregion
            }
        }
        else
        {
            linePoints.Add(endPoint);
        }

        linePoints = OptimizeLinePoints(linePoints, new Rect[] { rectSource, rectSink }, sourceOrientation, sinkOrientation);

        linePoints.Insert(0, sourcePoint);
        linePoints.Add(sinkPoint);

        return linePoints.ToArray();
    }

    const int const_margin = 20;

    private static Point[] GetRouteWithPartConnectionLine(ConnectorInfo source, Point sink, ConnectorOrientation preferredOrientation)
    {
        Point sourcePoint = source.Position;
        Point sinkPoint = sink;

        ConnectorOrientation sourceOrientation = source.Orientation;

        List<Point> linePoints = new List<Point>();
        int margin = 0;

        Rect rectSource = GetRectWithMargin(sourcePoint, margin);
        Point startPoint = GetOffsetPoint(sourcePoint, sourceOrientation, rectSource, true);
        Point endPoint = sinkPoint;

        linePoints.Add(startPoint);
        Point currentPoint = startPoint;

        if (!rectSource.Contains(endPoint))
        {
            while (true)
            {
                if (IsPointVisible(currentPoint, endPoint, new Rect[] { rectSource }))
                {
                    linePoints.Add(endPoint);
                    break;
                }

                bool sideFlag;
                Point n = GetNearestNeighborSource(sourceOrientation, endPoint, rectSource, out sideFlag, isInnerPoint: true);
                linePoints.Add(n);
                currentPoint = n;

                if (IsPointVisible(currentPoint, endPoint, new Rect[] { rectSource }))
                {
                    linePoints.Add(endPoint);
                    break;
                }
                else
                {
                    Point n1, n2;
                    GetOppositeCorners(sourceOrientation, rectSource, out n1, out n2, isInnerPoint: true);
                    if (sideFlag)
                        linePoints.Add(n1);
                    else
                        linePoints.Add(n2);

                    linePoints.Add(endPoint);
                    break;
                }
            }
        }
        else
        {
            linePoints.Add(endPoint);
        }

        if (preferredOrientation != ConnectorOrientation.None)
            linePoints = OptimizeLinePoints(linePoints, new Rect[] { rectSource }, sourceOrientation, preferredOrientation);
        else
            linePoints = OptimizeLinePoints(linePoints, new Rect[] { rectSource }, sourceOrientation, GetOpositeOrientation(sourceOrientation));

        linePoints.Insert(0, sourcePoint);

        return linePoints.ToArray();
    }

    private static List<Point> OptimizeLinePoints(List<Point> linePoints, Rect[] rectangles, ConnectorOrientation sourceOrientation, ConnectorOrientation sinkOrientation)
    {
        List<Point> points = new List<Point>();
        int cut = 0;

        for (int i = 0; i < linePoints.Count; i++)
        {
            if (i >= cut)
            {
                for (int k = linePoints.Count - 1; k > i; k--)
                {
                    if (IsPointVisible(linePoints[i], linePoints[k], rectangles))
                    {
                        cut = k;
                        break;
                    }
                }
                points.Add(linePoints[i]);
            }
        }

        #region Line
        for (int j = 0; j < points.Count - 1; j++)
        {
            if (points[j].X != points[j + 1].X && points[j].Y != points[j + 1].Y)
            {
                ConnectorOrientation orientationFrom;
                ConnectorOrientation orientationTo;

                // orientation from point
                if (j == 0)
                    orientationFrom = sourceOrientation;
                else
                    orientationFrom = GetOrientation(points[j], points[j - 1]);

                // orientation to pint 
                if (j == points.Count - 2)
                    orientationTo = sinkOrientation;
                else
                    orientationTo = GetOrientation(points[j + 1], points[j + 2]);


                if ((orientationFrom == ConnectorOrientation.Left || orientationFrom == ConnectorOrientation.Right) &&
                    (orientationTo == ConnectorOrientation.Left || orientationTo == ConnectorOrientation.Right))
                {
                    double centerX = Math.Min(points[j].X, points[j + 1].X) + Math.Abs(points[j].X - points[j + 1].X) / 2;
                    points.Insert(j + 1, new Point(centerX, points[j].Y));
                    points.Insert(j + 2, new Point(centerX, points[j + 2].Y));
                    if (points.Count - 1 > j + 3)
                        points.RemoveAt(j + 3);
                    return points;
                }

                if ((orientationFrom == ConnectorOrientation.Top || orientationFrom == ConnectorOrientation.Bottom) &&
                    (orientationTo == ConnectorOrientation.Top || orientationTo == ConnectorOrientation.Bottom))
                {
                    double centerY = Math.Min(points[j].Y, points[j + 1].Y) + Math.Abs(points[j].Y - points[j + 1].Y) / 2;
                    points.Insert(j + 1, new Point(points[j].X, centerY));
                    points.Insert(j + 2, new Point(points[j + 2].X, centerY));
                    if (points.Count - 1 > j + 3)
                        points.RemoveAt(j + 3);
                    return points;
                }

                if ((orientationFrom == ConnectorOrientation.Left || orientationFrom == ConnectorOrientation.Right) &&
                    (orientationTo == ConnectorOrientation.Top || orientationTo == ConnectorOrientation.Bottom))
                {
                    points.Insert(j + 1, new Point(points[j + 1].X, points[j].Y));
                    return points;
                }

                if ((orientationFrom == ConnectorOrientation.Top || orientationFrom == ConnectorOrientation.Bottom) &&
                    (orientationTo == ConnectorOrientation.Left || orientationTo == ConnectorOrientation.Right))
                {
                    points.Insert(j + 1, new Point(points[j].X, points[j + 1].Y));
                    return points;
                }
            }
        }
        #endregion

        return points;
    }

    private static ConnectorOrientation GetOrientation(Point p1, Point p2)
    {
        if (p1.X == p2.X)
        {
            if (p1.Y >= p2.Y)
                return ConnectorOrientation.Bottom;
            else
                return ConnectorOrientation.Top;
        }
        else if (p1.Y == p2.Y)
        {
            if (p1.X >= p2.X)
                return ConnectorOrientation.Right;
            else
                return ConnectorOrientation.Left;
        }
        throw new Exception("Failed to retrieve orientation");
    }

    private static Point GetNearestNeighborSource(ConnectorOrientation orientation, Point endPoint, Rect rectSource, Rect rectSink, out bool flag, bool isInnerPoint)
    {
        Point n1, n2; // neighbors
        GetNeighborCorners(orientation, rectSource, out n1, out n2, isInnerPoint);

        if (rectSink.Contains(n1))
        {
            flag = false;
            return n2;
        }

        if (rectSink.Contains(n2))
        {
            flag = true;
            return n1;
        }

        if ((Distance(n1, endPoint) <= Distance(n2, endPoint)))
        {
            flag = true;
            return n1;
        }
        else
        {
            flag = false;
            return n2;
        }
    }

    private static Point GetNearestNeighborSource(ConnectorOrientation orientation, Point endPoint, Rect rectSource, out bool flag, bool isInnerPoint)
    {
        Point n1, n2; // neighbors
        GetNeighborCorners(orientation, rectSource, out n1, out n2, isInnerPoint);

        if ((Distance(n1, endPoint) <= Distance(n2, endPoint)))
        {
            flag = true;
            return n1;
        }
        else
        {
            flag = false;
            return n2;
        }
    }

    private static Point GetNearestVisibleNeighborSink(Point currentPoint, Point endPoint, ConnectorOrientation orientation, Rect rectSource, Rect rectSink)
    {
        Point s1, s2; // neighbors on sink side
        GetNeighborCorners(orientation, rectSink, out s1, out s2);

        bool flag1 = IsPointVisible(currentPoint, s1, new Rect[] { rectSource, rectSink });
        bool flag2 = IsPointVisible(currentPoint, s2, new Rect[] { rectSource, rectSink });

        if (flag1) // s1 visible
        {
            if (flag2) // s1 and s2 visible
            {
                if (rectSink.Contains(s1))
                    return s2;

                if (rectSink.Contains(s2))
                    return s1;

                if ((Distance(s1, endPoint) <= Distance(s2, endPoint)))
                    return s1;
                else
                    return s2;

            }
            else
            {
                return s1;
            }
        }
        else // s1 not visible
        {
            if (flag2) // only s2 visible
            {
                return s2;
            }
            else // s1 and s2 not visible
            {
                return new Point(double.NaN, double.NaN);
            }
        }
    }

    private static bool IsPointVisible(Point fromPoint, Point targetPoint, Rect[] rectangles)
    {
        foreach (Rect rect in rectangles)
        {
            if (RectangleIntersectsLine(rect, fromPoint, targetPoint))
                return false;
        }
        return true;
    }

    private static bool IsRectVisible(Point fromPoint, Rect targetRect, Rect[] rectangles)
    {
        if (IsPointVisible(fromPoint, targetRect.TopLeft, rectangles))
            return true;

        if (IsPointVisible(fromPoint, targetRect.TopRight, rectangles))
            return true;

        if (IsPointVisible(fromPoint, targetRect.BottomLeft, rectangles))
            return true;

        if (IsPointVisible(fromPoint, targetRect.BottomRight, rectangles))
            return true;

        return false;
    }

    private static bool RectangleIntersectsLine(Rect rect, Point startPoint, Point endPoint)
    {
        //rect.Inflate(-1, -1);
        rect.Inflate(-1);
        //return rect.IntersectsWith(new Rect(startPoint, endPoint));
        return rect.Intersects(new Rect(startPoint, endPoint));
    }

    private static void GetOppositeCorners(ConnectorOrientation orientation, Rect rect, out Point n1, out Point n2, bool isInnerPoint = false)
    {
        if (isInnerPoint)
        {
            //n1 = rect.Location; n2 = rect.Location;
            n1 = rect.Position; n2 = rect.Position;
            return;
        }
        switch (orientation)
        {
            case ConnectorOrientation.Left:
                n1 = rect.TopRight; n2 = rect.BottomRight;
                break;
            case ConnectorOrientation.Top:
                n1 = rect.BottomLeft; n2 = rect.BottomRight;
                break;
            case ConnectorOrientation.Right:
                n1 = rect.TopLeft; n2 = rect.BottomLeft;
                break;
            case ConnectorOrientation.Bottom:
                n1 = rect.TopLeft; n2 = rect.TopRight;
                break;
            default:
                throw new Exception("No opposite corners found!");
        }
    }

    private static void GetNeighborCorners(ConnectorOrientation orientation, Rect rect, out Point n1, out Point n2, bool isInnerPoint = false)
    {
        if (isInnerPoint)
        {
            //n1 = rect.Location; n2 = rect.Location;
            n1 = rect.Position; n2 = rect.Position;
            return;
        }
        switch (orientation)
        {
            case ConnectorOrientation.Left:
                n1 = rect.TopLeft; n2 = rect.BottomLeft;
                break;
            case ConnectorOrientation.Top:
                n1 = rect.TopLeft; n2 = rect.TopRight;
                break;
            case ConnectorOrientation.Right:
                n1 = rect.TopRight; n2 = rect.BottomRight;
                break;
            case ConnectorOrientation.Bottom:
                n1 = rect.BottomLeft; n2 = rect.BottomRight;
                break;
            default:
                //throw new Exception("No neighour corners found!");
                n1 = rect.TopLeft; n2 = rect.BottomLeft;//ToDo
                break;
        }
    }

    private static double Distance(Point p1, Point p2)
    {
        //return Point.Subtract(p1, p2).Length;
        return Point.Distance(p1, p2);
    }

    private static Rect GetRectWithMargin(Point point, double margin)
    {
        Rect rect = new Rect(point.X, point.Y, 0, 0);
        //rect.Inflate(margin, margin);
        rect.Inflate(margin);

        return rect;
    }

    private static Point GetOffsetPoint(Point point, ConnectorOrientation orientation, Rect rect, bool isInnerPoint = false)
    {
        Point offsetPoint = new Point();
        if (isInnerPoint)
        {
            offsetPoint = new Point(point.X, point.Y);
            return offsetPoint;
        }

        switch (orientation)
        {
            case ConnectorOrientation.Left:
                offsetPoint = new Point(rect.Left, point.Y);
                break;
            case ConnectorOrientation.Top:
                offsetPoint = new Point(point.X, rect.Top);
                break;
            case ConnectorOrientation.Right:
                offsetPoint = new Point(rect.Right, point.Y);
                break;
            case ConnectorOrientation.Bottom:
                offsetPoint = new Point(point.X, rect.Bottom);
                break;
            default:
                break;
        }

        return offsetPoint;
    }

    private static ConnectorOrientation GetOpositeOrientation(ConnectorOrientation connectorOrientation)
    {
        switch (connectorOrientation)
        {
            case ConnectorOrientation.Left:
                return ConnectorOrientation.Right;
            case ConnectorOrientation.Top:
                return ConnectorOrientation.Bottom;
            case ConnectorOrientation.Right:
                return ConnectorOrientation.Left;
            case ConnectorOrientation.Bottom:
                return ConnectorOrientation.Top;
            default:
                return ConnectorOrientation.Top;
        }
    }
}
