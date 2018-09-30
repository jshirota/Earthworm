using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geometry;

namespace Earthworm.SpatialAnalyst
{
    /// <summary>
    /// Provides common raster GIS operations via extension methods.
    /// </summary>
    public static class GridExt
    {
        #region Conditional

        /// <summary>
        /// Performs a conditional if/else evaluation on each of the input cells of an input grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <param name="grid3"></param>
        /// <returns></returns>
        public static Grid Con(this Grid grid1, Grid grid2, Grid grid3) => Grid.Con(grid1, grid2, grid3);

        /// <summary>
        /// Performs a conditional if/else evaluation on each of the input cells of an input grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid Con(this Grid grid1, Grid grid2, double n) => Grid.Con(grid1, grid2, n);

        /// <summary>
        /// Performs a conditional if/else evaluation on each of the input cells of an input grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="n"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Con(this Grid grid1, double n, Grid grid2) => Grid.Con(grid1, n, grid2);

        /// <summary>
        /// Performs a conditional if/else evaluation on each of the input cells of an input grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static Grid Con(this Grid grid, double n1, double n2) => Grid.Con(grid, n1, n2);

        /// <summary>
        /// Assigns output values using one of a list of grids determined by the value of an input grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Pick(this Grid grid1, Grid grid2) => Grid.Pick(grid1, grid2);

        /// <summary>
        /// Returns NoData if a conditional evaluation is true and returns the value specified by another grid if it is false.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid SetNull(this Grid grid1, Grid grid2) => Grid.SetNull(grid1, grid2);

        /// <summary>
        /// Returns NoData if a conditional evaluation is true and returns the value specified by another grid if it is false.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid SetNull(this Grid grid, double n) => Grid.SetNull(grid, n);

        #endregion

        #region Generalize

        /// <summary>
        /// Resamples the grid to a new cell size.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="cellSize"></param>
        /// <param name="resampleType"></param>
        /// <returns></returns>
        public static Grid Resample(this Grid grid, double cellSize, esriGeoAnalysisResampleEnum resampleType = esriGeoAnalysisResampleEnum.esriGeoAnalysisResampleNearest)
            => Grid.Resample(grid, cellSize, resampleType);

        #endregion

        #region Math

        /// <summary>
        /// Calculates the absolute value of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Abs(this Grid grid) => Grid.Abs(grid);

        /// <summary>
        /// Calculates the inverse cosine of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid ACos(this Grid grid) => Grid.ACos(grid);

        /// <summary>
        /// Calculates the inverse hyperbolic cosine of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid ACosH(this Grid grid) => Grid.ACosH(grid);

        /// <summary>
        /// Performs a bitwise 'and' operation on the binary value of two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid And(this Grid grid1, Grid grid2) => Grid.And(grid1, grid2);

        /// <summary>
        /// Calculates the inverse sine of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid ASin(this Grid grid) => Grid.ASin(grid);

        /// <summary>
        /// Calculates the inverse hyperbolic sine of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid ASinH(this Grid grid) => Grid.ASinH(grid);

        /// <summary>
        /// Calculates the inverse tangent of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid ATan(this Grid grid) => Grid.ATan(grid);

        /// <summary>
        /// Calculates the calculates the inverse tangent (based on x/y) of cells in a GeoDataset.
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <returns></returns>
        public static Grid ATan2(this Grid gridX, Grid gridY) => Grid.ATan2(gridX, gridY);

        /// <summary>
        /// Calculates the inverse hyperbolic tangent of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid ATanH(this Grid grid) => Grid.ATanH(grid);

        /// <summary>
        /// Performs a boolean 'and' operation on two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid BooleanAnd(this Grid grid1, Grid grid2) => Grid.BooleanAnd(grid1, grid2);

        /// <summary>
        /// Perorms a boolean 'complement' operation on two input grids.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid BooleanNot(this Grid grid) => Grid.BooleanNot(grid);

        /// <summary>
        /// Performs a boolean 'or' operation on two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid BooleanOr(this Grid grid1, Grid grid2) => Grid.BooleanOr(grid1, grid2);

        /// <summary>
        /// Performs a boolean 'exclusive or' operation on two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid BooleanXOr(this Grid grid1, Grid grid2) => Grid.BooleanXOr(grid1, grid2);

