using FluentAssertions;
using hinos_api.Services;

namespace hinos_api.Tests.Services;

public class OneDriveSyncServiceTests
{
    [Theory]
    [InlineData("Grito de Guerra_Msg 37_PDF_DIVULGAÇÃO_Jacó_A necessidade de transformação", 37, "Jacó", "A necessidade de transformação")]
    [InlineData("Grito de Guerra_Msg 1_PDF_Teste Tema", 1, "Tema", "Teste Tema")]
    [InlineData("Grito de Guerra_Msg 42_PDF_Davi_Forte e Valente", 42, "Davi", "Forte e Valente")]
    [InlineData("grito de guerra_msg 10_pdf_jesus_Amor eterno", 10, "Jesus", "Amor eterno")]
    [InlineData("GRITO DE GUERRA_MSG 5_PDF_Pedro_Fé Inabalável", 5, "Pedro", "Fé Inabalável")]
    public void ParseWarCryFileName_ShouldExtractMessageNumberAndTheme(string fileName, int expectedMessageNumber, string? expectedTheme, string expectedTitle)
    {
        var (messageNumber, theme, title) = OneDriveSyncService.ParseWarCryFileName(fileName);

        messageNumber.Should().Be(expectedMessageNumber);
        theme.Should().Be(expectedTheme);
        title.Should().Contain(expectedTitle);
    }

    [Fact]
    public void ParseWarCryFileName_ShouldHandleNoMessageNumber()
    {
        var fileName = "Grito de Guerra_Título sem número";
        
        var (messageNumber, theme, title) = OneDriveSyncService.ParseWarCryFileName(fileName);

        messageNumber.Should().Be(0);
        title.Should().NotBeEmpty();
    }

    [Fact]
    public void ParseWarCryFileName_ShouldHandleNoTheme()
    {
        var fileName = "Grito de Guerra_Msg 15_PDF_Título Simples";
        
        var (messageNumber, theme, title) = OneDriveSyncService.ParseWarCryFileName(fileName);

        messageNumber.Should().Be(15);
        theme.Should().BeNull();
    }

    [Fact]
    public void ParseWarCryFileName_ShouldHandlePdfExtension()
    {
        var fileName = "Grito de Guerra_Msg 20_PDF_Josué_Venha.pdf";
        
        var (messageNumber, theme, title) = OneDriveSyncService.ParseWarCryFileName(fileName);

        messageNumber.Should().Be(20);
        theme.Should().Be("Josué");
    }
}
