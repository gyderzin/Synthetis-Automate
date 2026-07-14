using SkiaSharp;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using BackEnd_Synthetis.DTOs.Requests;

namespace BackEnd_Synthetis.Services;

public class DocumentoService
{
    // --------------------------------------------------------
    // SUBSTITUIR TEXTOS
    // --------------------------------------------------------
    public void SubstituirTexto(
        MemoryStream stream,
        Dictionary<string, object> dados
    )
    {
        stream.Position = 0;

        using var document =
            WordprocessingDocument.Open(
                stream,
                true
            );

        // BODY
        ProcessarElementos(
            document.MainDocumentPart?
                .Document?
                .Body,
            dados
        );

        // HEADERS
        foreach (
            var headerPart
            in document.MainDocumentPart?
                .HeaderParts
                ?? []
        )
        {
            ProcessarElementos(
                headerPart.Header,
                dados
            );
        }

        // FOOTERS
        foreach (
            var footerPart
            in document.MainDocumentPart?
                .FooterParts
                ?? []
        )
        {
            ProcessarElementos(
                footerPart.Footer,
                dados
            );
        }

        document.MainDocumentPart?
            .Document?
            .Save();

        stream.Position = 0;
    }

    // --------------------------------------------------------
    // SUBSTITUIR IMAGENS
    // --------------------------------------------------------
    public void SubstituirImagens(
        MemoryStream stream,
        Dictionary<string, object> dados
    )
    {
        stream.Position = 0;

        using var document =
            WordprocessingDocument.Open(
                stream,
                true
            );

        var mainPart =
            document.MainDocumentPart;

        if (mainPart == null)
            return;

        // BODY
        ProcessarImagens(
            mainPart.Document?.Body,
            mainPart,
            dados
        );

        // HEADERS
        foreach (
            var headerPart
            in mainPart.HeaderParts
        )
        {
            ProcessarImagens(
                headerPart.Header,
                mainPart,
                dados
            );
        }

        // FOOTERS
        foreach (
            var footerPart
            in mainPart.FooterParts
        )
        {
            ProcessarImagens(
                footerPart.Footer,
                mainPart,
                dados
            );
        }

        mainPart.Document?.Save();

        stream.Position = 0;
    }

    // --------------------------------------------------------
    // PROCESSA TEXTOS
    // --------------------------------------------------------
    private void ProcessarElementos(
        OpenXmlElement? root,
        Dictionary<string, object> dados
    )
    {
        if (root == null)
            return;

        var paragraphs =
            root.Descendants<Paragraph>();

        foreach (var paragraph in paragraphs)
        {
            var textoCompleto =
                paragraph.InnerText;

            var textoAlterado =
                textoCompleto;

            foreach (var item in dados)
            {
                var placeholder =
                    $"{{{{{item.Key}}}}}";

                var valor =
                    item.Value?.ToString()
                    ?? string.Empty;

                // IGNORA IMAGENS
                if (EhImagemBase64(valor))
                {
                    continue;
                }

                // IGNORA VAZIOS
                if (
                    string.IsNullOrWhiteSpace(
                        valor
                    )
                    || valor == "null"
                )
                {
                    continue;
                }

                textoAlterado =
                    textoAlterado.Replace(
                        placeholder,
                        valor
                    );
            }

            // SE HOUVE ALTERAÇÃO
            if (textoAlterado != textoCompleto)
            {
                var runs =
                    paragraph.Elements<Run>()
                        .ToList();

                if (!runs.Any())
                    continue;

                var primeiroRun =
                    runs.First();

                var primeiroTexto =
                    primeiroRun
                        .GetFirstChild<Text>();

                if (primeiroTexto != null)
                {
                    primeiroTexto.Text =
                        textoAlterado;
                }

                // REMOVE RUNS ANTIGOS
                foreach (
                    var run
                    in runs.Skip(1)
                )
                {
                    run.Remove();
                }
            }
        }
    }

    // --------------------------------------------------------
    // PROCESSA IMAGENS
    // --------------------------------------------------------
    private void ProcessarImagens(
     OpenXmlElement? root,
     MainDocumentPart mainPart,
     Dictionary<string, object> dados
 )
    {
        if (root == null)
            return;

        var paragraphs =
            root.Descendants<Paragraph>();

        foreach (var paragraph in paragraphs)
        {
            var texto =
                paragraph.InnerText
                    .Replace("–", "-")
                    .Replace("—", "-")
                    .Trim();

            Console.WriteLine(
                $"PARAGRAFO: {texto}"
            );

            foreach (var item in dados)
            {
                var valor =
                    item.Value?.ToString()
                    ?? string.Empty;

                // IGNORA SE NÃO FOR BASE64
                if (!EhImagemBase64(valor))
                    continue;

                var chave =
                    item.Key
                        .Replace("–", "-")
                        .Replace("—", "-")
                        .Trim();

                var placeholder =
                    $"{{{{{chave}}}}}";

                Console.WriteLine(
                    $"PROCURANDO: {placeholder}"
                );

                if (!texto.Contains(placeholder))
                    continue;

                Console.WriteLine(
                    $"PLACEHOLDER ENCONTRADO: {placeholder}"
                );

                InserirImagemNoParagrafo(
                 paragraph,
                mainPart,
                valor
            );
            }
        }
    }

