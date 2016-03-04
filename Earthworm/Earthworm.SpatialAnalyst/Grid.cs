using System;
using System.IO;
using System.Linq;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SpatialAnalyst;
using static System.IO.Path;
using static ESRI.ArcGIS.GeoAnalyst.esriGeoAnalysisStatisticsEnum;

namespace Earthworm.SpatialAnalyst
{
    /// <summary>
    /// Abstracts a raster dataset and common raster GIS operations.
    /// </summary>
    public class Grid
    {
        /// <summary>
        /// The underlying raster dataset.
        /// </summary>
        public IRasterDataset RasterDataset { get; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals((obj as Grid)?.RasterDataset, RasterDataset);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region Constructors

        static Grid()
        {
            EnableSpatialAnalyst();
        }

        /// <summary>
        /// Initializes a new instance of the Grid class.
        /// </summary>
        /// <param name="rasterDataset"></param>
        public Grid(IRasterDataset rasterDataset)
        {
            RasterDataset = rasterDataset;
        }

        /// <summary>
        /// Initializes a new instance of the Grid class.
        /// </summary>
        /// <param name="raster"></param>
        public Grid(IRaster raster)
        {
            RasterDataset = GetRasterDataset(raster);
        }

        /// <summary>
        /// Initializes a new instance of the Grid class.
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="name"></param>
        public Grid(IWorkspace workspace, string name)
        {
            RasterDataset = GetRasterDataset(workspace, name);
        }

        /// <summary>
        /// Initializes a new instance of the Grid class from the file path.
        /// </summary>
        /// <param name="fileName"></param>
        public Grid(string fileName)
        {
            var directory = GetDirectoryName(fileName);
            var name = GetFileName(fileName);
            var workspace = GetWorkspace(directory);

            RasterDataset = GetRasterDataset(workspace, name);
        }

        /// <summary>
        /// Initializes a new instance of the Grid class from the file path of an ASCII grid file.
        /// </summary>
        /// <param name="asciiFile"></param>
        /// <param name="isInteger"></param>
        public Grid(string asciiFile, bool isInteger)
        {
            var name = "r" + Guid.NewGuid().ToString().Substring(0, 6);
            var environment = new RasterAnalysis();
            var conversionOp = (IRasterImportOp)new RasterConversionOp();

            RasterDataset = conversionOp.ImportFromASCII(IsPathRooted(asciiFile) ? asciiFile : $@"{Environment.CurrentDirectory}\{asciiFile}", environment.OutWorkspace, name, "GRID", isInteger);
        }

        #endregion

        #region Private

        private static Grid Apply(Func<IGeoDataset> func)
            => new Grid((IRaster)func());

        private static Grid Apply(Func<IGeoDataset, IGeoDataset> func, Grid grid)
            => new Grid((IRaster)func((IGeoDataset)grid.RasterDataset));

        private static Grid Apply(Func<IGeoDataset, IGeoDataset, IGeoDataset> func, Grid grid1, Grid grid2)
            => new Grid((IRaster)func((IGeoDataset)grid1.RasterDataset, (IGeoDataset)grid2.RasterDataset));

        private static Grid Apply(Func<IGeoDataset, IGeoDataset, IGeoDataset, IGeoDataset> func, Grid grid1, Grid grid2, Grid grid3)
            => new Grid((IRaster)func((IGeoDataset)grid1.RasterDataset, (IGeoDataset)grid2.RasterDataset, (IGeoDataset)grid3.RasterDataset));

        private static Grid Apply(string expression, params Grid[] grids)
        {
            var mapAlgebraOp = (IMapAlgebraOp)new RasterMapAlgebraOp();

            for (var i = 0; i < grids.Length; i++)
                mapAlgebraOp.BindRaster((IGeoDataset)grids[i].RasterDataset, $"R{i + 1}");

            return new Grid((IRaster)mapAlgebraOp.Execute(expression));
        }

        private static void EnableSpatialAnalyst()
        {
            var uid = new UID { Value = "esriCore.SAExtension.1" };

            object o = null;
            var extensionManager = new ExtensionManager();
            ((IExtensionManagerAdmin)extensionManager).AddExtension(uid, ref o);

            var extensionConfig = (IExtensionConfig)extensionManager.FindExtension(uid);

            if (extensionConfig.State == esriExtensionState.esriESUnavailable)
                throw new InvalidOperationException("Spatial Analyst is not licensed.");

            if (extensionConfig.State == esriExtensionState.esriESDisabled)
                extensionConfig.State = esriExtensionState.esriESEnabled;
        }

        private IWorkspace GetWorkspace(string directory)
        {
            var workspaceFactory = new RasterWorkspaceFactory();

            return workspaceFactory.OpenFromFile(directory, 0);
        }

        private string GetFormat(string name)
        {
            var extension = GetExtension(name).ToLower();

            switch (extension)
            {
                case ".bmp": return "BMP";
                case ".gif": return "GIF";
                case ".img": return "IMAGINE Image";
                case ".jpg": return "JPG";
                case ".png": return "PNG";
                case ".tif": return "TIFF";
                case "": return "GRID";
                default: return extension.ToUpper();
            }
        }

        private static IRasterDataset GetRasterDataset(IWorkspace workspace, string name)
        {
            var enumDataset = workspace.Datasets[esriDatasetType.esriDTRasterDataset];
            var rasterDataset = Enumerable.Range(0, int.MaxValue).Select(n => enumDataset.Next()).FirstOrDefault(d => d.Name == name);
            enumDataset.Reset();

            return rasterDataset as IRasterDataset;
        }

        private static IRasterDataset GetRasterDataset(IRaster raster)
        {
            return ((IRasterBandCollection)raster).Item(0).RasterDataset;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Performs a Boolean 'And' operation on two grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid operator &(Grid grid1, Grid grid2) => Apply($"[R1] & [R2]", grid1, grid2);

        /// <summary>
        /// Performs a Boolean 'Or' operation on two grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid operator |(Grid grid1, Grid grid2) => Apply($"[R1] | [R2]", grid1, grid2);

        /// <summary>
        /// Performs an addition operation.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid operator +(Grid grid1, Grid grid2) => Apply($"[R1] + [R2]", grid1, grid2);

        /// <summary>
        /// Performs an addition operation.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid operator +(Grid grid, double n) => Apply($"[R1] + {n}", grid);

        /// <summary>
        /// Performs an addition operation.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid operator +(double n, Grid grid) => Apply($"{n} + [R1]", grid);

        /// <summary>
        /// Performs a subtraction operation.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid operator -(Grid grid1, Grid grid2) => Apply($"[R1] - [R2]", grid1, grid2);

        /// <summary>
        /// Performs a subtraction operation.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid operator -(Grid grid, double n) => Apply($"[R1] - {n}", grid);

        /// <summary>
        /// Performs a subtraction operation.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid operator -(double n, Grid grid) => Apply($"{n} - [R1]", grid);

        /// <summary>
        /// Negates the input grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid operator -(Grid grid) => Apply($"0 - [R1]", grid);

        /// <summary>
        /// Performs a multiplication operation.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid operator *(Grid grid1, Grid grid2) => Apply($"[R1] * [R2]", grid1, grid2);

        /// <summary>
        /// Performs a multiplication operation.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid operator *(Grid grid, double n) => Apply($"[R1] * {n}", grid);

        /// <summary>
        /// Performs a multiplication operation.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid operator *(double n, Grid grid) => Apply($"{n} * [R1]", grid);

        /// <summary>
        /// Performs a division operation.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid operator /(Grid grid1, Grid grid2) => Apply($"[R1] / [R2]", grid1, grid2);

        /// <summary>
        /// Performs a division operation.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid operator /(Grid grid, double n) => Apply($"[R1] / {n}", grid);

        /// <summary>
        /// Performs a division operation.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid operator /(double n, Grid grid) => Apply($"{n} / [R1]", grid);

        /// <summary>
        /// Performs a modulus operation.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid operator %(Grid grid1, Grid grid2) => Apply($"[R1] MOD [R2]", grid1, grid2);

        /// <summary>
        /// Performs a modulus operation.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid operator %(Grid grid, double n) => Apply($"[R1] MOD {n}", grid);

        /// <summary>
        /// Performs a modulus operation.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid operator %(double n, Grid grid) => Apply($"{n} MOD [R1]", grid);

        /// <summary>
        /// Performs a relational equal-to operation on two inputs.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid operator ==(Grid grid1, Grid grid2) => Apply($"[R1] == [R2]", grid1, grid2);

        /// <summary>
        /// Performs a relational equal-to operation on two inputs.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid operator ==(Grid grid, double n) => Apply($"[R1] == {n}", grid);

        /// <summary>
        /// Performs a relational equal-to operation on two inputs.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid operator ==(double n, Grid grid) => Apply($"{n} == [R1]", grid);

        /// <summary>
        /// Performs a relational not-equal-to operation on two inputs.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid operator !=(Grid grid1, Grid grid2) => Apply($"[R1] <> [R2]", grid1, grid2);

        /// <summary>
        /// Performs a relational not-equal-to operation on two inputs.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid operator !=(Grid grid, double n) => Apply($"[R1] <> {n}", grid);

        /// <summary>
        /// Performs a relational not-equal-to operation on two inputs.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid operator !=(double n, Grid grid) => Apply($"{n} <> [R1]", grid);

        /// <summary>
        /// Performs a relational greater-than operation on two inputs.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid operator >(Grid grid1, Grid grid2) => Apply($"[R1] > [R2]", grid1, grid2);

        /// <summary>
        /// Performs a relational greater-than operation on two inputs.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid operator >(Grid grid, double n) => Apply($"[R1] > {n}", grid);

        /// <summary>
        /// Performs a relational greater-than operation on two inputs.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid operator >(double n, Grid grid) => Apply($"{n} > [R1]", grid);

        /// <summary>
        /// Performs a relational less-than operation on two inputs.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid operator <(Grid grid1, Grid grid2) => Apply($"[R1] < [R2]", grid1, grid2);

        /// <summary>
        /// Performs a relational less-than operation on two inputs.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid operator <(Grid grid, double n) => Apply($"[R1] < {n}", grid);

        /// <summary>
        /// Performs a relational less-than operation on two inputs.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid operator <(double n, Grid grid) => Apply($"{n} < [R1]", grid);

        /// <summary>
        /// Performs a relational greater-than-or-equal-to operation on two inputs.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid operator >=(Grid grid1, Grid grid2) => Apply($"[R1] >= [R2]", grid1, grid2);

        /// <summary>
        /// Performs a relational greater-than-or-equal-to operation on two inputs.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid operator >=(Grid grid, double n) => Apply($"[R1] >= {n}", grid);

        /// <summary>
        /// Performs a relational greater-than-or-equal-to operation on two inputs.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid operator >=(double n, Grid grid) => Apply($"{n} >= [R1]", grid);

        /// <summary>
        /// Performs a relational less-than-or-equal-to operation on two inputs.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid operator <=(Grid grid1, Grid grid2) => Apply($"[R1] <= [R2]", grid1, grid2);

        /// <summary>
        /// Performs a relational less-than-or-equal-to operation on two inputs.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid operator <=(Grid grid, double n) => Apply($"[R1] <= {n}", grid);

        /// <summary>
        /// Performs a relational less-than-or-equal-to operation on two inputs.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid operator <=(double n, Grid grid) => Apply($"{n} <= [R1]", grid);

        #endregion

        #region Save

        /// <summary>
        /// Saves this grid.
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="name"></param>
        public void Save(IWorkspace workspace, string name)
        {
            var format = workspace.Type == esriWorkspaceType.esriFileSystemWorkspace ? GetFormat(name) : "GDB";

            ((ISaveAs2)RasterDataset).SaveAs(name, workspace, format);
        }

        /// <summary>
        /// Saves this grid.  The format is inferred from the file extension.
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            var directoryName = GetDirectoryName(fileName);
            var directory = directoryName == "" ? Environment.CurrentDirectory : directoryName;

            Directory.CreateDirectory(directory);

            if (File.Exists(fileName))
                File.Delete(fileName);

            if (fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                var conversionOp = (IRasterExportOp)new RasterConversionOp();
                conversionOp.ExportToASCII((IGeoDataset)RasterDataset, fileName);
                return;
            }

            var workspace = GetWorkspace(directory);
            var name = GetFileName(fileName);

            Save(workspace, name);
        }

        #endregion

        #region Conditional

        /// <summary>
        /// Performs a conditional if/else evaluation on each of the input cells of an input grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <param name="grid3"></param>
        /// <returns></returns>
        public static Grid Con(Grid grid1, Grid grid2, Grid grid3) => Apply($"CON([R1], [R2], [R3])", grid1, grid2, grid3);

        /// <summary>
        /// Performs a conditional if/else evaluation on each of the input cells of an input grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid Con(Grid grid1, Grid grid2, double n) => Apply($"CON([R1], [R2], {n})", grid1, grid2);

        /// <summary>
        /// Performs a conditional if/else evaluation on each of the input cells of an input grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="n"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Con(Grid grid1, double n, Grid grid2) => Apply($"CON([R1], {n}, [R2])", grid1, grid2);

        /// <summary>
        /// Performs a conditional if/else evaluation on each of the input cells of an input grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static Grid Con(Grid grid, double n1, double n2) => Apply($"CON([R1], {n1}, {n2})", grid);

        /// <summary>
        /// Assigns output values using one of a list of grids determined by the value of an input grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Pick(Grid grid1, Grid grid2) => Apply($"PICK([R1], [R2])", grid1, grid2);

        /// <summary>
        /// Returns NoData if a conditional evaluation is true and returns the value specified by another grid if it is false.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid SetNull(Grid grid1, Grid grid2) => Apply($"SETNULL([R1], [R2])", grid1, grid2);

        /// <summary>
        /// Returns NoData if a conditional evaluation is true and returns the value specified by another grid if it is false.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid SetNull(Grid grid, double n) => Apply($"SETNULL([R1], {n})", grid);

        #endregion

        #region Generalize

        private static IGeneralizeOp GeneralizeOp => (IGeneralizeOp)new RasterGeneralizeOp();

        /// <summary>
        /// Resamples the grid to a new cell size.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="cellSize"></param>
        /// <param name="resampleType"></param>
        /// <returns></returns>
        public static Grid Resample(Grid grid, double cellSize, esriGeoAnalysisResampleEnum resampleType = esriGeoAnalysisResampleEnum.esriGeoAnalysisResampleNearest) => Apply(r => GeneralizeOp.Resample(r, cellSize, resampleType), grid);

        #endregion

        #region Math

        private static IBitwiseOp BitwiseOp => (IBitwiseOp)new RasterMathOps();
        private static ILogicalOp LogicalOp => (ILogicalOp)new RasterMathOps();
        private static ILogicalOperatorOp LogicalOperatorOp => (ILogicalOperatorOp)new RasterMathOps();
        private static IMathOp MathOp => (IMathOp)new RasterMathOps();
        private static ITrigOp TrigOp => (ITrigOp)new RasterMathOps();

        /// <summary>
        /// Calculates the absolute value of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Abs(Grid grid) => Apply(MathOp.Abs, grid);

        /// <summary>
        /// Calculates the inverse cosine of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid ACos(Grid grid) => Apply(TrigOp.ACos, grid);

        /// <summary>
        /// Calculates the inverse hyperbolic cosine of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid ACosH(Grid grid) => Apply(TrigOp.ACosH, grid);

        /// <summary>
        /// Performs a bitwise 'and' operation on the binary value of two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid And(Grid grid1, Grid grid2) => Apply(BitwiseOp.And, grid1, grid2);

        /// <summary>
        /// Calculates the inverse sine of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid ASin(Grid grid) => Apply(TrigOp.ASin, grid);

        /// <summary>
        /// Calculates the inverse hyperbolic sine of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid ASinH(Grid grid) => Apply(TrigOp.ASinH, grid);

        /// <summary>
        /// Calculates the inverse tangent of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid ATan(Grid grid) => Apply(TrigOp.ATan, grid);

        /// <summary>
        /// Calculates the calculates the inverse tangent (based on x/y) of cells in a GeoDataset.
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <returns></returns>
        public static Grid ATan2(Grid gridX, Grid gridY) => Apply(TrigOp.ATan2, gridX, gridY);

        /// <summary>
        /// Calculates the inverse hyperbolic tangent of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid ATanH(Grid grid) => Apply(TrigOp.ATanH, grid);

        /// <summary>
        /// Performs a boolean 'and' operation on two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid BooleanAnd(Grid grid1, Grid grid2) => Apply(LogicalOp.BooleanAnd, grid1, grid2);

        /// <summary>
        /// Perorms a boolean 'complement' operation on two input grids.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid BooleanNot(Grid grid) => Apply(LogicalOp.BooleanNot, grid);

        /// <summary>
        /// Performs a boolean 'or' operation on two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid BooleanOr(Grid grid1, Grid grid2) => Apply(LogicalOp.BooleanOr, grid1, grid2);

        /// <summary>
        /// Performs a boolean 'exclusive or' operation on two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid BooleanXOr(Grid grid1, Grid grid2) => Apply(LogicalOp.BooleanXOr, grid1, grid2);

        /// <summary>
        /// Performs a combinatorial 'and' operation on two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid CombinatorialAnd(Grid grid1, Grid grid2) => Apply(LogicalOp.CombinatorialAnd, grid1, grid2);

        /// <summary>
        /// Performs a combinatorial 'or' operation on two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid CombinatorialOr(Grid grid1, Grid grid2) => Apply(LogicalOp.CombinatorialOr, grid1, grid2);

        /// <summary>
        /// Performs a combinatorial 'exclusive or' operation on two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid CombinatorialXOr(Grid grid1, Grid grid2) => Apply(LogicalOp.CombinatorialXOr, grid1, grid2);

        /// <summary>
        /// Calculates the cosine of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Cos(Grid grid) => Apply(TrigOp.Cos, grid);

        /// <summary>
        /// Calculates the hyperbolic cosine of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid CosH(Grid grid) => Apply(TrigOp.CosH, grid);

        /// <summary>
        /// Determines which values from the first input are logically different from the values of the second.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Diff(Grid grid1, Grid grid2) => Apply(LogicalOperatorOp.Diff, grid1, grid2);

        /// <summary>
        /// Divides the values of two inputs.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Divide(Grid grid1, Grid grid2) => Apply(MathOp.Divide, grid1, grid2);

        /// <summary>
        /// Returns 1 for cells where the first grid equals than the second grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid EqualTo(Grid grid1, Grid grid2) => Apply(LogicalOp.EqualTo, grid1, grid2);

        /// <summary>
        /// Calculates the base e exponential of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Exp(Grid grid) => Apply(MathOp.Exp, grid);

        /// <summary>
        /// Calculates the base 10 exponential of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Exp10(Grid grid) => Apply(MathOp.Exp10, grid);

        /// <summary>
        /// Calculates the base 2 exponential of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Exp2(Grid grid) => Apply(MathOp.Exp2, grid);

        /// <summary>
        /// Converts a grid into floating point representation.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Float(Grid grid) => Apply(MathOp.Float, grid);

        /// <summary>
        /// Returns 1 for cells where the first grid is greater than the second grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid GreaterThan(Grid grid1, Grid grid2) => Apply(LogicalOp.GreaterThan, grid1, grid2);

        /// <summary>
        /// Returns 1 for cells where the first grid is greater than or equal to the second grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid GreaterThanEqual(Grid grid1, Grid grid2) => Apply(LogicalOp.GreaterThanEqual, grid1, grid2);

        /// <summary>
        /// Determines which values from the first input are contained in list of geodata in the second input.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid InList(Grid grid1, Grid grid2) => Apply(LogicalOperatorOp.InList, grid1, grid2);

        /// <summary>
        /// Converts a grid to integer by truncation.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Int(Grid grid) => Apply(MathOp.Int, grid);

        /// <summary>
        /// On a cell by cell basis, returns 1 if the input value is NoData, and 0 if it is not.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid IsNull(Grid grid) => Apply(LogicalOp.IsNull, grid);

        /// <summary>
        /// Performs a bitwise 'left shift' operation on the binary value of two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid LeftShift(Grid grid1, Grid grid2) => Apply(BitwiseOp.LeftShift, grid1, grid2);

        /// <summary>
        /// Returns 1 for cells where the first grid is less than the second grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid LessThan(Grid grid1, Grid grid2) => Apply(LogicalOp.LessThan, grid1, grid2);

        /// <summary>
        /// Returns 1 for cells where the first grid less than or equal to the second grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid LessThanEqual(Grid grid1, Grid grid2) => Apply(LogicalOp.LessThanEqual, grid1, grid2);

        /// <summary>
        /// Calculates the natural logarithm (base e) of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Ln(Grid grid) => Apply(MathOp.Ln, grid);

        /// <summary>
        /// Calculates the base 10 logarithm of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Log10(Grid grid) => Apply(MathOp.Log10, grid);

        /// <summary>
        /// Calculates the base 2 logarithm of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Log2(Grid grid) => Apply(MathOp.Log2, grid);

        /// <summary>
        /// Subtracts the values of two inputs.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Minus(Grid grid1, Grid grid2) => Apply(MathOp.Minus, grid1, grid2);

        /// <summary>
        /// Finds the remainder of the first input when divided by the second.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Mod(Grid grid1, Grid grid2) => Apply(MathOp.Mod, grid1, grid2);

        /// <summary>
        /// Changes the sign of the input grid (multiplies by -1).
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Negate(Grid grid) => Apply(MathOp.Negate, grid);

        /// <summary>
        /// Performs a bitwise 'complement' operation on the binary value of an input GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Not(Grid grid) => Apply(BitwiseOp.Not, grid);

        /// <summary>
        /// Returns 1 for cells where the first grid is not equal to the second grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid NotEqual(Grid grid1, Grid grid2) => Apply(LogicalOp.NotEqual, grid1, grid2);

        /// <summary>
        /// Performs a bitwise 'or' operation on the binary value of two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Or(Grid grid1, Grid grid2) => Apply(BitwiseOp.Or, grid1, grid2);

        /// <summary>
        /// Returns those values from the first input that are nonzero; otherwise, returns the value from the second.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Over(Grid grid1, Grid grid2) => Apply(LogicalOperatorOp.Over, grid1, grid2);

        /// <summary>
        /// Adds the values of two inputs.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Plus(Grid grid1, Grid grid2) => Apply(MathOp.Plus, grid1, grid2);

        /// <summary>
        /// Raises the cells in a grid to the Nth power.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid Power(Grid grid, double n) => Apply(r => MathOp.Power(r, n), grid);

        /// <summary>
        /// Raises the cells in a grid to the power of values found in another grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid PowerByCellValue(Grid grid1, Grid grid2) => Apply(MathOp.PowerByCellValue, grid1, grid2);

        /// <summary>
        /// Performs a bitwise 'right shift' operation on the binary value of two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid RightShift(Grid grid1, Grid grid2) => Apply(BitwiseOp.RightShift, grid1, grid2);

        /// <summary>
        /// Returns the next lower whole number for each cell in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid RoundDown(Grid grid) => Apply(MathOp.RoundDown, grid);

        /// <summary>
        /// Returns the next higher whole number for each cell in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid RoundUp(Grid grid) => Apply(MathOp.RoundUp, grid);

        /// <summary>
        /// Calculates the sine of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Sin(Grid grid) => Apply(TrigOp.Sin, grid);

        /// <summary>
        /// Calculates the hyperbolic sine of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid SinH(Grid grid) => Apply(TrigOp.SinH, grid);

        /// <summary>
        /// Calculates the square of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Square(Grid grid) => Apply(MathOp.Square, grid);

        /// <summary>
        /// Calculates the square root of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid SquareRoot(Grid grid) => Apply(MathOp.SquareRoot, grid);

        /// <summary>
        /// Calculates the tangent of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Tan(Grid grid) => Apply(TrigOp.Tan, grid);

        /// <summary>
        /// Calculates the hyperbolic tangent of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid TanH(Grid grid) => Apply(TrigOp.TanH, grid);

        /// <summary>
        /// Multiplies the values of two inputs.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Times(Grid grid1, Grid grid2) => Apply(MathOp.Times, grid1, grid2);

        /// <summary>
        /// Performs a bitwise 'exclusive or' operation on the binary value of two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid XOr(Grid grid1, Grid grid2) => Apply(BitwiseOp.XOr, grid1, grid2);

        #endregion

        #region Neighborhood

        private static INeighborhoodOp NeighborhoodOp => (INeighborhoodOp)new RasterNeighborhoodOp();

        private static IRasterNeighborhood Circle(int radius)
        {
            var neighbourhood = new RasterNeighborhood();
            neighbourhood.SetCircle(radius, esriGeoAnalysisUnitsEnum.esriUnitsCells);
            return neighbourhood;
        }

        private static IRasterNeighborhood Rectangle(int width, int height)
        {
            var neighbourhood = new RasterNeighborhood();
            neighbourhood.SetRectangle(width, height, esriGeoAnalysisUnitsEnum.esriUnitsCells);
            return neighbourhood;
        }

        private static Grid Focal(Grid grid, esriGeoAnalysisStatisticsEnum statistics, IRasterNeighborhood neighbourhood, bool ignoreNoData)
            => new Grid((IRaster)NeighborhoodOp.FocalStatistics((IGeoDataset)grid.RasterDataset, statistics, neighbourhood, ignoreNoData));

        /// <summary>
        /// Finds the majority value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMajority(Grid grid, int width, int height, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsMajority, Rectangle(width, height), ignoreNoData);

        /// <summary>
        /// Finds the majority value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMajority(Grid grid, int radius, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsMajority, Circle(radius), ignoreNoData);

        /// <summary>
        /// Finds the maximum value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMax(Grid grid, int width, int height, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsMaximum, Rectangle(width, height), ignoreNoData);

        /// <summary>
        /// Finds the maximum value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMax(Grid grid, int radius, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsMaximum, Circle(radius), ignoreNoData);

        /// <summary>
        /// Finds the mean value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMean(Grid grid, int width, int height, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsMean, Rectangle(width, height), ignoreNoData);

        /// <summary>
        /// Finds the mean value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMean(Grid grid, int radius, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsMean, Circle(radius), ignoreNoData);

        /// <summary>
        /// Finds the median value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMedian(Grid grid, int width, int height, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsMedian, Rectangle(width, height), ignoreNoData);

        /// <summary>
        /// Finds the median value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMedian(Grid grid, int radius, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsMedian, Circle(radius), ignoreNoData);

        /// <summary>
        /// Finds the minimum value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMin(Grid grid, int width, int height, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsMinimum, Rectangle(width, height), ignoreNoData);

        /// <summary>
        /// Finds the minimum value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMin(Grid grid, int radius, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsMinimum, Circle(radius), ignoreNoData);

        /// <summary>
        /// Finds the minority value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMinority(Grid grid, int width, int height, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsMinority, Rectangle(width, height), ignoreNoData);

        /// <summary>
        /// Finds the minority value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMinority(Grid grid, int radius, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsMinority, Circle(radius), ignoreNoData);

        /// <summary>
        /// Finds the range value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalRange(Grid grid, int width, int height, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsRange, Rectangle(width, height), ignoreNoData);

        /// <summary>
        /// Finds the range value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalRange(Grid grid, int radius, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsRange, Circle(radius), ignoreNoData);

        /// <summary>
        /// Finds the standard deviation value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalStd(Grid grid, int width, int height, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsStd, Rectangle(width, height), ignoreNoData);

        /// <summary>
        /// Finds the standard deviation value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalStd(Grid grid, int radius, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsStd, Circle(radius), ignoreNoData);

        /// <summary>
        /// Finds the sum value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalSum(Grid grid, int width, int height, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsSum, Rectangle(width, height), ignoreNoData);

        /// <summary>
        /// Finds the sum value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalSum(Grid grid, int radius, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsSum, Circle(radius), ignoreNoData);

        /// <summary>
        /// Finds the variety value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalVariety(Grid grid, int width, int height, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsVariety, Rectangle(width, height), ignoreNoData);

        /// <summary>
        /// Finds the variety value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalVariety(Grid grid, int radius, bool ignoreNoData = true)
            => Focal(grid, esriGeoAnalysisStatsVariety, Circle(radius), ignoreNoData);

        #endregion

        #region Surface

        private static ISurfaceOp SurfaceOp => (ISurfaceOp)new RasterSurfaceOp();

        /// <summary>
        /// Calculates the aspect grid from the input grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Aspect(Grid grid)
            => Apply(SurfaceOp.Aspect, grid);

        /// <summary>
        /// Calculates the shaded relief grid from the input grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="azimuth"></param>
        /// <param name="altitude"></param>
        /// <param name="inModelShadows"></param>
        /// <param name="zFactor"></param>
        /// <returns></returns>
        public static Grid Hillshade(Grid grid, double azimuth = 315, double altitude = 45, bool inModelShadows = true, double zFactor = 1)
            => Apply(r => SurfaceOp.HillShade(r, 315, altitude, inModelShadows, zFactor), grid);

        /// <summary>
        /// Calculates the slope grid from the input grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="slopeType"></param>
        /// <param name="zFactor"></param>
        /// <returns></returns>
        public static Grid Slope(Grid grid, esriGeoAnalysisSlopeEnum slopeType = esriGeoAnalysisSlopeEnum.esriGeoAnalysisSlopeDegrees, double zFactor = 1)
            => Apply(r => SurfaceOp.Slope(r, slopeType, zFactor), grid);

        #endregion

        #region Transformation

        /// <summary>
        /// Combines multiple input grids into a single grid.
        /// </summary>
        /// <param name="grids"></param>
        /// <returns></returns>
        public static Grid Mosaic(params Grid[] grids)
            => Apply($"MOSAIC({string.Join(",", Enumerable.Range(0, grids.Length).Select(n => $"[R{n}]"))})", grids);

        #endregion
    }
}
