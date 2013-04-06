Earthworm - An ORM for Esri Geodatabase (via ArcObjects)
=========

A simple .NET assembly that provides an object-relational mapping abstraction layer for geodatabase feature classes and tables. It converts features and rows into a (lazy) sequence of strongly-typed objects.

*You will need ArcGIS Desktop, Engine or Server 9.3.1, 10.0, 10.1 or 10.2.

[![A basic example](https://raw.github.com/jshirota/Earthworm/gh-pages/images/screenshot.png "Click here to start!")]
(http://jshirota.github.com/Earthworm/ORMappingDeploy.application)

-Fights the impedance mismatch between geodatabase tables and OOP.

-Does not get in the way of existing ArcObjects code.  It is basically the same as using ICursor.

-Works with Desktop ArcObjects, Addin, SOE and Engine applications.

-Does not require any auto-generated code.  No XML mapping files.  No dependency on the IDE.

-Supports CRUD operations with intuitive syntax.

-Supports runtime "code first" methods for creating feature classes and tables based on defined types.

-Respects all geodatabase behaviours such as versions, edit sessions, etc.

-Works with anything that implements IFeatureClass or ITable (i.e. shapefiles, coverages).  Note: The GUI tool only works with SDE, file and personal geodatabases.

-Returns mapped objects as a lazy sequence implementing IEnumerable\<T\>.  Allows for LINQ queries.  Facilitates functional programming.

-Encapsulates low-level ArcObjects "ceremony code" such as the cursor cleanup code.

-Provides JSON serialization/deserialization compatible with ArcGIS REST API.

-Provides KML serialization for geometries.

-Enables common spatial operations and predicates as extension methods for IGeometry to avoid verbose interface casting code.

-Provides class code generation for meta programming (i.e. code generation tools or runtime compilation via CodeDom or Roslyn).  Supports C# and Visual Basic.

-All mapped classes implement INotifyPropertyChanged (for use with WPF).  All mapped properties automatically raise the PropertyChanged event upon property setter access (via IL weaving) without using an AOP library.

-Acts as a download client for ArcGIS REST endpoints.  Continuously downloads and yields features mapped to defined types, which can be immediately inserted into a feature class.

The project page is here:

http://jshirota.github.com/Earthworm/

The library reference is here:

http://jshirota.github.com/Earthworm/Help/Index.html

The NuGet package is here:

http://nuget.org/packages/Earthworm
