using System.Text.Json.Serialization;

namespace PizzaShop.Repository.ViewModels;

public class TokenWithSectionsResult
{
    public TokenDto Token { get; set; }
    public List<SectionDto> Sections { get; set; }
}

public class TokenDto
{
    [JsonPropertyName("WaitingTokenId")]
    public int WaitingTokenId { get; set; }

    [JsonPropertyName("CustomerId")]
    public int CustomerId { get; set; }

    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Email")]
    public string Email { get; set; }

    [JsonPropertyName("MobileNumber")]
    public string MobileNumber { get; set; }

    [JsonPropertyName("NoOfPersons")]
    public int NoOfPersons { get; set; }

    [JsonPropertyName("SectionId")]
    public int SectionId { get; set; }
}

public class SectionDto
{
    public int SectionId { get; set; }
    public string SectionName { get; set; }
    public string Description { get; set; }
    public bool IsDeleted { get; set; }
}
