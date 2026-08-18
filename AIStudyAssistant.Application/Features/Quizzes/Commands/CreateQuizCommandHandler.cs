using System.Text.Json;
using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Application.Services;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Quizzes.Commands;

public class CreateQuizCommandHandler
    : IRequestHandler<CreateQuizCommand, List<Quiz>>
{
    private readonly IQuizRepository _repository;
    private readonly OllamaService _ollamaService;

    public CreateQuizCommandHandler(
        IQuizRepository repository,
        OllamaService ollamaService)
    {
        _repository = repository;
        _ollamaService = ollamaService;
    }

    public async Task<List<Quiz>> Handle(
        CreateQuizCommand request,
        CancellationToken cancellationToken)
    {
        // =====================================================
        // SETTINGS
        // =====================================================

        const int totalQuestions = 5;
        const int maxAttempts = 4;

        if (string.IsNullOrWhiteSpace(request.Topic))
        {
            throw new Exception(
                "Quiz topic cannot be empty."
            );
        }

        var topic = request.Topic.Trim();

        Console.WriteLine();
        Console.WriteLine("==========================================");
        Console.WriteLine("        STARTING QUIZ GENERATION");
        Console.WriteLine("==========================================");
        Console.WriteLine($"Topic: {topic}");
        Console.WriteLine($"Questions requested: {totalQuestions}");
        Console.WriteLine($"Maximum attempts: {maxAttempts}");
        Console.WriteLine("==========================================");

        // =====================================================
        // GET PREVIOUS QUIZZES
        // =====================================================

        var previousQuizzes =
            await _repository.GetQuizzesByTopicAsync(
                request.UserId,
                topic
            );

        Console.WriteLine();
        Console.WriteLine("========== PREVIOUS QUIZZES ==========");
        Console.WriteLine($"UserId: {request.UserId}");
        Console.WriteLine($"Topic: {topic}");
        Console.WriteLine(
            $"Previous quiz count: {previousQuizzes.Count}"
        );

        // =====================================================
        // EXCLUDED QUESTIONS
        // =====================================================

        var excludedQuestions =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase
            );

        foreach (var quiz in previousQuizzes)
        {
            var normalized =
                NormalizeQuestion(quiz.Question);

            if (!string.IsNullOrWhiteSpace(normalized))
            {
                excludedQuestions.Add(normalized);
            }

            Console.WriteLine(
                $"OLD QUESTION: {quiz.Question}"
            );
        }

        Console.WriteLine("======================================");

        Console.WriteLine(
            $"Initial excluded questions: {excludedQuestions.Count}"
        );

        // =====================================================
        // IMPORTANT:
        //
        // THIS LIST ACCUMULATES NEW QUESTIONS ACROSS ATTEMPTS.
        // =====================================================

        var collectedQuestions =
            new List<QuizAIQuestion>();

        var collectedQuestionKeys =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase
            );

        // =====================================================
        // GENERATION LOOP
        // =====================================================

        for (
            int attempt = 1;
            attempt <= maxAttempts;
            attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // =================================================
            // HOW MANY QUESTIONS DO WE STILL NEED?
            // =================================================

            var remaining =
                totalQuestions -
                collectedQuestions.Count;

            if (remaining <= 0)
            {
                break;
            }

            Console.WriteLine();
            Console.WriteLine("==========================================");
            Console.WriteLine(
                $"          AI ATTEMPT {attempt}/{maxAttempts}"
            );
            Console.WriteLine("==========================================");

            Console.WriteLine(
                $"Already collected: {collectedQuestions.Count}"
            );

            Console.WriteLine(
                $"Still needed: {remaining}"
            );

            Console.WriteLine(
                $"Excluded questions: {excludedQuestions.Count}"
            );

            // =================================================
            // SEND EXCLUDED QUESTIONS TO OLLAMA
            // =================================================

            var excludedForAI =
                excludedQuestions.ToList();

            string aiResponse;

            try
            {
                aiResponse =
                    await _ollamaService.GenerateQuizzesAsync(
                        topic,
                        remaining,
                        excludedForAI
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "OLLAMA GENERATION ERROR:"
                );

                Console.WriteLine(
                    ex.Message
                );

                if (attempt == maxAttempts)
                {
                    throw;
                }

                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();

            // =================================================
            // CLEAN JSON
            // =================================================

            aiResponse =
                CleanJson(aiResponse);

            if (string.IsNullOrWhiteSpace(aiResponse))
            {
                Console.WriteLine(
                    "AI returned empty response."
                );

                continue;
            }

            // =================================================
            // DESERIALIZE
            // =================================================

            QuizAIResponse? aiQuizResponse;

            try
            {
                aiQuizResponse =
                    JsonSerializer.Deserialize<QuizAIResponse>(
                        aiResponse,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }
                    );
            }
            catch (JsonException ex)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "QUIZ JSON ERROR:"
                );

                Console.WriteLine(
                    ex.Message
                );

                if (attempt == maxAttempts)
                {
                    throw new Exception(
                        "Ollama returned invalid quiz JSON.",
                        ex
                    );
                }

                continue;
            }

            if (
                aiQuizResponse?.Questions == null ||
                aiQuizResponse.Questions.Count == 0
            )
            {
                Console.WriteLine(
                    "Ollama returned no questions."
                );

                continue;
            }

            Console.WriteLine();
            Console.WriteLine(
                $"AI returned {aiQuizResponse.Questions.Count} questions."
            );

            // =================================================
            // PROCESS AI QUESTIONS
            // =================================================

            foreach (
                var question
                in aiQuizResponse.Questions)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // ---------------------------------------------
                // BASIC VALIDATION
                // ---------------------------------------------

                if (!IsValidQuestion(question))
                {
                    Console.WriteLine(
                        "INVALID QUESTION SKIPPED."
                    );

                    continue;
                }

                // ---------------------------------------------
                // NORMALIZE
                // ---------------------------------------------

                var normalized =
                    NormalizeQuestion(
                        question.Question
                    );

                if (string.IsNullOrWhiteSpace(normalized))
                {
                    continue;
                }

                // ---------------------------------------------
                // DATABASE DUPLICATE
                // ---------------------------------------------

                if (excludedQuestions.Contains(normalized))
                {
                    Console.WriteLine(
                        $"DUPLICATE FROM DATABASE/PREVIOUS ATTEMPT: {question.Question}"
                    );

                    continue;
                }

                // ---------------------------------------------
                // DUPLICATE WITHIN CURRENT COLLECTION
                // ---------------------------------------------

                if (collectedQuestionKeys.Contains(normalized))
                {
                    Console.WriteLine(
                        $"DUPLICATE COLLECTED: {question.Question}"
                    );

                    continue;
                }

                // ---------------------------------------------
                // OPTIONS MUST BE UNIQUE
                // ---------------------------------------------

                var optionA =
                    CleanValue(question.OptionA);

                var optionB =
                    CleanValue(question.OptionB);

                var optionC =
                    CleanValue(question.OptionC);

                var optionD =
                    CleanValue(question.OptionD);

                var optionsAreUnique =
                    new[]
                    {
                        optionA,
                        optionB,
                        optionC,
                        optionD
                    }
                    .Select(NormalizeAnswer)
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase
                    )
                    .Count() == 4;

                if (!optionsAreUnique)
                {
                    Console.WriteLine(
                        $"DUPLICATE OPTIONS: {question.Question}"
                    );

                    continue;
                }

                // ---------------------------------------------
                // CORRECT ANSWER
                // ---------------------------------------------

                var correctAnswer =
                    CleanValue(
                        question.CorrectAnswer
                    )
                    .ToUpperInvariant();

                if (
                    correctAnswer != "A" &&
                    correctAnswer != "B" &&
                    correctAnswer != "C" &&
                    correctAnswer != "D"
                )
                {
                    Console.WriteLine(
                        $"INVALID ANSWER: {correctAnswer}"
                    );

                    continue;
                }

                // ---------------------------------------------
                // ACCEPT QUESTION
                // ---------------------------------------------

                collectedQuestions.Add(
                    question
                );

                collectedQuestionKeys.Add(
                    normalized
                );

                // Add to exclusion list so the next AI attempt
                // will not generate this question again.

                excludedQuestions.Add(
                    normalized
                );

                Console.WriteLine(
                    $"NEW QUESTION ACCEPTED: {question.Question}"
                );

                Console.WriteLine(
                    $"Collected: {collectedQuestions.Count}/{totalQuestions}"
                );

                // ---------------------------------------------
                // STOP IF WE HAVE ENOUGH
                // ---------------------------------------------

                if (
                    collectedQuestions.Count
                    >= totalQuestions
                )
                {
                    break;
                }
            }

            // =================================================
            // STATUS
            // =================================================

            Console.WriteLine();
            Console.WriteLine(
                $"New questions collected so far: {collectedQuestions.Count}/{totalQuestions}"
            );

            // =================================================
            // SUCCESS
            // =================================================

            if (
                collectedQuestions.Count
                >= totalQuestions
            )
            {
                break;
            }

            // =================================================
            // RETRY
            // =================================================

            if (attempt < maxAttempts)
            {
                Console.WriteLine();
                Console.WriteLine(
                    $"Still need {totalQuestions - collectedQuestions.Count} question(s)."
                );

                Console.WriteLine(
                    "Generating another DIFFERENT batch..."
                );
            }
        }

        // =====================================================
        // FINAL CHECK
        // =====================================================

        if (
            collectedQuestions.Count
            < totalQuestions
        )
        {
            throw new Exception(
                $"Unable to generate {totalQuestions} new questions for '{topic}'. " +
                $"Only {collectedQuestions.Count} unique questions were generated after {maxAttempts} attempts."
            );
        }

        // =====================================================
        // TAKE EXACTLY 5
        // =====================================================

        var finalQuestions =
            collectedQuestions
                .Take(totalQuestions)
                .ToList();

        // =====================================================
        // CREATE DATABASE ENTITIES
        // =====================================================

        var quizzes =
            new List<Quiz>();

        foreach (var aiQuiz in finalQuestions)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var question =
                CleanValue(
                    aiQuiz.Question
                );

            var optionA =
                CleanValue(
                    aiQuiz.OptionA
                );

            var optionB =
                CleanValue(
                    aiQuiz.OptionB
                );

            var optionC =
                CleanValue(
                    aiQuiz.OptionC
                );

            var optionD =
                CleanValue(
                    aiQuiz.OptionD
                );

            var correctAnswer =
                CleanValue(
                    aiQuiz.CorrectAnswer
                )
                .ToUpperInvariant();

            // =================================================
            // CONVERT LETTER TO ANSWER TEXT
            // =================================================

            string finalCorrectAnswer;

            switch (correctAnswer)
            {
                case "A":
                    finalCorrectAnswer = optionA;
                    break;

                case "B":
                    finalCorrectAnswer = optionB;
                    break;

                case "C":
                    finalCorrectAnswer = optionC;
                    break;

                case "D":
                    finalCorrectAnswer = optionD;
                    break;

                default:
                    throw new Exception(
                        $"Invalid correct answer '{correctAnswer}'."
                    );
            }

            // =================================================
            // CREATE ENTITY
            // =================================================

            var quiz =
                new Quiz
                {
                    Title = topic,

                    Question = question,

                    OptionA = optionA,

                    OptionB = optionB,

                    OptionC = optionC,

                    OptionD = optionD,

                    CorrectAnswer =
                        finalCorrectAnswer,

                    Score = 1,

                    UserId =
                        request.UserId,

                    CreatedDate =
                        DateTime.UtcNow
                };

            quizzes.Add(
                quiz
            );
        }

        // =====================================================
        // SAVE TO DATABASE
        // =====================================================

        var savedQuizzes =
            new List<Quiz>();

        foreach (var quiz in quizzes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var saved =
                await _repository.CreateQuizAsync(
                    quiz
                );

            savedQuizzes.Add(
                saved
            );
        }

        // =====================================================
        // SUCCESS
        // =====================================================

        Console.WriteLine();
        Console.WriteLine(
            "=========================================="
        );

        Console.WriteLine(
            $"SUCCESS: Saved {savedQuizzes.Count}/{totalQuestions} NEW questions."
        );

        Console.WriteLine(
            "=========================================="
        );

        return savedQuizzes;
    }

    // =========================================================
    // VALIDATE QUESTION
    // =========================================================

    private static bool IsValidQuestion(
        QuizAIQuestion? question)
    {
        if (question == null)
            return false;

        if (
            string.IsNullOrWhiteSpace(
                question.Question
            )
        )
            return false;

        if (
            string.IsNullOrWhiteSpace(
                question.OptionA
            )
        )
            return false;

        if (
            string.IsNullOrWhiteSpace(
                question.OptionB
            )
        )
            return false;

        if (
            string.IsNullOrWhiteSpace(
                question.OptionC
            )
        )
            return false;

        if (
            string.IsNullOrWhiteSpace(
                question.OptionD
            )
        )
            return false;

        if (
            string.IsNullOrWhiteSpace(
                question.CorrectAnswer
            )
        )
            return false;

        var answer =
            question.CorrectAnswer
                .Trim()
                .ToUpperInvariant();

        return
            answer == "A" ||
            answer == "B" ||
            answer == "C" ||
            answer == "D";
    }

    // =========================================================
    // CLEAN JSON
    // =========================================================

    private static string CleanJson(
        string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            return string.Empty;

        response =
            response.Trim();

        response =
            response.Replace(
                "```json",
                "",
                StringComparison.OrdinalIgnoreCase
            );

        response =
            response.Replace(
                "```",
                "",
                StringComparison.OrdinalIgnoreCase
            );

        response =
            response.Trim();

        var firstBrace =
            response.IndexOf('{');

        var lastBrace =
            response.LastIndexOf('}');

        if (
            firstBrace >= 0 &&
            lastBrace > firstBrace
        )
        {
            response =
                response.Substring(
                    firstBrace,
                    lastBrace -
                    firstBrace +
                    1
                );
        }

        return response.Trim();
    }

    // =========================================================
    // NORMALIZE QUESTION
    // =========================================================

    private static string NormalizeQuestion(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return string.Join(
            " ",
            value
                .Trim()
                .ToLowerInvariant()
                .Split(
                    new[]
                    {
                        ' ',
                        '\t',
                        '\r',
                        '\n'
                    },
                    StringSplitOptions.RemoveEmptyEntries
                )
        );
    }

    // =========================================================
    // NORMALIZE ANSWER
    // =========================================================

    private static string NormalizeAnswer(
    string? value)
{
    if (string.IsNullOrWhiteSpace(value))
        return string.Empty;

    return string.Join(
        " ",
        value
            .Trim()
            .ToLowerInvariant()
            .Split(
                new[]
                {
                    ' ',
                    '\t',
                    '\r',
                    '\n'
                },
                StringSplitOptions.RemoveEmptyEntries
            )
    );
}
    // =========================================================
    // CLEAN VALUE
    // =========================================================

    private static string CleanValue(
        string? value)
    {
        return value?.Trim()
               ?? string.Empty;
    }

    // =========================================================
    // AI RESPONSE
    // =========================================================

    private class QuizAIResponse
    {
        public List<QuizAIQuestion> Questions { get; set; }
            = new();
    }

    private class QuizAIQuestion
    {
        public string Question { get; set; }
            = string.Empty;

        public string OptionA { get; set; }
            = string.Empty;

        public string OptionB { get; set; }
            = string.Empty;

        public string OptionC { get; set; }
            = string.Empty;

        public string OptionD { get; set; }
            = string.Empty;

        public string CorrectAnswer { get; set; }
            = string.Empty;
    }
}