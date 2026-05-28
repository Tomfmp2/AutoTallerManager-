using Domain.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Domain.Entities;

public class PersonPhone : BaseEntity
{
    public int PersonId { get; set; }
    public int PhoneCodeId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;

    // Navegacion
    public Person? Person { get; set; }
    public PhoneCode? PhoneCode { get; set; }
}
