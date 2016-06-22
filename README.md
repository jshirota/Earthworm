A simple .NET assembly that provides an object-relational mapping abstraction layer for geodatabase feature classes and tables. It converts features and rows into a (lazy) sequence of strongly-typed objects.

*You will need ArcGIS Desktop, Engine or Server 10.0, 10.1, 10.2, 10.3 or 10.4.

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
