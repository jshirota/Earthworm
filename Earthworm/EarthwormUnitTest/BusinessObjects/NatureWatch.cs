using System;
using Earthworm;

public class NatureWatch : MappableFeature
{
    [MappedField("ObservationDate")]
    public virtual DateTime? ObservationDate { get; set; }

    [MappedField("FK_ExternalObsID")]
    public virtual int? FK_ExternalObsID { get; set; }

    [MappedField("FK_AttachmentListID")]
    public virtual Guid? FK_AttachmentListID { get; set; }

    [MappedField("FK_UserID")]
    public virtual Guid? FK_UserID { get; set; }

    [MappedField("CreateDate")]
    public virtual DateTime? CreateDate { get; set; }

    [MappedField("ObservationID")]
    public virtual Guid? ObservationID { get; set; }
}