        /// <summary>
        /// Performs a combinatorial 'and' operation on two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid CombinatorialAnd(this Grid grid1, Grid grid2) => Grid.CombinatorialAnd(grid1, grid2);

        /// <summary>
        /// Performs a combinatorial 'or' operation on two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid CombinatorialOr(this Grid grid1, Grid grid2) => Grid.CombinatorialOr(grid1, grid2);

        /// <summary>
        /// Performs a combinatorial 'exclusive or' operation on two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid CombinatorialXOr(this Grid grid1, Grid grid2) => Grid.CombinatorialXOr(grid1, grid2);

        /// <summary>
        /// Calculates the cosine of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Cos(this Grid grid) => Grid.Cos(grid);

        /// <summary>
        /// Calculates the hyperbolic cosine of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid CosH(this Grid grid) => Grid.CosH(grid);

        /// <summary>
        /// Determines which values from the first input are logically different from the values of the second.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Diff(this Grid grid1, Grid grid2) => Grid.Diff(grid1, grid2);

        /// <summary>
        /// Divides the values of two inputs.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Divide(this Grid grid1, Grid grid2) => Grid.Divide(grid1, grid2);

        /// <summary>
        /// Returns 1 for cells where the first grid equals than the second grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid EqualTo(this Grid grid1, Grid grid2) => Grid.EqualTo(grid1, grid2);

        /// <summary>
        /// Calculates the base e exponential of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Exp(this Grid grid) => Grid.Exp(grid);

        /// <summary>
        /// Calculates the base 10 exponential of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Exp10(this Grid grid) => Grid.Exp10(grid);

        /// <summary>
        /// Calculates the base 2 exponential of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Exp2(this Grid grid) => Grid.Exp2(grid);

        /// <summary>
        /// Converts a grid into floating point representation.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Float(this Grid grid) => Grid.Float(grid);

        /// <summary>
        /// Returns 1 for cells where the first grid is greater than the second grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid GreaterThan(this Grid grid1, Grid grid2) => Grid.GreaterThan(grid1, grid2);

        /// <summary>
        /// Returns 1 for cells where the first grid is greater than or equal to the second grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid GreaterThanEqual(this Grid grid1, Grid grid2) => Grid.GreaterThanEqual(grid1, grid2);

        /// <summary>
        /// Determines which values from the first input are contained in list of geodata in the second input.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid InList(this Grid grid1, Grid grid2) => Grid.InList(grid1, grid2);

        /// <summary>
        /// Converts a grid to integer by truncation.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Int(this Grid grid) => Grid.Int(grid);

        /// <summary>
        /// On a cell by cell basis, returns 1 if the input value is NoData, and 0 if it is not.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid IsNull(this Grid grid) => Grid.IsNull(grid);

        /// <summary>
        /// Performs a bitwise 'left shift' operation on the binary value of two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid LeftShift(this Grid grid1, Grid grid2) => Grid.LeftShift(grid1, grid2);

        /// <summary>
        /// Returns 1 for cells where the first grid is less than the second grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid LessThan(this Grid grid1, Grid grid2) => Grid.LessThan(grid1, grid2);

        /// <summary>
        /// Returns 1 for cells where the first grid less than or equal to the second grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid LessThanEqual(this Grid grid1, Grid grid2) => Grid.LessThanEqual(grid1, grid2);

        /// <summary>
        /// Calculates the natural logarithm (base e) of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Ln(this Grid grid) => Grid.Ln(grid);

        /// <summary>
        /// Calculates the base 10 logarithm of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Log10(this Grid grid) => Grid.Log10(grid);

        /// <summary>
        /// Calculates the base 2 logarithm of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Log2(this Grid grid) => Grid.Log2(grid);

        /// <summary>
        /// Subtracts the values of two inputs.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Minus(this Grid grid1, Grid grid2) => Grid.Minus(grid1, grid2);

        /// <summary>
        /// Finds the remainder of the first input when divided by the second.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Mod(this Grid grid1, Grid grid2) => Grid.Mod(grid1, grid2);

