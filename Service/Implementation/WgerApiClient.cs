using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Domain.Dto;
using Domain.ExternalModels;
using Domain.WgerApiResponse;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.Interface;
using WgerApiSettings = Domain.Configurations.WgerApiSettings;

namespace Service.Implementation;

public class WgerApiClient : IWgerApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WgerApiClient> _logger;
    private readonly WgerApiSettings _settings;

    public WgerApiClient(HttpClient httpClient, ILogger<WgerApiClient> logger, IOptions<WgerApiSettings> settings)
    {
        _httpClient = httpClient;
        _logger = logger;
        _settings = settings.Value;
    }


    public async Task<List<ExerciseWgerDto>> GetAllExercisesAsync()
    {
        var allExercise = new List<ExerciseWgerDto>();
        var url = $"exerciseinfo/?language=2&limit=50";

        while (url != null)
        {
            _logger.LogInformation("Fetching exercises from {url}", url);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var wgerData = await response.Content.ReadFromJsonAsync<WgerApiResponse>();
            
            foreach (var raw in wgerData.Results)
            {
                var mapped = MapToExercisesDto(raw);
                if (mapped != null)
                    allExercise.Add(mapped);
            }

            url = wgerData.Next != null
                ? wgerData.Next.Replace(_httpClient.BaseAddress!.ToString(), string.Empty)
                : null;

        }
        
        _logger.LogInformation("Fetched {Count} exercises from Wger", allExercise.Count);
        return allExercise;
    }

    private ExerciseWgerDto? MapToExercisesDto(WgerRawExercise raw)
    {
        var englishTranslation = raw.Translations.FirstOrDefault(t => t.Language == 2);
        if (englishTranslation == null || string.IsNullOrWhiteSpace(englishTranslation.Name))
            return null;

        return new ExerciseWgerDto
        {
            ExternalId = GuidHelper.FromLegacyId("Exercise", raw.Id),
            Name = englishTranslation.Name,
            Description = StripHtml(englishTranslation.Description ?? string.Empty),
            MuscleGroup = raw.Muscles.FirstOrDefault()?.Name ?? raw.Category?.Name ?? "Unspecified",
            Equipment = raw.Equipment?.FirstOrDefault()?.Name ?? "None"
        };
    }
    
    private static string StripHtml(string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty).Trim();
    }
}