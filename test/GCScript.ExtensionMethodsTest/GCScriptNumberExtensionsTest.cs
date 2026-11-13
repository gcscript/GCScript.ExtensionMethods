using GCScript.ExtensionMethods;
using System.Globalization;

namespace GCScript.ExtensionMethodsTest;

public class GCScriptNumberExtensionsTest {
	[Theory]
	[InlineData("1234,56", "1234.56")]
	[InlineData("1.234,56", "1234.56")]
	[InlineData("1.234", "1234")]
	[InlineData("1.234.567,891", "1234567.891")]
	[InlineData("0012", "12")]
	[InlineData("-1.234,5", "-1234.5")]
	[InlineData("+7", "7")]
	[InlineData(",5", "0.5")]
	[InlineData("  42  ", "42")]
	[InlineData(" 1.500,00 ", "1500.00")]
	public void TryParseBrazilianNumber_ValidValues(string text, string expected) {
		Assert.True(text.TryParseBrazilianNumber(out decimal number));
		Assert.Equal(decimal.Parse(expected, CultureInfo.InvariantCulture), number);
	}

	[Theory]
	[InlineData("10.5")]      // ponto decimal: ambíguo no padrão brasileiro
	[InlineData("1.50")]      // grupo de milhar com 2 dígitos
	[InlineData("1.2345")]    // grupo de milhar com 4 dígitos
	[InlineData("12.34,5")]
	[InlineData("1,234,56")]
	[InlineData("R$ 10,00")]
	[InlineData("10%")]
	[InlineData("1 234")]
	[InlineData("-")]
	[InlineData(",")]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	[InlineData("SEM VALOR")]
	[InlineData("١٢")]        // dígitos árabes
	[InlineData("99999999999999999999999999999999")] // não cabe em decimal
	public void TryParseBrazilianNumber_InvalidValues(string? text) {
		Assert.False(text.TryParseBrazilianNumber(out decimal number));
		Assert.Equal(0m, number);
	}

	[Theory]
	[InlineData("R$13,40", "13.40")]
	[InlineData("R$ 1.234,56", "1234.56")]
	[InlineData("R$ 13,40", "13.40")]  // espaço rígido, comum no formato de moeda copiado
	[InlineData("-R$ 13,40", "-13.40")]
	[InlineData("R$ -13,40", "-13.40")]
	[InlineData("13,40 reais", "13.40")]
	[InlineData("1.234,56", "1234.56")]
	[InlineData("+7", "7")]
	[InlineData("1 234", "1234")]
	[InlineData("Pedido 123", "123")]       // amplo de propósito
	[InlineData("(13,40)", "13.40")]        // sem o sinal
	[InlineData("13,40%", "13.40")]         // sem a escala
	public void TryExtractBrazilianNumber_ValidValues(string text, string expected) {
		Assert.True(text.TryExtractBrazilianNumber(out decimal number));
		Assert.Equal(decimal.Parse(expected, CultureInfo.InvariantCulture), number);
	}

	[Theory]
	[InlineData("R$")]
	[InlineData("SEM VALOR")]
	[InlineData("R$ 10.5")]     // o que sobra continua estrito
	[InlineData("R$ 1,234,56")]
	[InlineData("13,40-")]
	[InlineData("10-20")]
	[InlineData("R$ -")]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	[InlineData("١٢")]          // dígitos árabes
	public void TryExtractBrazilianNumber_InvalidValues(string? text) {
		Assert.False(text.TryExtractBrazilianNumber(out decimal number));
		Assert.Equal(0m, number);
	}
}
