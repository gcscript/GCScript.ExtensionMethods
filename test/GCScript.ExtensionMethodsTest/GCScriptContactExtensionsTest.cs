using GCScript.ExtensionMethods;

namespace GCScript.ExtensionMethodsTest;

public class GCScriptContactExtensionsTest {
	[Theory]
	[InlineData("20040-020", "20040020", "20040-020")]
	[InlineData("20040020", "20040020", "20040-020")]
	[InlineData("20.040-020", "20040020", "20040-020")]
	[InlineData(" 01310-100 ", "01310100", "01310-100")]
	public void Cep_ValidValues(string value, string digits, string masked) {
		Assert.True(value.IsValidCep());
		Assert.Equal(digits, value.ToCep());
		Assert.Equal(masked, value.ToCepWithMask());
	}

	[Theory]
	[InlineData("2004002")]
	[InlineData("200400201")]
	[InlineData("00000-000")]
	[InlineData("2004A-020")]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Cep_InvalidValues(string? value) {
		Assert.False(value.IsValidCep());
		Assert.Equal(string.Empty, value.ToCep());
		Assert.Equal(string.Empty, value.ToCepWithMask());
	}

	[Theory]
	[InlineData("(21) 99999-8888", "21999998888", "(21) 99999-8888")]
	[InlineData("21999998888", "21999998888", "(21) 99999-8888")]
	[InlineData("+55 21 99999-8888", "21999998888", "(21) 99999-8888")]
	[InlineData("5521999998888", "21999998888", "(21) 99999-8888")]
	[InlineData("(11) 3333-4444", "1133334444", "(11) 3333-4444")]
	[InlineData("552133334444", "2133334444", "(21) 3333-4444")]
	public void BrazilianPhone_ValidValues(string value, string digits, string masked) {
		Assert.True(value.IsValidBrazilianPhone());
		Assert.Equal(digits, value.ToBrazilianPhone());
		Assert.Equal(masked, value.ToBrazilianPhoneWithMask());
	}

	[Theory]
	[InlineData("(20) 99999-8888")] // DDD que não existe
	[InlineData("(21) 89999-8888")] // 11 dígitos sem o 9 do celular
	[InlineData("(21) 9999-8888")] // fixo começando com 9
	[InlineData("(21) 1333-4444")] // fixo começando com 1
	[InlineData("999998888")]
	[InlineData("219999988881")]
	[InlineData("(21) 9999A-8888")]
	[InlineData("")]
	[InlineData(null)]
	public void BrazilianPhone_InvalidValues(string? value) {
		Assert.False(value.IsValidBrazilianPhone());
		Assert.Equal(string.Empty, value.ToBrazilianPhone());
		Assert.Equal(string.Empty, value.ToBrazilianPhoneWithMask());
	}
}