    // --------------------------------------------------------
    // INSERIR IMAGEM
    // --------------------------------------------------------
    public void InserirImagemNoParagrafo(
    Paragraph paragraph,
    MainDocumentPart mainPart,
    string base64
)
    {
        // =========================================
        // REMOVE TODOS OS RUNS
        // =========================================

        paragraph.RemoveAllChildren<Run>();

        // =========================================
        // REMOVE PREFIXO BASE64
        // =========================================

        var base64Data =
            Regex.Replace(
                base64,
                @"^data:image\/[a-zA-Z]+;base64,",
                string.Empty
            );

        var imageBytes =
            Convert.FromBase64String(
                base64Data
            );

        // =========================================
        // TAMANHO PROPORCIONAL
        // =========================================

        const long emusPorCm = 360000;

        const double larguraMaxCm = 15;

        const double alturaMaxCm = 10;

       using var bitmap = SKBitmap.Decode(imageBytes);

        double larguraOriginal = bitmap.Width;
        double alturaOriginal = bitmap.Height;

        double larguraMaxEmus =
            larguraMaxCm * emusPorCm;

        double alturaMaxEmus =
            alturaMaxCm * emusPorCm;

        double escalaLargura =
            larguraMaxEmus / larguraOriginal;

        double escalaAltura =
            alturaMaxEmus / alturaOriginal;

        double escala =
            Math.Min(
                escalaLargura,
                escalaAltura
            );

        long larguraFinal =
            (long)(
                larguraOriginal * escala
            );

        long alturaFinal =
            (long)(
                alturaOriginal * escala
            );

        // =========================================
        // IMAGE PART
        // =========================================

        var imagePart =
            mainPart.AddImagePart(
                ImagePartType.Jpeg
            );

        using (
            var stream =
                new MemoryStream(imageBytes)
        )
        {
            imagePart.FeedData(stream);
        }

        var relationshipId =
            mainPart.GetIdOfPart(
                imagePart
            );

        // =========================================
        // DRAWING
        // =========================================

        var drawing =
            new Drawing(
                new DW.Inline(
                    new DW.Extent()
                    {
                        Cx = larguraFinal,
                        Cy = alturaFinal
                    },

                    new DW.EffectExtent()
                    {
                        LeftEdge = 0L,
                        TopEdge = 0L,
                        RightEdge = 0L,
                        BottomEdge = 0L
                    },

                    new DW.DocProperties()
                    {
                        Id = (UInt32Value)1U,
                        Name = "Imagem"
                    },

                    new DW.NonVisualGraphicFrameDrawingProperties(
                        new A.GraphicFrameLocks()
                        {
                            NoChangeAspect = true
                        }
                    ),

                    new A.Graphic(
                        new A.GraphicData(
                            new PIC.Picture(
                                new PIC.NonVisualPictureProperties(
                                    new PIC.NonVisualDrawingProperties()
                                    {
                                        Id = (UInt32Value)0U,
                                        Name = "Imagem"
                                    },

                                    new PIC.NonVisualPictureDrawingProperties()
                                ),

                                new PIC.BlipFill(
                                    new A.Blip()
                                    {
                                        Embed = relationshipId
                                    },

                                    new A.Stretch(
                                        new A.FillRectangle()
                                    )
                                ),

                                new PIC.ShapeProperties(
                                    new A.Transform2D(
                                        new A.Offset()
                                        {
                                            X = 0L,
                                            Y = 0L
                                        },

                                        new A.Extents()
                                        {
                                            Cx = larguraFinal,
                                            Cy = alturaFinal
                                        }
                                    ),

                                    new A.PresetGeometry(
                                        new A.AdjustValueList()
                                    )
                                    {
                                        Preset =
                                            A.ShapeTypeValues.Rectangle
                                    }
                                )
                            )
                        )
                        {
                            Uri =
                                "http://schemas.openxmlformats.org/drawingml/2006/picture"
                        }
                    )
                )
            );

        // =========================================
        // CENTRALIZA
        // =========================================

        paragraph.ParagraphProperties =
            new ParagraphProperties(
                new Justification()
                {
                    Val =
                        JustificationValues.Center
                }
            );

        // =========================================
        // INSERE NOVO RUN LIMPO
        // =========================================

        var run =
            new Run(drawing);

        paragraph.AppendChild(run);

        Console.WriteLine(
            "IMAGEM INSERIDA COM SUCESSO"
        );
    }


