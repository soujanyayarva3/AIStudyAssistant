using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace AIStudyAssistant.Application.Services;

public class OllamaService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    private const string GroqUrl =
        "https://api.groq.com/openai/v1/chat/completions";

    private const string Model =
        "llama-3.3-70b-versatile";

    public OllamaService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _apiKey =
            configuration["Groq:ApiKey"]
            ?? configuration["Groq__ApiKey"]
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            throw new InvalidOperationException(
                "Groq API key is not configured. " +
                "Set the Groq__ApiKey environment variable."
            );
        }

        _httpClient.Timeout =
            TimeSpan.FromMinutes(5);

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _apiKey
            );

        Console.WriteLine(
            "=========================================="
        );

        Console.WriteLine(
            "GROQ AI SERVICE INITIALIZED"
        );

        Console.WriteLine(
            $"Model: {Model}"
        );

        Console.WriteLine(
            "=========================================="
        );
    }

    // =====================================================
    // GENERIC AI RESPONSE
    // =====================================================

    public async Task<string> GenerateResponseAsync(
        string prompt)
    {
        var request = new
        {
            model = Model,

            messages = new[]
            {
                new
                {
                    role = "user",
                    content = prompt
                }
            },

            temperature = 0.7,

            max_tokens = 1000,

            stream = false
        };

        Console.WriteLine();
        Console.WriteLine(
            "========== AI GENERATION =========="
        );

        Console.WriteLine(
            "Calling Groq..."
        );

        HttpResponseMessage response;

        try
        {
            response =
                await _httpClient.PostAsJsonAsync(
                    GroqUrl,
                    request
                );
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(
                "GROQ CONNECTION ERROR:"
            );

            Console.WriteLine(
                ex.Message
            );

            throw new Exception(
                "Unable to connect to the Groq AI service.",
                ex
            );
        }

        Console.WriteLine(
            $"GROQ STATUS: {response.StatusCode}"
        );

        var responseText =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                "GROQ ERROR RESPONSE:"
            );

            Console.WriteLine(
                responseText
            );

            throw new Exception(
                $"Groq API returned {(int)response.StatusCode}: " +
                responseText
            );
        }

        try
        {
            using var document =
                JsonDocument.Parse(responseText);

            var content =
                document.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

            return content?.Trim()
                   ?? "No response";
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                "INVALID GROQ RESPONSE:"
            );

            Console.WriteLine(
                responseText
            );

            throw new Exception(
                "Unable to read the response from Groq.",
                ex
            );
        }
    }

    // =====================================================
    // GENERATE QUIZ
    // =====================================================

    public async Task<string> GenerateQuizzesAsync(
        string topic,
        int numberOfQuestions = 5,
        IEnumerable<string>? previousQuestions = null)
    {
        const int totalQuestions = 5;

        var oldQuestions =
            previousQuestions?
                .Where(q =>
                    !string.IsNullOrWhiteSpace(q))
                .Select(q => q.Trim())
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList()
            ?? new List<string>();

        // =====================================================
        // PREVIOUS QUESTIONS
        // =====================================================

        var previousQuestionText =
            oldQuestions.Count > 0
                ? string.Join(
                    "\n",
                    oldQuestions.Select(
                        (q, index) =>
                            $"{index + 1}. {q}"
                    )
                )
                : "There are no previous questions.";

        // =====================================================
        // PROMPT
        // =====================================================

        var prompt =
            "You are an expert multiple-choice quiz generator.\n\n" +

            $"Topic: {topic}\n\n" +

            $"Generate EXACTLY {totalQuestions} NEW and DIFFERENT " +
            "multiple-choice questions about this topic.\n\n" +

            "VERY IMPORTANT:\n" +

            "- Every generated question MUST be new.\n" +
            "- DO NOT repeat any question from the previous question list.\n" +
            "- Do not merely change punctuation or capitalization.\n" +
            "- Do not generate the same concept using almost identical wording.\n" +
            "- Generate questions from different concepts of the topic.\n" +
            "- Each question must have exactly 4 options.\n" +
            "- Options must be named optionA, optionB, optionC and optionD.\n" +
            "- Every question MUST contain correctAnswer.\n" +
            "- correctAnswer MUST contain exactly one letter: A, B, C or D.\n" +
            "- correctAnswer MUST correspond to the correct option.\n" +
            "- Do not provide explanations.\n" +
            "- Do not provide markdown.\n" +
            "- Return ONLY valid JSON.\n\n" +

            "PREVIOUS QUESTIONS - DO NOT REPEAT THESE:\n" +
            "------------------------------------------\n" +

            previousQuestionText +

            "\n------------------------------------------\n\n" +

            "Return EXACTLY this JSON structure:\n\n" +

            "{\n" +
            "  \"questions\": [\n" +
            "    {\n" +
            "      \"question\": \"What is Java?\",\n" +
            "      \"optionA\": \"A programming language\",\n" +
            "      \"optionB\": \"A database\",\n" +
            "      \"optionC\": \"An operating system\",\n" +
            "      \"optionD\": \"A web browser\",\n" +
            "      \"correctAnswer\": \"A\"\n" +
            "    }\n" +
            "  ]\n" +
            "}\n\n" +

            $"Generate exactly {totalQuestions} NEW questions.";

        // =====================================================
        // GROQ REQUEST
        // =====================================================

        var request = new
        {
            model = Model,

            messages = new[]
            {
                new
                {
                    role = "system",

                    content =
                        "You generate valid JSON only. " +
                        "Never use markdown or explanatory text."
                },

                new
                {
                    role = "user",

                    content = prompt
                }
            },

            temperature = 0.2,

            max_tokens = 3000,

            stream = false,

            response_format = new
            {
                type = "json_object"
            }
        };

        Console.WriteLine();
        Console.WriteLine(
            "========== QUIZ GENERATION START =========="
        );

        Console.WriteLine(
            $"Topic: {topic}"
        );

        Console.WriteLine(
            $"Requested questions: {totalQuestions}"
        );

        Console.WriteLine(
            $"Previous questions: {oldQuestions.Count}"
        );

        Console.WriteLine(
            $"Model: {Model}"
        );

        Console.WriteLine(
            "JSON mode: ENABLED"
        );

        Console.WriteLine(
            "Duplicate prevention: ENABLED"
        );

        Console.WriteLine(
            "============================================"
        );

        HttpResponseMessage response;

        try
        {
            response =
                await _httpClient.PostAsJsonAsync(
                    GroqUrl,
                    request
                );
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(
                "GROQ CONNECTION ERROR:"
            );

            Console.WriteLine(
                ex.Message
            );

            throw new Exception(
                "Unable to connect to the Groq AI service.",
                ex
            );
        }

        Console.WriteLine(
            $"GROQ STATUS: {response.StatusCode}"
        );

        var responseText =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                "GROQ ERROR RESPONSE:"
            );

            Console.WriteLine(
                responseText
            );

            throw new Exception(
                $"Groq API returned {(int)response.StatusCode}: " +
                responseText
            );
        }

        // =====================================================
        // EXTRACT AI CONTENT
        // =====================================================

        string answer;

        try
        {
            using var groqDocument =
                JsonDocument.Parse(responseText);

            answer =
                groqDocument.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString()
                    ?.Trim()
                    ?? string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                "INVALID GROQ RESPONSE:"
            );

            Console.WriteLine(
                responseText
            );

            throw new Exception(
                "Unable to read quiz response from Groq.",
                ex
            );
        }

        Console.WriteLine();
        Console.WriteLine(
            "========== RAW GROQ RESPONSE =========="
        );

        Console.WriteLine(
            answer
        );

        Console.WriteLine(
            "========================================"
        );

        if (string.IsNullOrWhiteSpace(answer))
        {
            throw new Exception(
                "Groq returned an empty quiz response."
            );
        }

        // =====================================================
        // VALIDATE JSON
        // =====================================================

        try
        {
            using var document =
                JsonDocument.Parse(answer);

            var root =
                document.RootElement;

            if (!root.TryGetProperty(
                    "questions",
                    out var questions))
            {
                throw new Exception(
                    "Groq response does not contain 'questions'."
                );
            }

            if (questions.ValueKind !=
                JsonValueKind.Array)
            {
                throw new Exception(
                    "'questions' must be a JSON array."
                );
            }

            var count =
                questions.GetArrayLength();

            Console.WriteLine(
                $"AI returned {count} questions."
            );

            if (count < totalQuestions)
            {
                throw new Exception(
                    $"AI generated only {count} questions. " +
                    $"Expected {totalQuestions}."
                );
            }

            // =================================================
            // VALIDATE QUESTIONS
            // =================================================

            for (int i = 0;
                 i < totalQuestions;
                 i++)
            {
                var question =
                    questions[i];

                var questionText =
                    GetString(
                        question,
                        "question"
                    );

                var optionA =
                    GetString(
                        question,
                        "optionA"
                    );

                var optionB =
                    GetString(
                        question,
                        "optionB"
                    );

                var optionC =
                    GetString(
                        question,
                        "optionC"
                    );

                var optionD =
                    GetString(
                        question,
                        "optionD"
                    );

                var correctAnswer =
                    GetString(
                        question,
                        "correctAnswer"
                    )
                    .ToUpperInvariant();

                if (string.IsNullOrWhiteSpace(
                    questionText))
                {
                    throw new Exception(
                        $"Question {i + 1} has an empty question."
                    );
                }

                if (string.IsNullOrWhiteSpace(
                    optionA))
                {
                    throw new Exception(
                        $"Question {i + 1} has an empty Option A."
                    );
                }

                if (string.IsNullOrWhiteSpace(
                    optionB))
                {
                    throw new Exception(
                        $"Question {i + 1} has an empty Option B."
                    );
                }

                if (string.IsNullOrWhiteSpace(
                    optionC))
                {
                    throw new Exception(
                        $"Question {i + 1} has an empty Option C."
                    );
                }

                if (string.IsNullOrWhiteSpace(
                    optionD))
                {
                    throw new Exception(
                        $"Question {i + 1} has an empty Option D."
                    );
                }

                if (correctAnswer != "A" &&
                    correctAnswer != "B" &&
                    correctAnswer != "C" &&
                    correctAnswer != "D")
                {
                    throw new Exception(
                        $"Question {i + 1} has invalid " +
                        $"correctAnswer: '{correctAnswer}'."
                    );
                }

                Console.WriteLine(
                    $"Question {i + 1}: " +
                    $"CorrectAnswer = {correctAnswer}"
                );
            }

            Console.WriteLine();
            Console.WriteLine(
                "QUIZ JSON VALIDATION SUCCESSFUL"
            );

            Console.WriteLine(
                "5 questions with correct answers received."
            );

            Console.WriteLine(
                "=========================================="
            );
        }
        catch (JsonException ex)
        {
            Console.WriteLine(
                "INVALID JSON FROM GROQ:"
            );

            Console.WriteLine(
                ex.Message
            );

            throw new Exception(
                "Groq returned invalid quiz JSON.",
                ex
            );
        }

        return answer;
    }

    // =====================================================
    // GET STRING FROM JSON
    // =====================================================

    private static string GetString(
        JsonElement element,
        string propertyName)
    {
        if (!element.TryGetProperty(
                propertyName,
                out var property))
        {
            return string.Empty;
        }

        return property
            .GetString()
            ?.Trim()
            ?? string.Empty;
    }
}