        /// <summary>
        /// Changes the sign of the input grid (multiplies by -1).
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Negate(this Grid grid) => Grid.Negate(grid);

        /// <summary>
        /// Performs a bitwise 'complement' operation on the binary value of an input GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Not(this Grid grid) => Grid.Not(grid);

        /// <summary>
        /// Returns 1 for cells where the first grid is not equal to the second grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid NotEqual(this Grid grid1, Grid grid2) => Grid.NotEqual(grid1, grid2);

        /// <summary>
        /// Performs a bitwise 'or' operation on the binary value of two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Or(this Grid grid1, Grid grid2) => Grid.Or(grid1, grid2);

        /// <summary>
        /// Returns those values from the first input that are nonzero; otherwise, returns the value from the second.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Over(this Grid grid1, Grid grid2) => Grid.Over(grid1, grid2);

        /// <summary>
        /// Adds the values of two inputs.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Plus(this Grid grid1, Grid grid2) => Grid.Plus(grid1, grid2);

        /// <summary>
        /// Raises the cells in a grid to the Nth power.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Grid Power(this Grid grid, double n) => Grid.Power(grid, n);

        /// <summary>
        /// Raises the cells in a grid to the power of values found in another grid.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid PowerByCellValue(this Grid grid1, Grid grid2) => Grid.PowerByCellValue(grid1, grid2);

        /// <summary>
        /// Performs a bitwise 'right shift' operation on the binary value of two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid RightShift(this Grid grid1, Grid grid2) => Grid.RightShift(grid1, grid2);

        /// <summary>
        /// Returns the next lower whole number for each cell in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid RoundDown(this Grid grid) => Grid.RoundDown(grid);

        /// <summary>
        /// Returns the next higher whole number for each cell in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid RoundUp(this Grid grid) => Grid.RoundUp(grid);

        /// <summary>
        /// Calculates the sine of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Sin(this Grid grid) => Grid.Sin(grid);

        /// <summary>
        /// Calculates the hyperbolic sine of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid SinH(this Grid grid) => Grid.SinH(grid);

        /// <summary>
        /// Calculates the square of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Square(this Grid grid) => Grid.Square(grid);

        /// <summary>
        /// Calculates the square root of cells in a grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid SquareRoot(this Grid grid) => Grid.SquareRoot(grid);

        /// <summary>
        /// Calculates the tangent of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Tan(this Grid grid) => Grid.Tan(grid);

        /// <summary>
        /// Calculates the hyperbolic tangent of cells in a GeoDataset.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid TanH(this Grid grid) => Grid.TanH(grid);

        /// <summary>
        /// Multiplies the values of two inputs.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid Times(this Grid grid1, Grid grid2) => Grid.Times(grid1, grid2);

        /// <summary>
        /// Performs a bitwise 'exclusive or' operation on the binary value of two input grids.
        /// </summary>
        /// <param name="grid1"></param>
        /// <param name="grid2"></param>
        /// <returns></returns>
        public static Grid XOr(this Grid grid1, Grid grid2) => Grid.XOr(grid1, grid2);

        #endregion

        #region Neighborhood

        /// <summary>
        /// Finds the majority value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMajority(this Grid grid, int width, int height, bool ignoreNoData = true)
            => Grid.FocalMajority(grid, width, height, ignoreNoData);

        /// <summary>
        /// Finds the majority value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMajority(this Grid grid, int radius, bool ignoreNoData = true)
            => Grid.FocalMajority(grid, radius, ignoreNoData);

        /// <summary>
        /// Finds the maximum value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMax(this Grid grid, int width, int height, bool ignoreNoData = true)
            => Grid.FocalMax(grid, width, height, ignoreNoData);

        /// <summary>
        /// Finds the maximum value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMax(this Grid grid, int radius, bool ignoreNoData = true)
            => Grid.FocalMax(grid, radius, ignoreNoData);

        /// <summary>
        /// Finds the mean value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMean(this Grid grid, int width, int height, bool ignoreNoData = true)
            => Grid.FocalMean(grid, width, height, ignoreNoData);

        /// <summary>
        /// Finds the mean value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMean(this Grid grid, int radius, bool ignoreNoData = true)
            => Grid.FocalMean(grid, radius, ignoreNoData);

