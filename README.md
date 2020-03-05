A simple .NET assembly that provides an object-relational mapping abstraction layer for geodatabase feature classes and tables. It converts features and rows into a (lazy) sequence of strongly-typed objects.

*You will need ArcGIS Desktop, Engine or Server 10.0, 10.1, 10.2, 10.3, 10.4, 10.5, 10.6, 10.7 or 10.8.

The ClickOnce installer is here:

http://jshirota.github.io/Earthworm/ORMappingDeploy.application

The project page is here:

http://jshirota.github.io/Earthworm/

The library reference is here:

http://jshirota.github.io/Earthworm/Help/

The NuGet package is here:

http://nuget.org/packages/Earthworm

##Examples

This update uses the update cursor.
```c#
foreach (var city in cityFeatureClass.Map<City>(useUpdateCursor: true))
{
    city.POP2000 += 1;
    city.Update();
}
```

If you use the following static imports, you can define geometries much more succinctly.
```c#
using static Earthworm.Shape;
```
For example,
```c#
var state = new State
{
    STATE_NAME = "Test",
    Shape = Polygon(
        OuterRing(P(0, 0), P(0, 3), P(3, 3), P(3, 0)),
        InnerRing(P(1, 1), P(1, 2), P(2, 2), P(2, 1))
        )
};

state.ToKml().Save("test.kml");
```

This inserts a new record.
```c#
var toronto = new City
{
    AREANAME = "Toronto",
    POP2000 = 123,
    Shape = P(-79.5, 43.6)
};

toronto.InsertInto(cityFeatureClass);
```

This also inserts records.  If you have many records to insert, this is more effective because all of the records are inserted via an inert cursor.
```c#
cityFeatureClass.Insert(manyCities);
```

Sometimes you have to dynamically access a field by name.  That's OK.  We support editing data that way, too.
```c#
city["hidden_field"] = 1234;
city.Update();
```

You need a workspace edit session?  No problem.
```c#
try
{
    workspace.Edit(() =>
    {
        foreach (var city in cityFeatureClass.Map<City>(useUpdateCursor: true))
        {
            city.POP2000 = 0;
            city.Update();
        }

        throw new Exception("Dummy error!");
    });
}
catch (Exception ex)
{
    Console.WriteLine("Error occurred and rolled back.  " + ex.Message);
}
```

This returns an XElement.
```c#
toronto.ToKml()
```
So does this.
```C#
toronto.Shape.ToKml()
```

Many overloads available for KML, which I will discuss later.
```c#
cityFeatureClass.Map<City>().ToKml().Save("doc.kml");
```

Let's say you have a Windows Forms UI like this.
```c#
var cities = cityFeatureClass.Map<City>().ToList();

var f = new Form();
f.Controls.Add(new DataGridView
{
    DataSource = new BindingSource { DataSource = cities },
    Dock = DockStyle.Fill
});

Application.Run(f);
```

If you change an attribute value, IsDirty becomes true.  This is because the mapped objects raise the ProperyChanged event on its own.

This creates a new feature class in inserts cities with more than 1 million people into it.
```c#
var cityFeatureClass = featureWorkspace.OpenFeatureClass("Cities");

featureWorkspace.CreateFeatureClass<City>("Cities2", esriGeometryType.esriGeometryPoint, 4326)
    .Insert(cityFeatureClass.Map<City>(new QueryFilter { WhereClause = "POP2000>1000000" }));
```

Earthworm.SpatialAnalyst is a separate NuGet package.  Most raster algebra expressions and operators work just like ArcINFO GRID.  Yeah!

```c#
var dem = new Grid("n52_w128_1arc_v3.bil");
var dem_feet = dem * 3.28084;
```

Using static imports, Hillshade can be called like this.
```c#
var hillshade = Hillshade(dem, zFactor: 0.00001);
```

Or via an extension method like this.
```c#
var hillshade = dem.Hillshade(zFactor: 0.00001);
hillshade.Save("hillshade.png");
```

Here's an example of "Game of Life" procedure using Earthworm.SpatialAnalyst.
```c#
static Grid Tick(Grid grid)
{
    var count = FocalSum(grid, 3, 3) - grid;
    return Con(count == 3 | (grid == 1 & count == 2), 1, 0);
}
```
