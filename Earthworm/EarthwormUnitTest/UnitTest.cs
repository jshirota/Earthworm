using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Earthworm;
using Earthworm.AO;
using Earthworm.Meta;
using Earthworm.Serialization;
using ESRI.ArcGIS;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using NUnit.Framework;

namespace EarthwormUnitTest
{
    [TestFixture, RequiresSTA]
    public class UnitTest
    {
        private AoInitialize _ao;
        private IWorkspace _workspace;
        private IFeatureWorkspace _featureWorkspace;
        private IFeatureClass _cityFC;
        private IFeatureClass _countyFC;
        private IFeatureClass _highwayFC;
        private IFeatureClass _stateFC;

        #region SetUp/TearDown

        private static void CopyDirectory(string sourcePath, string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            foreach (string file in Directory.GetFiles(sourcePath))
            {
                string dest = System.IO.Path.Combine(destPath, System.IO.Path.GetFileName(file));
                File.Copy(file, dest);
            }

            foreach (string folder in Directory.GetDirectories(sourcePath))
            {
                string dest = System.IO.Path.Combine(destPath, System.IO.Path.GetFileName(folder));
                CopyDirectory(folder, dest);
            }
        }

        [TestFixtureSetUp]
        public void Init()
        {
            RuntimeManager.Bind(ProductCode.Desktop);

            _ao = new AoInitialize();

            foreach (esriLicenseProductCode licenseCode in new int[] { 40, 50, 60 })
            {
                try { _ao.Initialize(licenseCode); }
                catch { }
            }

            if (!Directory.Exists("Temp"))
                Directory.CreateDirectory("Temp");

            IWorkspaceFactory workspaceFactory;

            string name = @"Temp\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".gdb";
            CopyDirectory(@"Data\Data.gdb", name);

            workspaceFactory = new FileGDBWorkspaceFactory();
            _workspace = workspaceFactory.OpenFromFile(name, 0);

            _featureWorkspace = _workspace as IFeatureWorkspace;
            _cityFC = _featureWorkspace.OpenFeatureClass("Cities");
            _countyFC = _featureWorkspace.OpenFeatureClass("Counties");
            _highwayFC = _featureWorkspace.OpenFeatureClass("Highways");
            _stateFC = _featureWorkspace.OpenFeatureClass("States");
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            _ao.Shutdown();

            if (Directory.Exists("Temp"))
            {
                foreach (string folder in Directory.GetDirectories("Temp"))
                {
                    try
                    {
                        Directory.Delete(folder, true);
                    }
                    catch
                    {
                    }
                }

                foreach (string file in Directory.GetFiles("Temp"))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                    }
                }
            }
        }

        #endregion

        #region Stable

        private bool CloseEnough(double number1, double number2)
        {
            return Math.Abs(number1 - number2) < 0.001;
        }

        [Test]
        public void _001_FindNewYorkViaQueryFilter()
        {
            IQueryFilter filter = new QueryFilter { WhereClause = "AREANAME='New York'" };
            Assert.AreEqual(1, _cityFC.Map<City>(filter).Count());
        }

        [Test]
        public void _002_FindNewYorkViaLambda()
        {
            Assert.AreEqual(1, _cityFC.Map<City>().Count(c => c.AREANAME == "New York"));
        }

        [Test]
        public void _003_CityCount()
        {
            Assert.AreEqual(3557, _cityFC.Map<City>().Count());
        }

        [Test]
        public void _004_CountyCount()
        {
            Assert.AreEqual(3141, _countyFC.Map<County>().Count());
        }

        [Test]
        public void _005_HighwayCount()
        {
            Assert.AreEqual(679, _highwayFC.Map<Highway>().Count());
        }

        [Test]
        public void _006_StateCount()
        {
            Assert.AreEqual(51, _stateFC.Map<State>().Count());
        }

        [Test]
        public void _007_DistanceInLambert()
        {
            List<City> cities = _cityFC.Map<City>().OrderBy(c => c.AREANAME).ToList();

            City city1 = cities.First();
            City city2 = cities.Last();

            double d = city1.Shape.DistanceTo(city2.Shape);

            Assert.AreEqual(true, CloseEnough(13.197074309827412d, d));
        }

        [Test]
        public void _008_TotalAreaInAlbers()
        {
            double a = (from c in _countyFC.Map<County>()
                        where c.STATE_NAME == "New York"
                        select ((IArea)c.Shape).Area).Sum();

            double b = ((IArea)_stateFC.Map<State>().Single(s => s.STATE_NAME == "New York").Shape).Area;


            Assert.AreEqual(a, b);
        }

        [Test]
        public void _009_California()
        {
            var a = from county in _countyFC.Map<County>()
                    where county.STATE_NAME == "California"
                    select county.Shape.Project2(102008);

            var b = a.Aggregate((g1, g2) => g1.Union(g2));

            var c = _cityFC.Map<City>().Where(city => city.Shape.Project2(102008).Intersects(b));

            var cal = _stateFC.Map<State>().Single(s => s.STATE_NAME == "California").Shape;
            var d = _cityFC.Map<City>().Where(city => city.Shape.Intersects(cal));

            Assert.AreEqual(c.Count(), d.Count());
        }

        [Test]
        public void _010_Insert()
        {
            City city = new City
                {
                    AREANAME = "Toronto",
                    CAPITAL = "N",
                    CLASS = "city",
                    POP2000 = 4000000,
                    Shape = new Point { X = -80, Y = 50 },
                    ST = "ON"
                };

            city.InsertInto(_cityFC);

            Assert.AreEqual(3558, _cityFC.Map<City>().Count());
        }

        [Test]
        public void _011_Update()
        {
            City toronto = _cityFC.Map<City>().Single(c => c.AREANAME == "Toronto");
            toronto.POP2000 = null;
            toronto.Update();
        }

        [Test]
        public void _012_Cleanup()
        {
            IEnumerable<City> citiesToDelete = _cityFC.Map<City>().Where(c => c.OID > 3557);

            foreach (City city in citiesToDelete)
            {
                city.Delete();
            }

            Assert.AreEqual(0, citiesToDelete.Count());
        }

        [Test]
        public void _013_Create()
        {
            IFeatureClass featureClass = _featureWorkspace.CreateFeatureClass<City>("City123", esriGeometryType.esriGeometryPoint, 4326);

            Assert.AreEqual("City123", featureClass.AliasName);
        }

        [Test]
        public void _014_Drop()
        {
            IFeatureClass featureClass = _featureWorkspace.OpenFeatureClass("City123");

            ((IDataset)featureClass).Delete();

            Assert.AreEqual(0, 0);
        }

        [Test]
        public void _015_Create()
        {
            IFeatureClass featureClass = _featureWorkspace.CreateFeatureClass<City>("Cities100", esriGeometryType.esriGeometryPoint, 4326);

            for (int i = 0; i < 100; i++)
            {
                City city = new City
                    {
                        AREANAME = "Hello",
                        POP2000 = 123,
                        Shape = new Point { X = -80, Y = 50 }
                    };

                city.InsertInto(featureClass);
            }

            Assert.AreEqual(100, featureClass.FeatureCount(null));
        }

        [Test]
        public void _016_Create()
        {
            IFeatureClass featureClass = _featureWorkspace.CreateFeatureClass<City>("Cities75", esriGeometryType.esriGeometryPoint, 4326);

            for (int i = 0; i < 100; i++)
            {
                City city = new City
                {
                    AREANAME = "Hello",
                    POP2000 = 123,
                    Shape = new Point { X = -80, Y = 50 }
                };

                city.InsertInto(featureClass);
            }

            List<City> cities = featureClass.Map<City>().Take(25).ToList();

            foreach (City city in cities)
                city.Delete();

            Assert.AreEqual(75, featureClass.FeatureCount(null));
        }

        [Test]
        public void _017_Create()
        {
            IFeatureClass featureClass = _featureWorkspace.CreateFeatureClass<County>("Circle",
                                                                                     esriGeometryType.esriGeometryPolygon,
                                                                                     4326);

            IPoint point = new Point { X = -80, Y = 50, SpatialReference = new SpatialReferenceEnvironment().CreateGeographicCoordinateSystem(4326) };

            List<County> counties = Enumerable.Range(1, 10).Select(n => new County
            {
                NAME = "Hello123",
                POP2000 = 123,
                Shape = point.Buffer(n)
            }).ToList();

            foreach (County county in counties)
                county.InsertInto(featureClass);

            double areaSum = featureClass.Map<County>().Sum(c => ((IArea)c.Shape).Area);

            double expected = Enumerable.Range(1, 10).Sum(n => Math.PI * Math.Pow(n, 2));

            Assert.True(Math.Abs(expected - areaSum) < 0.1);
        }

        [Test]
        public void _018_Equality()
        {
            City c1 = new City();
            City c2 = new City();

            Assert.AreEqual(true, c1.ValueEquals(c2));

            c1.AREANAME = "abc";
            Assert.AreEqual(false, c1.ValueEquals(c2));

            c2.AREANAME = "abc";
            Assert.AreEqual(true, c1.ValueEquals(c2));

            c1.Shape = new Point { X = -80, Y = 50 };
            Assert.AreEqual(false, c1.ValueEquals(c2));

            c2.Shape = new Point { X = -80, Y = 50 };
            Assert.AreEqual(true, c1.ValueEquals(c2));

            c2.Shape = new Point { X = -80, Y = 50.001 };
            Assert.AreEqual(false, c1.ValueEquals(c2));

            c1.Shape = new Point { X = -80, Y = 50.001 };
            Assert.AreEqual(true, c2.ValueEquals(c1));

            c1.Shape = null;
            c2.Shape = null;

            Assert.AreEqual(true, c2.ValueEquals(c1));

            c1.AREANAME = null;
            Assert.AreEqual(false, c1.ValueEquals(c2));

            c2.AREANAME = null;
            Assert.AreEqual(true, c1.ValueEquals(c2));
        }

        [Test]
        public void _019_WithinDistanceVSFindDistanceTo()
        {
            IFeatureClass featureClass = _featureWorkspace.OpenFeatureClass("Counties");

            List<County> counties = featureClass.Map<County>().ToList();

            IPoint p = new Point { X = -80, Y = 30 };

            for (int i = 0; i < 40; i++)
            {
                Random r = new Random(i);

                double d = r.NextDouble() * 10;

                List<County> within3a = counties.Where(c => p.DistanceTo(c.Shape) < d).ToList();
                List<County> within3b = counties.Where(c => p.Within(c.Shape, d)).ToList();

                Assert.AreEqual(within3a.Count, within3b.Count);
            }
        }

        [Test]
        public void _020_Equality()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass("Cities");

            IFeatureClass fc2 = _featureWorkspace.CreateFeatureClass<City>("CitiesABC", fc.ShapeType, ((IGeoDataset)fc).SpatialReference, true);

            foreach (City city in fc.Map<City>().Take(500))
                city.InsertInto(fc2);

            IEnumerable<City> cities = fc2.Map<City>().Take(400);

            var c = new MappableFeatureComparer<City>();

            var enumerable = cities as City[] ?? cities.ToArray();
            Assert.AreEqual(400, enumerable.Distinct(c).Count());

            int i = 0;
            foreach (City city in enumerable)
            {
                city.AREANAME = "Toronto";
                city.POP2000 = 30000;
                city.CAPITAL = "N";
                city.ST = null;
                city.CLASS = null;
                city.Province = "Ontario" + i;
                city.Shape = new Point { X = -80, Y = 45 };
                city.Update();
                i++;
            }

            Assert.AreEqual(1, enumerable.Distinct(c).Count());

            i = 0;
            foreach (City city in enumerable)
            {
                city.AREANAME = "Toronto";
                city.POP2000 = 30000;
                city.CAPITAL = "N";
                city.ST = null;
                city.CLASS = null;
                city.Province = "Ontario" + i;
                city.Shape = null;
                city.Update();
                i++;
            }

            Assert.AreEqual(1, enumerable.Distinct(c).Count());

            i = 0;
            foreach (City city in enumerable)
            {
                city.AREANAME = "Toronto";
                city.POP2000 = i > 100 ? 1 : 0;
                city.CAPITAL = "N";
                city.ST = null;
                city.CLASS = null;
                city.Province = "Ontario";
                city.Shape = null;
                city.Update();
                i++;
            }

            Assert.AreEqual(2, enumerable.Distinct(c).Count());
        }

        [Test]
        public void _021_JSON()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2("US_Major_Cities");

            List<CityWithBlob> cities1 = fc.Map<CityWithBlob>().ToList();
            List<CityWithBlob> cities2 = cities1.Select(c => Json.Deserialize<CityWithBlob>(c.ToString(true))).ToList();

            Assert.IsTrue(cities1.SequenceEqual(cities2, new MappableFeatureComparer<CityWithBlob>()));
        }

        [Test]
        public void _037_WorkspaceEdit()
        {
            IWorkspaceEdit edit = (IWorkspaceEdit)_workspace;
            edit.StartEditing(false);

            _workspace.Edit(() =>
                {

                });

            Assert.AreEqual(true, edit.IsBeingEdited());

            IWorkspaceEdit edit2 = _workspace as IWorkspaceEdit;
            Assert.AreEqual(true, edit2.IsBeingEdited());

            edit.StopEditing(true);
            Assert.AreEqual(false, edit2.IsBeingEdited());

            edit.StartEditing(true);
            Assert.AreEqual(true, edit2.IsBeingEdited());

            edit.StopEditing(true);
            Assert.AreEqual(false, edit2.IsBeingEdited());

            edit.StopEditing(true);
            Assert.AreEqual(false, edit2.IsBeingEdited());

            edit2.StartEditing(true);
            Assert.AreEqual(true, edit.IsBeingEdited());

            edit2.StartEditing(true);
            Assert.AreEqual(true, edit.IsBeingEdited());

            edit2.StopEditing(true);
            Assert.AreEqual(false, edit.IsBeingEdited());
        }

        [Test]
        public void _038_WorkspaceEdit()
        {
            IWorkspaceEdit edit = _workspace as IWorkspaceEdit;
            edit.StartEditing(false);

            bool success = _workspace.Edit(() =>
            {

            });

            edit.StopEditing(success);

            Assert.AreEqual(true, success);
        }

        [Test]
        public void _039_WorkspaceEdit()
        {
            IWorkspaceEdit edit = _workspace as IWorkspaceEdit;
            edit.StartEditing(false);

            bool success = _workspace.Edit(() =>
            {
                throw new Exception("!!!");
            });

            edit.StopEditing(success);

            Assert.AreEqual(false, success);
            Assert.AreEqual(false, edit.IsBeingEdited());
        }

        [Test]
        public void _040_WorkspaceEdit()
        {
            IWorkspaceEdit edit = _workspace as IWorkspaceEdit;

            bool success = _workspace.Edit(() =>
            {
                throw new Exception("!!!");
            });

            Assert.AreEqual(false, success);
            Assert.AreEqual(false, edit.IsBeingEdited());
        }

        [Test]
        public void _041_WorkspaceEdit()
        {
            IWorkspaceEdit edit = _workspace as IWorkspaceEdit;

            bool success = _workspace.Edit(() =>
            {
                try
                {
                    throw new Exception("!!!");
                }
                catch { }
            });

            Assert.AreEqual(true, success);
            Assert.AreEqual(false, edit.IsBeingEdited());
        }

        [Test]
        public void _042_WorkspaceEdit()
        {
            bool success = _workspace.Edit(() =>
            {
                foreach (City city in _cityFC.Map<City>())
                {
                    if (city.OID == 300)
                    {
                        city.POP2000 = null;
                        city.ST = "ON";
                        city.AREANAME = "Toronto";
                        city.Update();
                    }
                }
            });

            Assert.AreEqual(true, success);
            Assert.AreEqual("Toronto", _cityFC.FindItemByOID<City>(300).AREANAME);
        }

        [Test]
        public void _043_WorkspaceEdit()
        {
            Exception ex;
            bool success = _workspace.Edit(() =>
            {
                foreach (City city in _cityFC.Map<City>())
                {
                    if (city.OID == 400)
                    {
                        city.POP2000 = null;
                        city.ST = "ON";
                        city.AREANAME = "Toronto";
                        city.Update();
                    }
                }

                throw new Exception("!!!");
            }, out ex);

            Assert.AreEqual(false, success);
            Assert.AreEqual("Montebello", _cityFC.FindItemByOID<City>(400).AREANAME);
            Assert.AreEqual("!!!", ex.Message);
        }

        [Test]
        public void _044_WorkspaceEdit()
        {
            IWorkspaceEdit edit = _workspace as IWorkspaceEdit;
            edit.StartEditing(false);

            Exception ex;

            bool success = _workspace.Edit(() =>
            {
                foreach (City city in _cityFC.Map<City>())
                {
                    if (city.OID == 500)
                    {
                        city.POP2000 = null;
                        city.ST = "ON";
                        city.AREANAME = "Toronto";
                        city.Update();
                    }
                }

                throw new Exception("Error");
            }, out ex);

            Assert.AreEqual(true, edit.IsBeingEdited());

            edit.StopEditing(success);

            Assert.AreEqual(false, success);
            Assert.AreEqual("San Clemente", _cityFC.FindItemByOID<City>(500).AREANAME);
            Assert.AreEqual("Error", ex.Message);
            Assert.AreEqual(false, edit.IsBeingEdited());
        }

        [Test]
        public void _045_WorkspaceEdit()
        {
            IWorkspaceEdit edit = _workspace as IWorkspaceEdit;
            edit.StartEditing(false);

            Exception ex;

            bool success = _workspace.Edit(() =>
            {
                foreach (City city in _cityFC.Map<City>())
                {
                    city.Delete();
                }

                throw new Exception("Error");
            }, out ex);

            Assert.AreEqual(true, edit.IsBeingEdited());

            edit.StopEditing(success);

            Assert.AreEqual(false, success);
            Assert.AreEqual(3557, _cityFC.FeatureCount(null));
            Assert.AreEqual("Error", ex.Message);
            Assert.AreEqual(false, edit.IsBeingEdited());
        }

        [Test]
        public void _046_WorkspaceEdit()
        {
            IWorkspaceEdit edit = _workspace as IWorkspaceEdit;
            edit.StartEditing(false);

            Exception ex;

            bool success = _workspace.Edit(() =>
            {
                foreach (City city in _cityFC.Map<City>().Skip(200))
                {
                    city.Delete();
                }
            }, out ex);

            Assert.AreEqual(true, edit.IsBeingEdited());

            edit.StopEditing(success);

            Assert.AreEqual(true, success);
            Assert.AreEqual(200, _cityFC.FeatureCount(null));
            Assert.AreEqual(false, edit.IsBeingEdited());
        }

        #endregion

        #region ReadWrite

        [Test]
        public void _048_Blob()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2("US_Major_Cities");

            List<CityWithBlob> cities = fc.Map<CityWithBlob>().ToList();

            cities[10].Override = File.ReadAllBytes(@"Data\logo3w.png");
            cities[10].Update();

            Assert.AreEqual(7007, cities[10].Override.Length);

            Assert.AreEqual(false, cities.All(c => c.Override == null));

            CityWithBlob city = cities[10];
            city.Override = null;
            city.Update();

            Assert.AreEqual(true, cities.All(c => c.Override == null));
        }

        [Test]
        public void _049_Blob()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2("US_Major_Cities");

            IFeatureClass fc2 = _featureWorkspace.CreateFeatureClass<CityWithBlob>("Test123", fc.ShapeType, ((IGeoDataset)fc).SpatialReference, true);

            var seq1 = fc.Map<CityWithBlob>();
            var seq2 = fc2.Map<CityWithBlob>();

            var cityWithBlobs = seq1 as CityWithBlob[] ?? seq1.ToArray();
            foreach (CityWithBlob city in cityWithBlobs)
            {
                city.InsertInto(fc2);
            }

            Assert.AreEqual(true, cityWithBlobs.SequenceEqual(seq2, new MappableFeatureComparer<CityWithBlob>()));
        }

        [Test]
        public void _050_Blob()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2("US_Major_Cities");

            CityWithBlob city = fc.FindItemByOID<CityWithBlob>(20);

            Assert.AreEqual(city.OID, 20);

            city.Override = File.ReadAllBytes(@"Data\logo3w.png");
            city.Update();

            string path = "Temp\\" + DateTime.Now.ToString(@"yyyy-MM-dd-HH-mm-ss") + ".png";

            File.WriteAllBytes(path, city.Override);

            CityWithBlob city2 = fc.FindItemByOID<CityWithBlob>(21);

            city2.Override = File.ReadAllBytes(path);
            city2.Update();

            Assert.AreEqual(true, city.Override.SequenceEqual(city2.Override));
        }

        [Test]
        public void _055_GlobalID()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2("CitiesWithGlobalID");

            List<CityWithGlobalID> cities1 = fc.Map<CityWithGlobalID>().ToList();

            foreach (CityWithGlobalID city in cities1)
            {
                city.Update();
            }

            List<CityWithGlobalID> cities2 = fc.Map<CityWithGlobalID>().ToList();

            for (int i = 0; i < cities1.Count; i++)
            {
                Assert.IsTrue(cities1[i].ValueEquals(cities2[i]));
            }
        }

        private void ReadAndWrite<T>(string fcName) where T : MappableFeature, new()
        {
            int n = 0;
            int count = 300;

            try
            {
                IFeatureClass fc1 = _featureWorkspace.OpenFeatureClass(fcName);
                IFeatureClass fc2 = _featureWorkspace.CreateFeatureClass<T>(fcName + "_999", fc1.ShapeType, ((IGeoDataset)fc1).SpatialReference, true);

                foreach (T item in fc1.Map<T>().Take(1000))
                {
                    item.InsertInto(fc2);
                }

                var query1 = fc1.Map<T>().Skip(n).Take(count).ToList();
                List<T> query2 = fc2.Map<T>().Skip(n).Take(count).ToList();

                bool test = query1.SequenceEqual(query2, new MappableFeatureComparer<T>());

                Assert.AreEqual(true, test);
            }
            catch
            {
                ITable fc1 = _featureWorkspace.OpenTable(fcName);
                ITable fc2 = _featureWorkspace.CreateTable<T>(fcName + "_999", true);

                foreach (T item in fc1.Map<T>().Take(1000))
                {
                    item.InsertInto(fc2);
                }

                var query1 = fc1.Map<T>().Skip(n).Take(count).ToList();
                List<T> query2 = fc2.Map<T>().Skip(n).Take(count).ToList();

                bool test = query1.SequenceEqual(query2, new MappableFeatureComparer<T>());

                Assert.AreEqual(true, test);
            }
        }

        [Test]
        public void _057_ReadWrite()
        {
            ReadAndWrite<City>("Cities");
        }

        [Test]
        public void _058_ReadWrite()
        {
            ReadAndWrite<County>("Counties");
        }

        [Test]
        public void _059_ReadWrite()
        {
            ReadAndWrite<Highway>("Highways");
        }

        [Test]
        public void _060_ReadWrite()
        {
            ReadAndWrite<State>("States");
        }

        [Test]
        public void _062_ReadWrite()
        {
            ReadAndWrite<CityWithGUID>("CitiesWithGlobalID");
        }

        [Test]
        public void _064_Update()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass("CitiesWithGlobalID");

            var c = new MappableFeatureComparer<CityWithGUID>();

            List<CityWithGUID> cities = fc.Map<CityWithGUID>().ToList();

            Guid guid = Guid.NewGuid();

            foreach (CityWithGUID city in cities)
            {
                city.NAME = null;
                city.MyID = guid;
                city.RuleID = null;
                city.ST = "ON";
            }

            Assert.AreEqual(120, cities.Distinct(c).Count());

            foreach (CityWithGUID city in cities)
            {
                city.NAME = null;
                city.MyID = guid;
                city.RuleID = null;
                city.ST = "ON";
            }

            Assert.AreEqual(120, cities.Distinct(c).Count());

            foreach (CityWithGUID city in cities)
            {
                city.NAME = null;
                city.MyID = guid;
                city.RuleID = null;
                city.ST = "ON";
                city.Shape = new Point { X = -80, Y = 40 };
                city.ZIP = "12345";
            }

            Assert.AreEqual(1, cities.Distinct(c).Count());

            Assert.AreEqual(120, fc.Map<CityWithGUID>().Distinct(c).Count());

            foreach (CityWithGUID city in cities)
            {
                city.Update();
            }

            Assert.AreEqual(1, cities.Distinct(c).Count());

            Assert.AreEqual(1, fc.Map<CityWithGUID>().Distinct(c).Count());
        }

        [Test]
        public void _065_ReadWrite()
        {
            ReadAndWrite<Altname>("altname");
        }

        [Test]
        public void _066_ReadWrite()
        {
            ReadAndWrite<Capacitorbank>("capacitorbank");
        }

        [Test]
        public void _067_ReadWrite()
        {
            ReadAndWrite<Citieswithglobalid>("citieswithglobalid");
        }

        [Test]
        public void _068_ReadWrite()
        {
            ReadAndWrite<Customer>("customers");
        }

        [Test]
        public void _069_ReadWrite()
        {
            ReadAndWrite<ElectricnetworkNetJunction>("electricnetwork_net_junctions");
        }

        [Test]
        public void _070_ReadWrite()
        {
            ReadAndWrite<Feeder>("feeder");
        }

        [Test]
        public void _071_ReadWrite()
        {
            ReadAndWrite<Generator>("generator");
        }

        [Test]
        public void _072_ReadWrite()
        {
            ReadAndWrite<InsideDiagram>("inside_diagrams");
        }

        [Test]
        public void _073_ReadWrite()
        {
            ReadAndWrite<InsideLink>("inside_links");
        }

        [Test]
        public void _074_ReadWrite()
        {
            ReadAndWrite<InsideNode>("inside_nodes");
        }

        [Test]
        public void _075_ReadWrite()
        {
            ReadAndWrite<MetroEntrance>("metro_entrances");
        }

        [Test]
        public void _076_ReadWrite()
        {
            ReadAndWrite<MetroLine>("metro_lines");
        }

        [Test]
        public void _077_ReadWrite()
        {
            ReadAndWrite<MetroStation>("metro_stations");
        }

        [Test]
        public void _078_ReadWrite()
        {
            ReadAndWrite<MyfeaturedatasetNetJunction>("myfeaturedataset_net_junctions");
        }

        [Test]
        public void _079_ReadWrite()
        {
            ReadAndWrite<Openpoint>("openpoint");
        }

        [Test]
        public void _080_ReadWrite()
        {
            ReadAndWrite<Paristurn>("paristurns");
        }

        [Test]
        public void _081_ReadWrite()
        {
            ReadAndWrite<Pipe>("pipes");
        }

        [Test]
        public void _082_ReadWrite()
        {
            ReadAndWrite<Pipeattr>("pipeattrs");
        }

        [Test]
        public void _083_ReadWrite()
        {
            ReadAndWrite<PlaceAliase>("place_aliases");
        }

        [Test]
        public void _084_ReadWrite()
        {
            ReadAndWrite<Plant>("plants");
        }

        [Test]
        public void _085_ReadWrite()
        {
            ReadAndWrite<Plantequip>("plantequip");
        }

        [Test]
        public void _086_ReadWrite()
        {
            ReadAndWrite<Primaryline>("primaryline");
        }

        [Test]
        public void _087_ReadWrite()
        {
            ReadAndWrite<Primarymeter>("primarymeter");
        }

        [Test]
        public void _088_ReadWrite()
        {
            ReadAndWrite<Protectiondevicebank>("protectiondevicebank");
        }

        [Test]
        public void _089_ReadWrite()
        {
            ReadAndWrite<Regulatorbank>("regulatorbank");
        }

        [Test]
        public void _090_ReadWrite()
        {
            ReadAndWrite<Secondaryline>("secondaryline");
        }

        [Test]
        public void _091_ReadWrite()
        {
            ReadAndWrite<Servicelocation>("servicelocation");
        }

        [Test]
        public void _092_ReadWrite()
        {
            ReadAndWrite<Servicepoint>("servicepoint");
        }

        [Test]
        public void _093_ReadWrite()
        {
            ReadAndWrite<Statepopulation>("statepopulation");
        }

        [Test]
        public void _094_ReadWrite()
        {
            ReadAndWrite<Stepbank>("stepbank");
        }

        [Test]
        public void _095_ReadWrite()
        {
            ReadAndWrite<Street>("streets");
        }

        [Test]
        public void _096_ReadWrite()
        {
            ReadAndWrite<Streets1>("streets_1");
        }

        [Test]
        public void _097_ReadWrite()
        {
            ReadAndWrite<Streets2>("streets_2");
        }

        [Test]
        public void _098_ReadWrite()
        {
            ReadAndWrite<Substation>("substation");
        }

        [Test]
        public void _099_ReadWrite()
        {
            ReadAndWrite<Timsertool>("timsertool");
        }

        [Test]
        public void _100_ReadWrite()
        {
            ReadAndWrite<TransferStation>("transfer_stations");
        }

        [Test]
        public void _101_ReadWrite()
        {
            ReadAndWrite<TransferStreetStation>("transfer_street_station");
        }

        [Test]
        public void _102_ReadWrite()
        {
            ReadAndWrite<Transformerbank>("transformerbank");
        }

        [Test]
        public void _103_ReadWrite()
        {
            ReadAndWrite<UsaMajorHighway>("usa_major_highways");
        }

        [Test]
        public void _104_ReadWrite()
        {
            ReadAndWrite<UsMajorCities1>("us_major_cities_1");
        }

        [Test]
        public void _105_ReadWrite()
        {
            ReadAndWrite<Waterbody>("waterbody");
        }

        [Test]
        public void _106_ReadWrite()
        {
            ReadAndWrite<Watershed>("watershed");
        }

        [Test]
        public void _107_Distinct()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass("watershed");

            IEnumerable<Watershed> items = fc.Map<Watershed>();

            Assert.AreEqual(4, items.Distinct().Count());

            foreach (var item in items)
            {
                item.Delete();
            }

            Assert.AreEqual(0, items.Distinct().Count());
        }

        #endregion

        #region Collapse Me

        [Test]
        public void _108_Blob()
        {
            byte[] data1 = File.ReadAllBytes(@"Data\p1.png");
            byte[] data2 = File.ReadAllBytes(@"Data\p2.png");
            byte[] data3 = File.ReadAllBytes(@"Data\p3.png");

            Picture pic1 = new Picture { Name = "Test1", Data = data1 };
            Picture pic2 = new Picture { Name = "Test1", Data = data2 };
            Picture pic3 = new Picture { Name = "Test1", Data = data3 };
            Picture pic4 = new Picture { Name = "Test4", Data = data1 };

            Assert.AreEqual(true, pic1.ValueEquals(pic1));
            Assert.AreEqual(true, pic1.ValueEquals(pic2));
            Assert.AreEqual(false, pic1.ValueEquals(pic3));
            Assert.AreEqual(false, pic1.ValueEquals(pic4));
        }

        private void TestJson<T>(string fcName) where T : MappableFeature, new()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2(fcName);

            List<T> items1 = fc.Map<T>().ToList();
            List<T> items2 = items1.Select(c => Json.Deserialize<T>(c.ToString(true))).ToList();

            Assert.IsTrue(items1.SequenceEqual(items2, new MappableFeatureComparer<T>()));
        }

        private void TestJsonGeometry<T>(string fcName) where T : MappableFeature, new()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2(fcName);

            foreach (T item in fc.Map<T>().Take(50))
            {
                Assert.IsTrue(item.Shape.Equals2(item.Shape.ToJsonGeometry().ToEsriGeometry()));
            }
        }

        private void TestJsonPoint<T>(string fcName) where T : MappableFeature, new()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2(fcName);

            foreach (T item in fc.Map<T>().Take(50))
            {
                Assert.IsTrue(item.Shape.Equals2(JsonPoint.FromJson(item.Shape.ToJsonGeometry().ToString()).ToEsriGeometry()));
            }
        }

        private void TestJsonPolyline<T>(string fcName) where T : MappableFeature, new()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2(fcName);

            foreach (T item in fc.Map<T>().Take(50))
            {
                Assert.IsTrue(item.Shape.Equals2(JsonPolyline.FromJson(item.Shape.ToJsonGeometry().ToString()).ToEsriGeometry()));
            }
        }

        private void TestJsonPolygon<T>(string fcName) where T : MappableFeature, new()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2(fcName);

            foreach (T item in fc.Map<T>().Take(50))
            {
                Assert.IsTrue(item.Shape.Equals2(JsonPolygon.FromJson(item.Shape.ToJsonGeometry().ToString()).ToEsriGeometry()));
            }
        }

        [Test]
        public void _110_JSON()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2("Cities");

            List<City> cities1 = fc.Map<City>().ToList();
            List<City> cities2 = cities1.Select(c => Json.Deserialize<City>(c.ToString(true))).ToList();

            Assert.IsTrue(cities1.SequenceEqual(cities2, new MappableFeatureComparer<City>()));
        }

        [Test]
        public void _112_JSON()
        {
            TestJson<Capacitorbank>("capacitorbank");
        }

        //[Test]
        //public void _113_JSON()
        //{
        //    TestJson<Streets2>("streets_2");
        //}

        [Test]
        public void _114_JSON()
        {
            TestJson<CityWithGlobalID>("CitiesWithGlobalID");
        }

        [Test]
        public void _115_JSON()
        {
            TestJson<Plant>("plants");
        }

        [Test]
        public void _116_JSON()
        {
            TestJson<Highway>("Highways");
        }

        [Test]
        public void _117_JSON()
        {
            TestJson<MetroEntrance>("metro_entrances");
        }

        class Dog : MappableFeature
        {
            [MappedField("Name")]
            public string Name { get; set; }

            [MappedField("Picture")]
            public byte[] Picture { get; set; }
        }

        [Test]
        public void _118_JSONGeometry()
        {
            TestJsonGeometry<Capacitorbank>("capacitorbank");
        }

        [Test]
        public void _119_JSONGeometry()
        {
            TestJsonGeometry<CityWithGlobalID>("CitiesWithGlobalID");
        }

        [Test]
        public void _120_JSONGeometry()
        {
            TestJsonGeometry<Plant>("plants");
        }

        [Test]
        public void _121_JSONGeometry()
        {
            TestJsonGeometry<Highway>("Highways");
        }

        [Test]
        public void _122_JSONGeometry()
        {
            TestJsonGeometry<MetroEntrance>("metro_entrances");
        }

        [Test]
        public void _123_JSONGeometry()
        {
            TestJsonGeometry<State>("States");
        }

        [Test]
        public void _124_JSONGeometry2()
        {
            TestJsonPoint<Plant>("plants");
        }

        [Test]
        public void _125_JSONGeometry2()
        {
            TestJsonPolyline<Highway>("Highways");
        }

        [Test]
        public void _126_JSONGeometry2()
        {
            TestJsonPoint<MetroEntrance>("metro_entrances");
        }

        [Test]
        public void _127_JSONGeometry2()
        {
            TestJsonPolygon<State>("States");
        }

        public class City5 : City
        {
            public override string AREANAME
            {
                get
                {
                    return base.AREANAME;
                }
                set
                {
                    if (value != base.AREANAME)
                    {
                        base.AREANAME = value;
                        RaisePropertyChanged("AREANAME");
                    }
                }
            }

            public override IGeometry Shape
            {
                get
                {
                    return base.Shape;
                }
                set
                {
                    if (value == null)
                    {
                        if (base.Shape != null)
                        {
                            base.Shape = null;
                            RaisePropertyChanged("Shape");
                        }
                    }
                    else
                    {
                        if (base.Shape == null)
                        {
                            base.Shape = value;
                            RaisePropertyChanged("Shape");
                        }
                        else
                        {
                            if (!value.Equals2(base.Shape))
                            {
                                base.Shape = value;
                                RaisePropertyChanged("Shape");
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void _128_Notify()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2("Cities");

            string p = null;

            City5 city = fc.Map<City5>().First();

            city.PropertyChanged += (s, e) =>
                                        {
                                            p = e.PropertyName;
                                            Debug.WriteLine(p);
                                        };

            Assert.AreEqual(null, p);

            city.AREANAME = "Hello";

            Assert.AreEqual("AREANAME", p);
        }

        [Test]
        public void _129_NotifyShapeChange()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2("Cities");

            string p = null;

            City5 city = fc.Map<City5>().First();

            city.PropertyChanged += (s, e) =>
            {
                p = e.PropertyName;

                Console.WriteLine(e.PropertyName);

            };

            Assert.AreEqual(null, p);

            city.Shape = city.Shape.Buffer(1);

            Assert.AreEqual("Shape", p);
        }

        public void JsonDotNet<T>(string fcName, int count) where T : MappableFeature, new()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2(fcName);

            Json.SerializingFunction =
                o => Newtonsoft.Json.JsonConvert.SerializeObject(o, Newtonsoft.Json.Formatting.Indented);

            foreach (T city in fc.Map<T>().Take(count))
            {
                var c1 = Json.Deserialize<T>(city.ToString(true));
                Assert.IsTrue(c1.ValueEquals(city));
            }

            Json.SerializingFunction =
                o => new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(o);
        }

        [Test]
        public void _130_JsonDotNet()
        {
            JsonDotNet<City5>("Cities", 100);
        }

        [Test]
        public void _131_JsonDotNet()
        {
            JsonDotNet<County>("Counties", 50);
        }

        private void CloneTest<T>(string fcName, int count) where T : MappableFeature, new()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2(fcName);
            var items1 = fc.Map<T>().Take(count).ToList();
            var items2 = fc.Map<T>().Take(count).Select(i =>
            {
                T i2 = new T();
                i2.CopyDataFrom(i);
                return i2;
            }).ToList();

            for (int i = 0; i < items1.Count; i++)
            {
                Assert.IsTrue(items1[i].ValueEquals(items2[i]));
            }
        }

        [Test]
        public void _132_CopyData()
        {
            CloneTest<City>("Cities", 100);
        }

        [Test]
        public void _133_CopyData()
        {
            CloneTest<County>("Counties", 100);
        }

        [Test]
        public void _134_CopyData()
        {
            CloneTest<Primaryline>("primaryline", 100);
        }

        [Test]
        public void _135_CopyData()
        {
            CloneTest<Waterbody>("waterbody", 100);
        }

        [Test]
        public void _136_CopyData()
        {
            CloneTest<Pipe>("pipes", 100);
        }

        [Test]
        public void _137_AutoNotify()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass("CITIES");

            City city = fc.Map<City>().First();

            Assert.AreEqual(false, city.IsDirty);

            string test = null;

            city.PropertyChanged += (s, e) => test = e.PropertyName;

            Assert.AreEqual(null, test);

            city.AREANAME = "Hello";
            city.AREANAME = "Hello";
            Assert.AreEqual("AREANAME", test);

            city.AREANAME = "Bye";

            Assert.AreEqual(true, city.IsDirty);

            city.Update();

            Assert.AreEqual(false, city.IsDirty);

            City city2 = new City();

            Assert.AreEqual(false, city2.IsDirty);

            city2.AREANAME = "Test";

            Assert.AreEqual(false, city2.IsDirty);
            Assert.AreEqual(false, city2.IsDataBound);

            IFeatureClass fc2 = _featureWorkspace.CreateFeatureClass<City>("Cities7654", esriGeometryType.esriGeometryPoint,
                                                         4326, true);

            city2 = city2.InsertInto(fc2);

            Assert.AreEqual(false, city2.IsDirty);
            Assert.AreEqual(true, city2.IsDataBound);

            city2.POP2000 = 123;

            Assert.AreEqual(true, city2.IsDirty);
            Assert.AreEqual(true, city2.IsDataBound);

        }

        [Test]
        public void _138_GlobalID()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2("CitiesWithGlobalID");

            foreach (CityWithGlobalID c in fc.Map<CityWithGlobalID>())
            {
                Guid g = c.GlobalID;
                Assert.AreEqual(g, c.GlobalID);
                c.GlobalID = new Guid("70ae9a13-4780-4bcb-b522-f2a70312f1f2");
                Assert.AreNotEqual(g, c.GlobalID);
                c.Update();
                Assert.AreEqual(g, c.GlobalID);
            }
        }

        //CandidateStore.cs
        //CentralDepot.cs
        //CompetitorStore.cs
        //DistributionCenter.cs
        //ExistingStore.cs
        //FireStation.cs
        //Hospital.cs
        //Route.cs
        //Store.cs
        //TractCentroid.cs

        [Test]
        public void _139_OrderByAndKml()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2("FireStations");

            List<FireStation> items1 = fc.Map<FireStation>("", "ORDER BY Match_addr").ToList();
            List<FireStation> items2 = fc.Map<FireStation>().OrderBy(i => i.Match_addr).ToList();

            for (int i = 0; i < items1.Count; i++)
            {
                Assert.AreEqual(false, items1[i].Shape.Equals(items2[i].Shape));
                Assert.AreEqual(true, items1[i].Shape.Equals2(items2[i].Shape));
                Assert.AreEqual(items1[i].Shape.ToKml().ToString(), items2[i].Shape.ToKml().ToString());
            }
        }

        [Test]
        public void _159_OrderBy()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2("TractCentroids");

            List<TractCentroid> items1 = fc.Map<TractCentroid>("", "ORDER BY POP2000").ToList();
            List<TractCentroid> items2 = fc.Map<TractCentroid>().OrderBy(i => i.POP2000).ToList();

            for (int i = 0; i < items1.Count; i++)
            {
                Assert.AreEqual(true, items1[i].ValueEquals(items2[i]));
            }
        }

        private void Notification<T>(string fcName) where T : MappableFeature, new()
        {
            IFeatureClass fc = _featureWorkspace.OpenFeatureClass2(fcName);

            T item1 = new T();

            Assert.AreEqual(false, item1.IsDataBound);
            Assert.AreEqual(false, item1.IsDirty);

            item1.Shape = item1.Shape;

            Assert.AreEqual(false, item1.IsDataBound);
            Assert.AreEqual(false, item1.IsDirty);

            T item2 = item1.InsertInto(fc);

            Assert.AreEqual(false, item1.IsDataBound);
            Assert.AreEqual(false, item1.IsDirty);

            Assert.AreEqual(true, item2.IsDataBound);
            Assert.AreEqual(false, item2.IsDirty);

            item1.Shape = item1.Shape;

            Assert.AreEqual(false, item1.IsDataBound);
            Assert.AreEqual(false, item1.IsDirty);

            item2.Shape = item2.Shape;

            Assert.AreEqual(true, item2.IsDataBound);
            Assert.AreEqual(true, item2.IsDirty);

            item2.Delete();

            Assert.AreEqual(false, item2.IsDataBound);
            Assert.AreEqual(false, item2.IsDirty);

            item2.Shape = item2.Shape;

            Assert.AreEqual(false, item2.IsDataBound);
            Assert.AreEqual(false, item2.IsDirty);
        }

        [Test]
        public void _160_Notification()
        {
            Notification<CandidateStore>("CandidateStores");
        }

        [Test]
        public void _162_Notification()
        {
            Notification<CompetitorStore>("CompetitorStores");
        }

        [Test]
        public void _163_Notification()
        {
            Notification<DistributionCenter>("DistributionCenter");
        }

        [Test]
        public void _164_Notification()
        {
            Notification<ExistingStore>("ExistingStore");
        }

        [Test]
        public void _165_Notification()
        {
            Notification<FireStation>("FireStations");
        }

        [Test]
        public void _166_Notification()
        {
            Notification<Hospital>("Hospitals");
        }

        [Test]
        public void _168_Notification()
        {
            Notification<Store>("Stores");
        }

        [Test]
        public void _169_Notification()
        {
            Notification<TractCentroid>("TractCentroids");
        }

        private void CodeFirst<T>(string fcName) where T : MappableFeature, new()
        {
            IFeatureClass fc1 = _featureWorkspace.OpenFeatureClass2(fcName);
            IFeatureClass fc2 = _featureWorkspace.CreateFeatureClass<T>(fcName + "_2", fc1.ShapeType, ((IGeoDataset)fc1).SpatialReference);

            foreach (T item in fc1.Map<T>())
            {
                item.InsertInto(fc2);
            }

            MappableFeatureComparer<T> comparer = new MappableFeatureComparer<T>();

            Assert.AreEqual(true, fc1.Map<T>().SequenceEqual(fc2.Map<T>(), comparer));
        }

        [Test]
        public void _170_CodeFirst()
        {
            CodeFirst<CentralDepot>("CentralDepots");
        }

        [Test]
        public void _171_CodeFirst()
        {
            CodeFirst<CompetitorStore>("CompetitorStores");
        }

        [Test]
        public void _172_CodeFirst()
        {
            CodeFirst<DistributionCenter>("DistributionCenter");
        }

        [Test]
        public void _173_CodeFirst()
        {
            CodeFirst<ExistingStore>("ExistingStore");
        }

        [Test]
        public void _174_CodeFirst()
        {
            CodeFirst<FireStation>("FireStations");
        }

        [Test]
        public void _175_CodeFirst()
        {
            CodeFirst<Hospital>("Hospitals");
        }

        [Test]
        public void _176_CodeFirst()
        {
            CodeFirst<Route>("Routes");
        }

        [Test]
        public void _177_CodeFirst()
        {
            CodeFirst<Store>("Stores");
        }

        [Test]
        public void _178_CodeFirst()
        {
            CodeFirst<TractCentroid>("TractCentroids");
        }

        [Test]
        public void _179_CodeFirst()
        {
            CodeFirst<CandidateStore>("CandidateStores");
        }

        private void CodeFirstAndMeta<T>(string fcName) where T : MappableFeature, new()
        {
            IFeatureClass fc1 = _featureWorkspace.OpenFeatureClass2(fcName);
            IFeatureClass fc2 = _featureWorkspace.CreateFeatureClass<T>(fcName + "_3", fc1.ShapeType, ((IGeoDataset)fc1).SpatialReference);

            Assert.AreEqual(fc1.ToCSharp("Hello", "World"), fc2.ToCSharp("Hello", "World"));
            Assert.AreEqual(fc1.ToVB("Hello"), fc2.ToVB("Hello"));
        }

        [Test]
        public void _180_CodeFirstAndMeta()
        {
            CodeFirstAndMeta<CentralDepot>("CentralDepots");
        }

        [Test]
        public void _181_CodeFirstAndMeta()
        {
            CodeFirstAndMeta<CompetitorStore>("CompetitorStores");
        }

        [Test]
        public void _182_CodeFirstAndMeta()
        {
            CodeFirstAndMeta<DistributionCenter>("DistributionCenter");
        }

        [Test]
        public void _183_CodeFirstAndMeta()
        {
            CodeFirstAndMeta<ExistingStore>("ExistingStore");
        }

        [Test]
        public void _184_CodeFirstAndMeta()
        {
            CodeFirstAndMeta<FireStation>("FireStations");
        }

        [Test]
        public void _185_CodeFirstAndMeta()
        {
            CodeFirstAndMeta<Hospital>("Hospitals");
        }

        [Test]
        public void _186_CodeFirstAndMeta()
        {
            CodeFirstAndMeta<Route>("Routes");
        }

        [Test]
        public void _187_CodeFirstAndMeta()
        {
            CodeFirstAndMeta<Store>("Stores");
        }

        [Test]
        public void _188_CodeFirstAndMeta()
        {
            CodeFirstAndMeta<TractCentroid>("TractCentroids");
        }

        [Test]
        public void _189_CodeFirstAndMeta()
        {
            CodeFirstAndMeta<CandidateStore>("CandidateStores");
        }

        private void ValueEquals<T>(string fcName) where T : MappableFeature, new()
        {
            IFeatureClass fc1 = _featureWorkspace.OpenFeatureClass2(fcName);
            IFeatureClass fc2 = _featureWorkspace.CreateFeatureClass<T>(fcName + "_4", fc1.ShapeType, ((IGeoDataset)fc1).SpatialReference);

            foreach (T item in fc1.Map<T>().Take(100))
            {
                item.InsertInto(fc2);
            }

            List<T> items2 = fc2.Map<T>().ToList();
            List<T> items1 = fc1.Map<T>().Take(items2.Count).ToList();

            Console.WriteLine(items1.Count);

            for (int i = 0; i < items1.Count; i++)
            {
                Assert.IsTrue(items1[i].ValueEquals(items2[i]));
                Assert.IsFalse(items1[i].Equals(items2[i]));
                Assert.IsFalse(items1[i] == items2[i]);
            }
        }

        [Test]
        public void _190_ValueEquals()
        {
            ValueEquals<CentralDepot>("CentralDepots");
        }

        [Test]
        public void _191_ValueEquals()
        {
            ValueEquals<CompetitorStore>("CompetitorStores");
        }

        [Test]
        public void _192_ValueEquals()
        {
            ValueEquals<DistributionCenter>("DistributionCenter");
        }

        [Test]
        public void _193_ValueEquals()
        {
            ValueEquals<ExistingStore>("ExistingStore");
        }

        [Test]
        public void _194_ValueEquals()
        {
            ValueEquals<FireStation>("FireStations");
        }

        [Test]
        public void _195_ValueEquals()
        {
            ValueEquals<Hospital>("Hospitals");
        }

        [Test]
        public void _196_ValueEquals()
        {
            ValueEquals<Route>("Routes");
        }

        [Test]
        public void _197_ValueEquals()
        {
            ValueEquals<Store>("Stores");
        }

        [Test]
        public void _198_ValueEquals()
        {
            ValueEquals<TractCentroid>("TractCentroids");
        }

        [Test]
        public void _199_ValueEquals()
        {
            ValueEquals<CandidateStore>("CandidateStores");
        }

        #endregion

        [Test]
        public void _200_NotifyPropertyChaged4Inherited()
        {
            IFeatureClass cityFeatureClass = _featureWorkspace.OpenFeatureClass("CITIES");

            IFeatureClass fc2 = _featureWorkspace.CreateFeatureClass<City2>("Cities987", esriGeometryType.esriGeometryPoint, 4326, true);

            City city = cityFeatureClass.Map<City>().First();

            City2 city2 = new City2();
            city2.CopyDataFrom(city);

            city2 = city2.InsertInto(fc2);

            Assert.AreEqual(false, city2.IsDirty);

            city2.AREANAME = "Hello";

            Assert.AreEqual(true, city2.IsDirty);

            city2 = city2.InsertInto(fc2);

            Assert.AreEqual(false, city2.IsDirty);

            city2.Comment = "Hello";

            Assert.AreEqual(true, city2.IsDirty);

            city2 = city2.InsertInto(fc2);

            Assert.AreEqual(false, city2.IsDirty);

            city2.Province = "Hello";

            Assert.AreEqual(false, city2.IsDirty);
        }
    }
}
