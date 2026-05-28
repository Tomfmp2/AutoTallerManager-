using Domain.Common;

namespace Domain.Entities;

public class PersonDocument : BaseEntity
{
    public int PersonId { get; set; }
    public int DocumentTypeId { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;  
    public bool IsPrimary { get; set; } = false;

    // Navegacion
    public Person? Person { get; set; }
    public DocumentType? DocumentType { get; set; }
}
