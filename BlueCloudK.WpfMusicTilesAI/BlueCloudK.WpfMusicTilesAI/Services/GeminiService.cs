using BlueCloudK.WpfMusicTilesAI.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlueCloudK.WpfMusicTilesAI.Services
{
    /// <summary>
    /// Service to generate beat maps using Google Gemini AI with OAuth authentication
    /// </summary>
    public class GeminiService : IGeminiService
    {
        private readonly IGoogleAuthService _authService;
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta";

        public GeminiService(IGoogleAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _httpClient = new HttpClient();
        }

        public async Task<BeatMap> GenerateBeatMapAsync(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be null or empty", nameof(description));

            // Ensure user is authenticated
            if (!_authService.IsAuthenticated)
            {
                throw new InvalidOperationException("User must be authenticated to use Gemini API");
            }

            var systemInstruction = @"You are a rhythm game AI composer. Your task is to create a playable beat map based on a user's song title or description.
    The beat map must be in JSON format and conform to the provided schema.
    - Analyze the provided song title/description to infer its style, tempo, and mood. If only a title is given, make a reasonable guess about the genre.
    - Generate a song duration that seems appropriate, typically between 45 and 90 seconds. A shorter duration is better for faster generation.
    - Create an engaging pattern of notes that reflects the song's inferred rhythm. Generate 2-4 notes per second for energetic songs, and 1-2 for calmer songs.
    - For about 15-20% of the notes, add a 'duration' property between 0.5 and 1.5 seconds to create long/sustain notes that fit the musical phrasing.
    - When creating long notes, ensure there is enough time before the next note in the same lane. The next note in the same lane must have a 'time' that is greater than the current note's 'time' + 'duration'.
    - Ensure note timings are musically coherent and rhythmically interesting. Avoid random placements.
    - Do not place notes at the exact same time on different lanes. Stagger them slightly to feel more natural.
    - Lanes must be integers from 1 to 4.
    - The final note's time should be at least 3 seconds before the total duration ends to allow for a concluding moment.

    Return the beat map in this JSON format:
    {
        ""metadata"": {
            ""title"": ""Creative title for the song"",
            ""duration"": 60.0,
            ""bpm"": 120
        },
        ""notes"": [
            {
                ""time"": 1.5,
                ""lane"": 1,
                ""duration"": null
            },
            {
                ""time"": 2.0,
                ""lane"": 2,
                ""duration"": 1.0
            }
        ]
    }";

            var prompt = $"Create a beat map for a song described as: \"{description}\"";

            try
            {
                // Get access token
                var credential = await _authService.GetCredentialAsync();
                var token = await credential.GetAccessTokenForRequestAsync();

                // Build request to Gemini API
                var fullPrompt = $"{systemInstruction}\n\n{prompt}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = fullPrompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 8192
                    }
                };

                var requestJson = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");

                // Set authorization header
                var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/models/gemini-2.0-flash-exp:generateContent")
                {
                    Content = content
                };
                request.Headers.Add("Authorization", $"Bearer {token}");

                // Send request
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var geminiResponse = JsonConvert.DeserializeObject<dynamic>(responseJson);

                // Extract text from response
                var jsonResponse = (string)(geminiResponse?.candidates?[0]?.content?.parts?[0]?.text ?? throw new Exception("Invalid response format from Gemini API"));
                jsonResponse = jsonResponse.Trim();

                // Remove markdown code blocks if present
                if (jsonResponse.StartsWith("```json"))
                {
                    jsonResponse = jsonResponse.Substring(7);
                }
                if (jsonResponse.StartsWith("```"))
                {
                    jsonResponse = jsonResponse.Substring(3);
                }
                if (jsonResponse.EndsWith("```"))
                {
                    jsonResponse = jsonResponse.Substring(0, jsonResponse.Length - 3);
                }
                jsonResponse = jsonResponse.Trim();

                var beatMap = JsonConvert.DeserializeObject<BeatMap>(jsonResponse);

                if (beatMap == null || beatMap.Notes == null)
                    throw new Exception("Failed to parse beat map from AI response");

                // Sort notes by time and add unique IDs
                beatMap.Notes = beatMap.Notes
                    .OrderBy(n => n.Time)
                    .Select((note, index) =>
                    {
                        note.Id = $"{note.Time}-{note.Lane}-{index}";
                        note.State = NoteState.Active;
                        note.Y = 0;
                        note.HoldProgress = 0;
                        return note;
                    })
                    .ToList();

                return beatMap;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to generate beat map: {ex.Message}", ex);
            }
        }
    }
}
