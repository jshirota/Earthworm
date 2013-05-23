using System;
using Earthworm;

public class InsideDiagram : MappableFeature
{
    [MappedField("DOCID")]
    public virtual int? DOCID { get; set; }

    [MappedField("DOCNAME", 100)]
    public virtual string DOCNAME { get; set; }

    [MappedField("SymbolScaling")]
    public virtual double? SymbolScaling { get; set; }

    [MappedField("LabelScaling")]
    public virtual double? LabelScaling { get; set; }

    [MappedField("Creator", 30)]
    public virtual string Creator { get; set; }

    [MappedField("CreationDate")]
    public virtual DateTime? CreationDate { get; set; }

    [MappedField("RefreshDate")]
    public virtual DateTime? RefreshDate { get; set; }

    [MappedField("ESRIVersion", 30)]
    public virtual string ESRIVersion { get; set; }
}
