using System;
using Earthworm;

public class Community : MappableFeature
{
    [MappedField("CommunityType", 255)]
    public virtual string CommunityType { get; set; }

    [MappedField("InitialLanguage", 255)]
    public virtual string InitialLanguage { get; set; }

    [MappedField("ShortName", 255)]
    public virtual string ShortName { get; set; }

    [MappedField("Name", 255)]
    public virtual string Name { get; set; }

    [MappedField("WebSiteURL", 255)]
    public virtual string WebSiteURL { get; set; }

    [MappedField("Description", 3000)]
    public virtual string Description { get; set; }

    [MappedField("CreateDate")]
    public virtual DateTime? CreateDate { get; set; }

    [MappedField("AccepAnyCommunity")]
    public virtual short? AccepAnyCommunity { get; set; }

    [MappedField("OnlyVerifyAreaOfIntrest")]
    public virtual short? OnlyVerifyAreaOfIntrest { get; set; }

    [MappedField("OnlyListCommSpecies")]
    public virtual short? OnlyListCommSpecies { get; set; }

    [MappedField("OnlyRecordUploadedMedia")]
    public virtual short? OnlyRecordUploadedMedia { get; set; }

    [MappedField("LastActivityDate")]
    public virtual DateTime? LastActivityDate { get; set; }

    [MappedField("IsActive")]
    public virtual short? IsActive { get; set; }

    [MappedField("IsHidden")]
    public virtual short? IsHidden { get; set; }

    [MappedField("FK_AttachmentListID_W")]
    public virtual Guid? FK_AttachmentListID_W { get; set; }

    [MappedField("FK_AttachmentListID_M")]
    public virtual Guid? FK_AttachmentListID_M { get; set; }

    [MappedField("FK_UserID")]
    public virtual Guid? FK_UserID { get; set; }

    [MappedField("CommunityID")]
    public virtual Guid? CommunityID { get; set; }
}