    // --------------------------------------------------------
    // VERIFICA BASE64 IMAGEM
    // --------------------------------------------------------
    private bool EhImagemBase64(
        string valor
    )
    {
        return
            valor.StartsWith(
                "data:image"
            )
            &&
            valor.Contains(
                ";base64,"
            );
    }

    // --------------------------------------------------------
    // INSERIR PENDÊNCIAS
    // --------------------------------------------------------
    public void InserirPendencias(
        MemoryStream stream,
        List<PendenciaRequest> pendencias,
        string chavePendencia
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                chavePendencia
            )
        )
        {
            return;
        }

        stream.Position = 0;

        using var document =
            WordprocessingDocument.Open(
                stream,
                true
            );

        var mainPart =
            document.MainDocumentPart;

        if (mainPart == null)
            return;

        var placeholder =
            $"{{{{{chavePendencia}}}}}";

        // BODY
        ProcessarPendencias(
            mainPart.Document?.Body,
            mainPart,
            placeholder,
            pendencias
        );

        // HEADERS
        foreach (
            var headerPart
            in mainPart.HeaderParts
        )
        {
            ProcessarPendencias(
                headerPart.Header,
                mainPart,
                placeholder,
                pendencias
            );
        }

        // FOOTERS
        foreach (
            var footerPart
            in mainPart.FooterParts
        )
        {
            ProcessarPendencias(
                footerPart.Footer,
                mainPart,
                placeholder,
                pendencias
            );
        }

        mainPart.Document?.Save();

        stream.Position = 0;
    }

    // --------------------------------------------------------
    // PROCESSA PENDÊNCIAS
    // --------------------------------------------------------
    private void ProcessarPendencias(
        OpenXmlElement? root,
        MainDocumentPart mainPart,
        string placeholder,
        List<PendenciaRequest> pendencias
    )
    {
        if (root == null)
            return;

        var paragraphs =
            root.Descendants<Paragraph>();

        foreach (var paragraph in paragraphs)
        {
            if (
                !paragraph.InnerText.Contains(
                    placeholder
                )
            )
            {
                continue;
            }

            // REMOVE PLACEHOLDER
            paragraph.RemoveAllChildren<Run>();

            // SEM PENDÊNCIAS
            if (
                pendencias == null
                || !pendencias.Any()
            )
            {
                paragraph.AppendChild(
                    new Run(
                        new Text(
                            "Nenhuma pendência relatada."
                        )
                    )
                );

                return;
            }

            // INSERE PENDÊNCIAS
            for (
                int i = 0;
                i < pendencias.Count;
                i++
            )
            {
                var pendencia =
                    pendencias[i];

                // -----------------------------------
                // TÍTULO
                // -----------------------------------

                var tituloParagraph =
                    new Paragraph();

                var tituloRun =
                    new Run();

                var tituloProps =
                    new RunProperties(
                        new Bold(),
                        new FontSize()
                        {
                            Val = "20"
                        }
                    );

                tituloRun.Append(
                    tituloProps
                );

                tituloRun.Append(
                    new Text(
                        $"{i + 1}. {pendencia.Titulo}"
                    )
                );

                tituloParagraph.Append(
                    tituloRun
                );

                paragraph.InsertBeforeSelf(
                    tituloParagraph
                );

                // -----------------------------------
                // DESCRIÇÃO
                // -----------------------------------

                if (
                    !string.IsNullOrWhiteSpace(
                        pendencia.Descricao
                    )
                )
                {
                    var descricaoParagraph =
                        new Paragraph();

                    var descricaoRun =
                        new Run();

                    descricaoRun.Append(
                        new RunProperties(
                            new FontSize()
                            {
                                Val = "20"
                            }
                        )
                    );

                    descricaoRun.Append(
                        new Text(
                            pendencia.Descricao
                        )
                    );

                    descricaoParagraph.Append(
                        descricaoRun
                    );

                    paragraph.InsertBeforeSelf(
                        descricaoParagraph
                    );
                }

                // -----------------------------------
                // IMAGEM
                // -----------------------------------

                if (
                    !string.IsNullOrWhiteSpace(
                        pendencia.Imagem
                    )
                    &&
                    EhImagemBase64(
                        pendencia.Imagem
                    )
                )
                {
                    var imagemParagraph =
                        new Paragraph();

                    paragraph.InsertBeforeSelf(
                        imagemParagraph
                    );

                    InserirImagemNoParagrafo(
                        imagemParagraph,
                        mainPart,
                        pendencia.Imagem
                    );
                }

                // ESPAÇO
                paragraph.InsertBeforeSelf(
                    new Paragraph(
                        new Run(
                            new Text(" ")
                        )
                    )
                );
            }

            // REMOVE PARÁGRAFO ORIGINAL
            paragraph.Remove();

            return;
        }
    }
}