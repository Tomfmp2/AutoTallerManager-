using Domain.Common;

namespace Domain.Entities;

public class PersonEmail : BaseEntity
{
    public int PersonId { get; set; }
    public int EmailDomainId { get; set; }
    public string EmailUser {  get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;

    // Navegacion 

    public Person? Person { get; set; }
    public EmailDomain? EmailDomain { get; set; }
}