        /// <summary>
        /// Finds the median value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMedian(this Grid grid, int width, int height, bool ignoreNoData = true)
            => Grid.FocalMedian(grid, width, height, ignoreNoData);

        /// <summary>
        /// Finds the median value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMedian(this Grid grid, int radius, bool ignoreNoData = true)
            => Grid.FocalMedian(grid, radius, ignoreNoData);

        /// <summary>
        /// Finds the minimum value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMin(this Grid grid, int width, int height, bool ignoreNoData = true)
            => Grid.FocalMin(grid, width, height, ignoreNoData);

        /// <summary>
        /// Finds the minimum value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMin(this Grid grid, int radius, bool ignoreNoData = true)
            => Grid.FocalMin(grid, radius, ignoreNoData);

        /// <summary>
        /// Finds the minority value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMinority(this Grid grid, int width, int height, bool ignoreNoData = true)
            => Grid.FocalMinority(grid, width, height, ignoreNoData);

        /// <summary>
        /// Finds the minority value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalMinority(this Grid grid, int radius, bool ignoreNoData = true)
            => Grid.FocalMinority(grid, radius, ignoreNoData);

        /// <summary>
        /// Finds the range value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalRange(this Grid grid, int width, int height, bool ignoreNoData = true)
            => Grid.FocalRange(grid, width, height, ignoreNoData);

        /// <summary>
        /// Finds the range value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalRange(this Grid grid, int radius, bool ignoreNoData = true)
            => Grid.FocalRange(grid, radius, ignoreNoData);

        /// <summary>
        /// Finds the standard deviation value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalStd(this Grid grid, int width, int height, bool ignoreNoData = true)
            => Grid.FocalStd(grid, width, height, ignoreNoData);

        /// <summary>
        /// Finds the standard deviation value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalStd(this Grid grid, int radius, bool ignoreNoData = true)
            => Grid.FocalStd(grid, radius, ignoreNoData);

        /// <summary>
        /// Finds the sum value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalSum(this Grid grid, int width, int height, bool ignoreNoData = true)
            => Grid.FocalSum(grid, width, height, ignoreNoData);

        /// <summary>
        /// Finds the sum value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalSum(this Grid grid, int radius, bool ignoreNoData = true)
            => Grid.FocalSum(grid, radius, ignoreNoData);

        /// <summary>
        /// Finds the variety value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalVariety(this Grid grid, int width, int height, bool ignoreNoData = true)
            => Grid.FocalVariety(grid, width, height, ignoreNoData);

        /// <summary>
        /// Finds the variety value for each cell location on an input grid within a specified neighbourhood.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="radius"></param>
        /// <param name="ignoreNoData"></param>
        /// <returns></returns>
        public static Grid FocalVariety(this Grid grid, int radius, bool ignoreNoData = true)
            => Grid.FocalVariety(grid, radius, ignoreNoData);

        #endregion

        #region Surface

        /// <summary>
        /// Calculates the aspect grid from the input grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Grid Aspect(this Grid grid)
            => Grid.Aspect(grid);

        /// <summary>
        /// Calculates the shaded relief grid from the input grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="azimuth"></param>
        /// <param name="altitude"></param>
        /// <param name="inModelShadows"></param>
        /// <param name="zFactor"></param>
        /// <returns></returns>
        public static Grid Hillshade(this Grid grid, double azimuth = 315, double altitude = 45, bool inModelShadows = true, double zFactor = 1)
            => Grid.Hillshade(grid, azimuth, altitude, inModelShadows, zFactor);

        /// <summary>
        /// Calculates the slope grid from the input grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="slopeType"></param>
        /// <param name="zFactor"></param>
        /// <returns></returns>
        public static Grid Slope(this Grid grid, esriGeoAnalysisSlopeEnum slopeType = esriGeoAnalysisSlopeEnum.esriGeoAnalysisSlopeDegrees, double zFactor = 1)
            => Grid.Slope(grid, slopeType, zFactor);

        #endregion

        #region Transformation

        /// <summary>
        /// Subsets a raster using a rectangle.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public static Grid Clip(this Grid grid, IEnvelope envelope)
            => Grid.Clip(grid, envelope);

        #endregion
    }
}
