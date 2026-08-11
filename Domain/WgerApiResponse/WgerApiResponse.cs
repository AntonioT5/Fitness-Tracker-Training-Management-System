using System.Text.Json.Serialization;

namespace Domain.WgerApiResponse;

public class WgerApiResponse
{
    [JsonPropertyName("count")] 
    public int Count { get; set; }
    
    [JsonPropertyName("next")] 
    public String? Next { get; set; }

    [JsonPropertyName("results")] 
    public List<WgerRawExercise> Results { get; set; } = new();
}

public class WgerRawExercise
{
    [JsonPropertyName("id")] 
    public int Id { get; set; }
    
    [JsonPropertyName("category")] 
    public WgerCategory? Category { get; set; }
    
    [JsonPropertyName("muscles")]
    public List<WgerMuscle> Muscles { get; set; } = new();
    
    [JsonPropertyName("equipment")]
    public List<WgerEquipment> Equipment { get; set; } = new();
    
    [JsonPropertyName("translations")]
    public List<WgerTranslation> Translations { get; set; } = new();
}

public class WgerCategory
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class WgerMuscle
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class WgerEquipment
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class WgerTranslation
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("language")]
    public int Language { get; set; }
}