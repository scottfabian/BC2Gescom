using System.ComponentModel.DataAnnotations.Schema;

namespace BC2Gescom.Model.BC;

public class CountryRegion
{
    [Column("id")]
    public string Id { get; set; } = string.Empty;

    [Column("code")]
    public string Abbreviation { get; set; } = string.Empty;

    [Column("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [Column("tracking")]
    public string Tracking { get; set; } = string.Empty;
}
