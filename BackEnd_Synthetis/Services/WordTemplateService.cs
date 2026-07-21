<<<<<<< HEAD
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;

namespace BackEnd_Synthetis.Services;

public class WordTemplateService
{
    public List<string> ExtrairVariaveis(Stream fileStream)
    {
        var textoCompleto = "";

        using var document =
            WordprocessingDocument.Open(fileStream, false);

        // Documento principal
        if (document.MainDocumentPart?.Document != null)
        {
            textoCompleto +=
                document.MainDocumentPart.Document.InnerText;
        }

        // Headers
        if (document.MainDocumentPart != null)
        {
            foreach (var headerPart in document.MainDocumentPart.HeaderParts)
            {
                if (headerPart.Header != null)
                {
                    textoCompleto +=
                        " " + headerPart.Header.InnerText;
                }
            }

            // Footers
            foreach (var footerPart in document.MainDocumentPart.FooterParts)
            {
                if (footerPart.Footer != null)
                {
                    textoCompleto +=
                        " " + footerPart.Footer.InnerText;
                }
            }
        }

        // Regex {{variavel}}
        var matches = Regex.Matches(
            textoCompleto,
            @"\{\{(.*?)\}\}"
        );

        var variaveis =
            matches
                .Select(m => m.Groups[1].Value.Trim())
                .Distinct()
                .ToList();

        return variaveis;
    }
=======
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;

namespace BackEnd_Synthetis.Services;

public class WordTemplateService
{
    public List<string> ExtrairVariaveis(Stream fileStream)
    {
        var textoCompleto = "";

        using var document =
            WordprocessingDocument.Open(fileStream, false);

        // Documento principal
        if (document.MainDocumentPart?.Document != null)
        {
            textoCompleto +=
                document.MainDocumentPart.Document.InnerText;
        }

        // Headers
        if (document.MainDocumentPart != null)
        {
            foreach (var headerPart in document.MainDocumentPart.HeaderParts)
            {
                if (headerPart.Header != null)
                {
                    textoCompleto +=
                        " " + headerPart.Header.InnerText;
                }
            }

            // Footers
            foreach (var footerPart in document.MainDocumentPart.FooterParts)
            {
                if (footerPart.Footer != null)
                {
                    textoCompleto +=
                        " " + footerPart.Footer.InnerText;
                }
            }
        }

        // Regex {{variavel}}
        var matches = Regex.Matches(
            textoCompleto,
            @"\{\{(.*?)\}\}"
        );

        var variaveis =
            matches
                .Select(m => m.Groups[1].Value.Trim())
                .Distinct()
                .ToList();

        return variaveis;
    }
>>>>>>> c7d2b61b066fb0d744f59c225dac19d723052154
}