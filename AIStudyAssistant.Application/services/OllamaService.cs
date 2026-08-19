using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace AIStudyAssistant.Application.Services;

public class OllamaService
{
    private readonly HttpClient _httpClient;

    public OllamaService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        // =====================================================
        // OLLAMA URL
        // =====================================================
        //
        // Local Docker:
        // http://host.docker.internal:11434/
        //
        // Render:
        // Set Ollama:BaseUrl as an environment variable/configuration
        // to a publicly reachable Ollama endpoint.
        //
        var ollamaUrl =
            configuration["Ollama:BaseUrl"]
            ?? "http://host.docker.internal:11434/";

        if (!ollamaUrl.EndsWith("/"))
        {
            ollamaUrl += "/";
        }

        _httpClient.BaseAddress = new Uri(ollamaUrl);

        _httpClient.Timeout =
            TimeSpan.FromMinutes(5);

        Console.WriteLine(
            $"OLLAMA BASE URL: {ollamaUrl}"
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
            model = "llama3.2:latest",

            prompt = prompt,

            stream = false,

            options = new
            {
                temperature = 0.7,

                num_predict = 500
            }
        };

        Console.WriteLine();
        Console.WriteLine(
            "========== AI GENERATION =========="
        );

        Console.WriteLine(
            "Calling Ollama..."
        );

        HttpResponseMessage response;

        try
        {
            response =
                await _httpClient.PostAsJsonAsync(
                    "api/generate",
                    request
                );
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(
                "OLLAMA CONNECTION ERROR:"
            );

            Console.WriteLine(
                ex.Message
            );

            throw new Exception(
                "Unable to connect to the configured AI service.",
                ex
            );
        }

        Console.WriteLine(
            $"OLLAMA STATUS: {response.StatusCode}"
        );

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content
                .ReadFromJsonAsync<OllamaResponse>();

        return result?.response?.Trim()
               ?? "No response";
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
                .Where(q => !string.IsNullOrWhiteSpace(q))
                .Select(q => q.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
            ?? new List<string>();

        // =====================================================
        // BUILD PREVIOUS QUESTION LIST
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

            $"Generate EXACTLY {totalQuestions} NEW and DIFFERENT multiple-choice questions about this topic.\n\n" +

            "VERY IMPORTANT:\n" +

            "- Every generated question MUST be new.\n" +
            "- DO NOT repeat any question from the previous question list.\n" +
            "- Do not merely change punctuation or capitalization of an old question.\n" +
            "- Do not generate the same concept using almost identical wording.\n" +
            "- Generate questions from different concepts of the topic.\n" +
            "- Each question must have exactly 4 options.\n" +
            "- Options must be named optionA, optionB, optionC and optionD.\n" +
            "- Every question MUST contain correctAnswer.\n" +
            "- correctAnswer MUST contain exactly one letter: A, B, C or D.\n" +
            "- correctAnswer MUST correspond to the correct option.\n" +
            "- Do not provide explanations.\n" +
            "- Do not provide markdown.\n" +
            "- Do not add any text outside the JSON.\n\n" +

            "PREVIOUS QUESTIONS - DO NOT REPEAT THESE:\n" +
            "------------------------------------------\n" +
            previousQuestionText +
            "\n------------------------------------------\n\n" +

            "The final JSON MUST contain exactly this structure:\n\n" +

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
        // JSON SCHEMA
        // =====================================================

        var format = new
        {
            type = "object",

            properties = new
            {
                questions = new
                {
                    type = "array",

                    items = new
                    {
                        type = "object",

                        properties = new
                        {
                            question = new
                            {
                                type = "string"
                            },

                            optionA = new
                            {
                                type = "string"
                            },

                            optionB = new
                            {
                                type = "string"
                            },

                            optionC = new
                            {
                                type = "string"
                            },

                            optionD = new
                            {
                                type = "string"
                            },

                            correctAnswer = new
                            {
                                type = "string",

                                @enum = new[]
                                {
                                    "A",
                                    "B",
                                    "C",
                                    "D"
                                }
                            }
                        },

                        required = new[]
                        {
                            "question",
                            "optionA",
                            "optionB",
                            "optionC",
                            "optionD",
                            "correctAnswer"
                        }
                    }
                }
            },

            required = new[]
            {
                "questions"
            }
        };

        // =====================================================
        // OLLAMA REQUEST
        // =====================================================

        var request = new
        {
            model = "llama3.2:latest",

            prompt = prompt,

            stream = false,

            format = format,

            options = new
            {
                temperature = 0.2,

                num_predict = 2500
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
            "Model: llama3.2:latest"
        );

        Console.WriteLine(
            "JSON schema enforcement: ENABLED"
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
                    "api/generate",
                    request
                );
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(
                "OLLAMA CONNECTION ERROR:"
            );

            Console.WriteLine(
                ex.Message
            );

            throw new Exception(
                "Unable to connect to the configured AI service.",
                ex
            );
        }

        Console.WriteLine(
            $"OLLAMA STATUS: {response.StatusCode}"
        );

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content
                .ReadFromJsonAsync<OllamaResponse>();

        var answer =
            result?.response?.Trim()
            ?? string.Empty;

        Console.WriteLine();
        Console.WriteLine(
            "========== RAW OLLAMA RESPONSE =========="
        );

        Console.WriteLine(
            answer
        );

        Console.WriteLine(
            "=========================================="
        );

        if (string.IsNullOrWhiteSpace(answer))
        {
            throw new Exception(
                "Ollama returned an empty quiz response."
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
                    "Ollama response does not contain 'questions'."
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
                    $"AI generated only {count} questions. Expected {totalQuestions}."
                );
            }

            // =================================================
            // VALIDATE EVERY QUESTION
            // =================================================

            for (int i = 0; i < totalQuestions; i++)
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

                if (string.IsNullOrWhiteSpace(optionA))
                {
                    throw new Exception(
                        $"Question {i + 1} has an empty Option A."
                    );
                }

                if (string.IsNullOrWhiteSpace(optionB))
                {
                    throw new Exception(
                        $"Question {i + 1} has an empty Option B."
                    );
                }

                if (string.IsNullOrWhiteSpace(optionC))
                {
                    throw new Exception(
                        $"Question {i + 1} has an empty Option C."
                    );
                }

                if (string.IsNullOrWhiteSpace(optionD))
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
                        $"Question {i + 1} has invalid correctAnswer: '{correctAnswer}'."
                    );
                }

                Console.WriteLine(
                    $"Question {i + 1}: CorrectAnswer = {correctAnswer}"
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
                "INVALID JSON FROM OLLAMA:"
            );

            Console.WriteLine(
                ex.Message
            );

            throw new Exception(
                "Ollama returned invalid quiz JSON.",
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

    // =====================================================
    // OLLAMA RESPONSE
    // =====================================================

    private class OllamaResponse
    {
        public string response { get; set; } = string.Empty;
    }
}