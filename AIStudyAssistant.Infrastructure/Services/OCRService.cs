using Tesseract;

namespace AIStudyAssistant.Infrastructure.Services;

public class OCRService
{
    private readonly string _tessDataPath;

    public OCRService()
    {
        _tessDataPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "tessdata");
    }

    public string ExtractText(string imagePath)
    {
        using var engine = new TesseractEngine(_tessDataPath, "eng", EngineMode.Default);

        using var img = Pix.LoadFromFile(imagePath);

        using var page = engine.Process(img);

        return page.GetText();
    }
}