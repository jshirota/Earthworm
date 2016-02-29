using System;
using System.Linq;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SpatialAnalyst;

namespace Earthworm
{
    public class Grid
    {
        public IRasterDataset RasterDataset { get; }

        static Grid()
        {
            EnableSpatialAnalyst();
        }

        public Grid(IRasterDataset rasterDataset)
        {
            RasterDataset = rasterDataset;
        }

        public Grid(IRaster raster) : this(GetRasterDataset(raster)) { }

        public Grid(IWorkspace workspace, string name) : this(GetRasterDataset(workspace, name)) { }

        public void Save(IWorkspace workspace, string name)
        {
            ((ISaveAs2)RasterDataset).SaveAs(name, workspace, "gdb");
        }

        #region Private

        private static Grid Apply(Func<IGeoDataset> func)
            => new Grid((IRaster)func());

        private static Grid Apply(Func<IGeoDataset, IGeoDataset> func, Grid grid)
            => new Grid((IRaster)func((IGeoDataset)grid.RasterDataset));

        private static Grid Apply(Func<IGeoDataset, IGeoDataset, IGeoDataset> func, Grid grid1, Grid grid2)
            => new Grid((IRaster)func((IGeoDataset)grid1.RasterDataset, (IGeoDataset)grid2.RasterDataset));

        private static Grid Apply(Func<IGeoDataset, IGeoDataset, IGeoDataset, IGeoDataset> func, Grid grid1, Grid grid2, Grid grid3)
            => new Grid((IRaster)func((IGeoDataset)grid1.RasterDataset, (IGeoDataset)grid2.RasterDataset, (IGeoDataset)grid3.RasterDataset));

        private static Grid Apply(string expression, Grid grid1)
        {
            var op = (IMapAlgebraOp)new RasterMapAlgebraOp();
            op.BindRaster((IGeoDataset)grid1.RasterDataset, "R1");
            return new Grid((IRaster)op.Execute(expression));
        }

        private static Grid Apply(string expression, Grid grid1, Grid grid2)
        {
            var op = (IMapAlgebraOp)new RasterMapAlgebraOp();
            op.BindRaster((IGeoDataset)grid1.RasterDataset, "R1");
            op.BindRaster((IGeoDataset)grid2.RasterDataset, "R2");
            return new Grid((IRaster)op.Execute(expression));
        }

