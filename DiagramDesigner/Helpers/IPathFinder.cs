using Avalonia;
using System.Collections.Generic;

namespace DiagramDesigner;

public interface IPathFinder
{
    List<Point> GetConnectionLine(ConnectorInfo source, ConnectorInfo sink, bool showLastLine);
    List<Point> GetConnectionLine(ConnectorInfo source, Point sinkPoint, ConnectorOrientation preferredOrientation);
}
