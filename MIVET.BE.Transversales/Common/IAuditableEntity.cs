namespace MIVET.BE.Transversales.Common;

public interface IAuditable : ICreationAuditable, IUpdateAuditable { }

public interface IAuditableEntity { }

public interface ICreationAuditable : IAuditableEntity
{
}

public interface IUpdateAuditable : IAuditableEntity
{
}

public interface ISoftDeleteAuditable : IAuditableEntity
{
}

public interface ISoftDeletable : ISoftDeleteAuditable
{
    public bool IsDeleted { get; set; }
}