        private static Grid Apply(string expression, Grid grid1, Grid grid2, Grid grid3)
        {
            var op = (IMapAlgebraOp)new RasterMapAlgebraOp();
            op.BindRaster((IGeoDataset)grid1.RasterDataset, "R1");
            op.BindRaster((IGeoDataset)grid2.RasterDataset, "R2");
            op.BindRaster((IGeoDataset)grid3.RasterDataset, "R3");
            return new Grid((IRaster)op.Execute(expression));
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

        public static Grid operator +(Grid grid1, Grid grid2) => Apply($"[R1] + [R2]", grid1, grid2);
        public static Grid operator +(Grid grid, double n) => Apply($"[R1] + {n}", grid);
        public static Grid operator +(double n, Grid grid) => Apply($"{n} + [R1]", grid);

        public static Grid operator -(Grid grid1, Grid grid2) => Apply($"[R1] - [R2]", grid1, grid2);
        public static Grid operator -(Grid grid, double n) => Apply($"[R1] - {n}", grid);
        public static Grid operator -(double n, Grid grid) => Apply($"{n} - [R1]", grid);

        public static Grid operator *(Grid grid1, Grid grid2) => Apply($"[R1] * [R2]", grid1, grid2);
        public static Grid operator *(Grid grid, double n) => Apply($"[R1] * {n}", grid);
        public static Grid operator *(double n, Grid grid) => Apply($"{n} * [R1]", grid);

        public static Grid operator /(Grid grid1, Grid grid2) => Apply($"[R1] / [R2]", grid1, grid2);
        public static Grid operator /(Grid grid, double n) => Apply($"[R1] / {n}", grid);
        public static Grid operator /(double n, Grid grid) => Apply($"{n} / [R1]", grid);

        public static Grid operator %(Grid grid1, Grid grid2) => Apply($"[R1] MOD [R2]", grid1, grid2);
        public static Grid operator %(Grid grid, double n) => Apply($"[R1] MOD {n}", grid);
        public static Grid operator %(double n, Grid grid) => Apply($"{n} MOD [R1]", grid);

        public static Grid operator ==(Grid grid1, Grid grid2) => Apply($"[R1] == [R2]", grid1, grid2);
        public static Grid operator ==(Grid grid, double n) => Apply($"[R1] == {n}", grid);
        public static Grid operator ==(double n, Grid grid) => Apply($"{n} == [R1]", grid);

        public static Grid operator !=(Grid grid1, Grid grid2) => Apply($"[R1] <> [R2]", grid1, grid2);
        public static Grid operator !=(Grid grid, double n) => Apply($"[R1] <> {n}", grid);
        public static Grid operator !=(double n, Grid grid) => Apply($"{n} <> [R1]", grid);

        public static Grid operator >(Grid grid1, Grid grid2) => Apply($"[R1] > [R2]", grid1, grid2);
        public static Grid operator >(Grid grid, double n) => Apply($"[R1] > {n}", grid);
        public static Grid operator >(double n, Grid grid) => Apply($"{n} > [R1]", grid);

        public static Grid operator <(Grid grid1, Grid grid2) => Apply($"[R1] < [R2]", grid1, grid2);
        public static Grid operator <(Grid grid, double n) => Apply($"[R1] < {n}", grid);
        public static Grid operator <(double n, Grid grid) => Apply($"{n} < [R1]", grid);

        public static Grid operator >=(Grid grid1, Grid grid2) => Apply($"[R1] >= [R2]", grid1, grid2);
        public static Grid operator >=(Grid grid, double n) => Apply($"[R1] >= {n}", grid);
        public static Grid operator >=(double n, Grid grid) => Apply($"{n} >= [R1]", grid);

        public static Grid operator <=(Grid grid1, Grid grid2) => Apply($"[R1] <= [R2]", grid1, grid2);
        public static Grid operator <=(Grid grid, double n) => Apply($"[R1] <= {n}", grid);
        public static Grid operator <=(double n, Grid grid) => Apply($"{n} <= [R1]", grid);

        #endregion

        #region ConditionalOp

        private static IConditionalOp ConditionalOp => (IConditionalOp)new RasterConditionalOp();

        public static Grid Con(Grid grid1, Grid grid2, Grid grid3) => Apply($"CON([R1], [R2], [R3])", grid1, grid2, grid3);
        public static Grid Con(Grid grid1, Grid grid2, double n) => Apply($"CON([R1], [R2], {n})", grid1, grid2);
        public static Grid Con(Grid grid1, double n, Grid grid2) => Apply($"CON([R1], {n}, [R2])", grid1, grid2);
        public static Grid Con(Grid grid, double n1, double n2) => Apply($"CON([R1], {n1}, {n2})", grid);
        public static Grid Pick(Grid grid1, Grid grid2) => Apply(ConditionalOp.Pick, grid1, grid2);
        public static Grid SetNull(Grid grid1, Grid grid2) => Apply($"SETNULL([R1], [R2])", grid1, grid2);
        public static Grid SetNull(Grid grid, double n) => Apply($"SETNULL([R1], {n})", grid);

        #endregion

        #region  GeneralizeOp

        private static IGeneralizeOp GeneralizeOp => (IGeneralizeOp)new RasterGeneralizeOp();

        public static Grid Resample(Grid grid, double cellSize, esriGeoAnalysisResampleEnum resampleType) => Apply(r => GeneralizeOp.Resample(r, cellSize, resampleType), grid);

        #endregion

        #region MathOp

        private static IMathOp MathOp => (IMathOp)new RasterMathOps();
        private static ITrigOp TrigOp => (ITrigOp)new RasterMathOps();
        private static ILogicalOp LogicalOp => (ILogicalOp)new RasterMathOps();
        private static IBitwiseOp BitwiseOp => (IBitwiseOp)new RasterMathOps();
        private static ILogicalOperatorOp LogicalOperatorOp => (ILogicalOperatorOp)new RasterMathOps();

        public static Grid Abs(Grid grid) => Apply(MathOp.Abs, grid);
        public static Grid ACos(Grid grid) => Apply(TrigOp.ACos, grid);
        public static Grid ACosH(Grid grid) => Apply(TrigOp.ACosH, grid);
        public static Grid And(Grid grid1, Grid grid2) => Apply(BitwiseOp.And, grid1, grid2);
        public static Grid ASin(Grid grid) => Apply(TrigOp.ASin, grid);
        public static Grid ASinH(Grid grid) => Apply(TrigOp.ASinH, grid);
        public static Grid ATan(Grid grid) => Apply(TrigOp.ATan, grid);
        public static Grid ATan2(Grid gridX, Grid gridY) => Apply(TrigOp.ATan2, gridX, gridY);
        public static Grid ATanH(Grid grid) => Apply(TrigOp.ATanH, grid);
        public static Grid BooleanAnd(Grid grid1, Grid grid2) => Apply(LogicalOp.BooleanAnd, grid1, grid2);
        public static Grid BooleanNot(Grid grid) => Apply(LogicalOp.BooleanNot, grid);
        public static Grid BooleanOr(Grid grid1, Grid grid2) => Apply(LogicalOp.BooleanOr, grid1, grid2);
        public static Grid BooleanXOr(Grid grid1, Grid grid2) => Apply(LogicalOp.BooleanXOr, grid1, grid2);
        public static Grid CombinatorialAnd(Grid grid1, Grid grid2) => Apply(LogicalOp.CombinatorialAnd, grid1, grid2);
        public static Grid CombinatorialOr(Grid grid1, Grid grid2) => Apply(LogicalOp.CombinatorialOr, grid1, grid2);
        public static Grid CombinatorialXOr(Grid grid1, Grid grid2) => Apply(LogicalOp.CombinatorialXOr, grid1, grid2);
        public static Grid Cos(Grid grid) => Apply(TrigOp.Cos, grid);
        public static Grid CosH(Grid grid) => Apply(TrigOp.CosH, grid);
        public static Grid Diff(Grid grid1, Grid grid2) => Apply(LogicalOperatorOp.Diff, grid1, grid2);
        public static Grid Divide(Grid grid1, Grid grid2) => Apply(MathOp.Divide, grid1, grid2);
        public static Grid EqualTo(Grid grid1, Grid grid2) => Apply(LogicalOp.EqualTo, grid1, grid2);
        public static Grid Exp(Grid grid) => Apply(MathOp.Exp, grid);
        public static Grid Exp10(Grid grid) => Apply(MathOp.Exp10, grid);
        public static Grid Exp2(Grid grid) => Apply(MathOp.Exp2, grid);
        public static Grid Float(Grid grid) => Apply(MathOp.Float, grid);
        public static Grid GreaterThan(Grid grid1, Grid grid2) => Apply(LogicalOp.GreaterThan, grid1, grid2);
        public static Grid GreaterThanEqual(Grid grid1, Grid grid2) => Apply(LogicalOp.GreaterThanEqual, grid1, grid2);
        public static Grid InList(Grid grid1, Grid grid2) => Apply(LogicalOperatorOp.InList, grid1, grid2);
        public static Grid Int(Grid grid) => Apply(MathOp.Int, grid);
        public static Grid IsNull(Grid grid) => Apply(LogicalOp.IsNull, grid);
        public static Grid LeftShift(Grid grid1, Grid grid2) => Apply(BitwiseOp.LeftShift, grid1, grid2);
        public static Grid LessThan(Grid grid1, Grid grid2) => Apply(LogicalOp.LessThan, grid1, grid2);
        public static Grid LessThanEqual(Grid grid1, Grid grid2) => Apply(LogicalOp.LessThanEqual, grid1, grid2);
        public static Grid Ln(Grid grid) => Apply(MathOp.Ln, grid);
        public static Grid Log10(Grid grid) => Apply(MathOp.Log10, grid);
        public static Grid Log2(Grid grid) => Apply(MathOp.Log2, grid);
        public static Grid Minus(Grid grid1, Grid grid2) => Apply(MathOp.Minus, grid1, grid2);
        public static Grid Mod(Grid grid1, Grid grid2) => Apply(MathOp.Mod, grid1, grid2);
        public static Grid Negate(Grid grid) => Apply(MathOp.Negate, grid);
        public static Grid Not(Grid grid) => Apply(BitwiseOp.Not, grid);
        public static Grid NotEqual(Grid grid1, Grid grid2) => Apply(LogicalOp.NotEqual, grid1, grid2);
        public static Grid Or(Grid grid1, Grid grid2) => Apply(BitwiseOp.Or, grid1, grid2);
        public static Grid Over(Grid grid1, Grid grid2) => Apply(LogicalOperatorOp.Over, grid1, grid2);
        public static Grid Plus(Grid grid1, Grid grid2) => Apply(MathOp.Plus, grid1, grid2);
        public static Grid Power(Grid grid, double n) => Apply(r => MathOp.Power(r, n), grid);
        public static Grid PowerByCellValue(Grid grid1, Grid grid2) => Apply(MathOp.PowerByCellValue, grid1, grid2);
        public static Grid RightShift(Grid grid1, Grid grid2) => Apply(BitwiseOp.RightShift, grid1, grid2);
        public static Grid RoundDown(Grid grid) => Apply(MathOp.RoundDown, grid);
        public static Grid RoundUp(Grid grid) => Apply(MathOp.RoundUp, grid);
        public static Grid Sin(Grid grid) => Apply(TrigOp.Sin, grid);
        public static Grid SinH(Grid grid) => Apply(TrigOp.SinH, grid);
        public static Grid Square(Grid grid) => Apply(MathOp.Square, grid);
        public static Grid SquareRoot(Grid grid) => Apply(MathOp.SquareRoot, grid);
        public static Grid Tan(Grid grid) => Apply(TrigOp.Tan, grid);
        public static Grid TanH(Grid grid) => Apply(TrigOp.TanH, grid);
        public static Grid Times(Grid grid1, Grid grid2) => Apply(MathOp.Times, grid1, grid2);
        public static Grid XOr(Grid grid1, Grid grid2) => Apply(BitwiseOp.XOr, grid1, grid2);

        #endregion

        #region NeighborhoodOp

        private static INeighborhoodOp NeighborhoodOp => (INeighborhoodOp)new RasterNeighborhoodOp();

        private static IRasterNeighborhood Circle(int radius)
        {
            var neighborhood = new RasterNeighborhood();
            neighborhood.SetCircle(radius, esriGeoAnalysisUnitsEnum.esriUnitsCells);
            return neighborhood;
        }

        public static Grid FocalMajority(Grid grid, int radius, bool ignoreNoData = true)
            => new Grid((IRaster)NeighborhoodOp.FocalStatistics((IGeoDataset)grid.RasterDataset, esriGeoAnalysisStatisticsEnum.esriGeoAnalysisStatsMajority, Circle(radius), ignoreNoData));

        public static Grid FocalMax(Grid grid, int radius, bool ignoreNoData = true)
            => new Grid((IRaster)NeighborhoodOp.FocalStatistics((IGeoDataset)grid.RasterDataset, esriGeoAnalysisStatisticsEnum.esriGeoAnalysisStatsMaximum, Circle(radius), ignoreNoData));

        public static Grid FocalMean(Grid grid, int radius, bool ignoreNoData = true)
            => new Grid((IRaster)NeighborhoodOp.FocalStatistics((IGeoDataset)grid.RasterDataset, esriGeoAnalysisStatisticsEnum.esriGeoAnalysisStatsMean, Circle(radius), ignoreNoData));

        public static Grid FocalMedian(Grid grid, int radius, bool ignoreNoData = true)
            => new Grid((IRaster)NeighborhoodOp.FocalStatistics((IGeoDataset)grid.RasterDataset, esriGeoAnalysisStatisticsEnum.esriGeoAnalysisStatsMedian, Circle(radius), ignoreNoData));

        public static Grid FocalMin(Grid grid, int radius, bool ignoreNoData = true)
            => new Grid((IRaster)NeighborhoodOp.FocalStatistics((IGeoDataset)grid.RasterDataset, esriGeoAnalysisStatisticsEnum.esriGeoAnalysisStatsMinimum, Circle(radius), ignoreNoData));

        public static Grid FocalMinority(Grid grid, int radius, bool ignoreNoData = true)
            => new Grid((IRaster)NeighborhoodOp.FocalStatistics((IGeoDataset)grid.RasterDataset, esriGeoAnalysisStatisticsEnum.esriGeoAnalysisStatsMinority, Circle(radius), ignoreNoData));

        public static Grid FocalRange(Grid grid, int radius, bool ignoreNoData = true)
            => new Grid((IRaster)NeighborhoodOp.FocalStatistics((IGeoDataset)grid.RasterDataset, esriGeoAnalysisStatisticsEnum.esriGeoAnalysisStatsRange, Circle(radius), ignoreNoData));

        public static Grid FocalStd(Grid grid, int radius, bool ignoreNoData = true)
            => new Grid((IRaster)NeighborhoodOp.FocalStatistics((IGeoDataset)grid.RasterDataset, esriGeoAnalysisStatisticsEnum.esriGeoAnalysisStatsStd, Circle(radius), ignoreNoData));

        public static Grid FocalSum(Grid grid, int radius, bool ignoreNoData = true)
            => new Grid((IRaster)NeighborhoodOp.FocalStatistics((IGeoDataset)grid.RasterDataset, esriGeoAnalysisStatisticsEnum.esriGeoAnalysisStatsSum, Circle(radius), ignoreNoData));

        public static Grid FocalVariety(Grid grid, int radius, bool ignoreNoData = true)
            => new Grid((IRaster)NeighborhoodOp.FocalStatistics((IGeoDataset)grid.RasterDataset, esriGeoAnalysisStatisticsEnum.esriGeoAnalysisStatsVariety, Circle(radius), ignoreNoData));

        #endregion

        #region SurfaceOp

        private static ISurfaceOp SurfaceOp => (ISurfaceOp)new RasterSurfaceOp();

        public static Grid Aspect(Grid grid)
            => Apply(SurfaceOp.Aspect, grid);

        public static Grid Hillshade(Grid grid, double azimuth = 315, double altitude = 45, bool inModelShadows = true, double zFactor = 1)
            => Apply(r => SurfaceOp.HillShade(r, 315, altitude, inModelShadows, zFactor), grid);

        public static Grid Slope(Grid grid, esriGeoAnalysisSlopeEnum slopeType = esriGeoAnalysisSlopeEnum.esriGeoAnalysisSlopeDegrees, double zFactor = 1)
            => Apply(r => SurfaceOp.Slope(r, slopeType, zFactor), grid);

        #endregion
    }
}
