using System;
using Earthworm;

public class Paristurn : MappableFeature
{
    [MappedField("Edge1End", 1)]
    public virtual string Edge1End { get; set; }

    [MappedField("Edge1FCID")]
    public virtual int Edge1FCID { get; set; }

    [MappedField("Edge1FID")]
    public virtual int Edge1FID { get; set; }

    [MappedField("Edge1Pos")]
    public virtual double Edge1Pos { get; set; }

    [MappedField("Edge2FCID")]
    public virtual int Edge2FCID { get; set; }

    [MappedField("Edge2FID")]
    public virtual int Edge2FID { get; set; }

    [MappedField("Edge2Pos")]
    public virtual double Edge2Pos { get; set; }

    [MappedField("Edge3FCID")]
    public virtual int Edge3FCID { get; set; }

    [MappedField("Edge3FID")]
    public virtual int Edge3FID { get; set; }

    [MappedField("Edge3Pos")]
    public virtual double Edge3Pos { get; set; }

    [MappedField("Edge4FCID")]
    public virtual int Edge4FCID { get; set; }

    [MappedField("Edge4FID")]
    public virtual int Edge4FID { get; set; }

    [MappedField("Edge4Pos")]
    public virtual double Edge4Pos { get; set; }

    [MappedField("Edge5FCID")]
    public virtual int Edge5FCID { get; set; }

    [MappedField("Edge5FID")]
    public virtual int Edge5FID { get; set; }

    [MappedField("Edge5Pos")]
    public virtual double Edge5Pos { get; set; }

    [MappedField("FROM_EDGE")]
    public virtual double FROM_EDGE { get; set; }

    [MappedField("TO_EDGE")]
    public virtual double TO_EDGE { get; set; }
}
