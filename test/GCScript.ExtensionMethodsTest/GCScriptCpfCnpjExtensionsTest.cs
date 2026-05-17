using GCScript.ExtensionMethods;

namespace GCScript.ExtensionMethodsTest;

public class GCScriptCpfCnpjExtensionsTest {
	[Theory]
	[InlineData("111.444.777-35")]
	[InlineData("11144477735")]
	[InlineData("529.982.247-25")]
	public void IsValidCPF_ValidValues(string value)
		=> Assert.True(value.IsValidCPF());

	[Theory]
	[InlineData("111.444.777-34")]
	[InlineData("11144477734")]
	[InlineData("00000000000")]
	[InlineData("11111111111")]
	[InlineData("123")]
	[InlineData("123456789012")]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void IsValidCPF_InvalidValues(string? value)
		=> Assert.False(value.IsValidCPF());

	[Theory]
	[InlineData("11.222.333/0001-81")]
	[InlineData("11222333000181")]
	[InlineData("04.252.011/0001-10")]
	public void IsValidCNPJ_ValidValues(string value)
		=> Assert.True(value.IsValidCNPJ());

	[Theory]
	[InlineData("11.222.333/0001-80")]
	[InlineData("11222333000180")]
	[InlineData("00000000000000")]
	[InlineData("11111111111111")]
	[InlineData("123")]
	[InlineData("123456789012345")]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void IsValidCNPJ_InvalidValues(string? value)
		=> Assert.False(value.IsValidCNPJ());

	[Theory]
	[InlineData("111.444.777-35", false, "11144477735")]
	[InlineData("11144477735", false, "11144477735")]
	[InlineData("11144477735", true, "111.444.777-35")]
	[InlineData("111.444.777-35", true, "111.444.777-35")]
	[InlineData("1", false, "00000000001")]
	[InlineData("1", true, "000.000.000-01")]
	public void ToCPF_FormatsCorrectly(string input, bool withMask, string expected)
		=> Assert.Equal(expected, input.ToCPF(withMask));

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData("abc")]
	public void ToCPF_EmptyForBlankOrNoDigits(string? input)
		=> Assert.Equal(string.Empty, input.ToCPF());

	[Theory]
	[InlineData("11.222.333/0001-81", false, "11222333000181")]
	[InlineData("11222333000181", false, "11222333000181")]
	[InlineData("11222333000181", true, "11.222.333/0001-81")]
	[InlineData("11.222.333/0001-81", true, "11.222.333/0001-81")]
	[InlineData("1", false, "00000000000001")]
	[InlineData("1", true, "00.000.000/0000-01")]
	public void ToCNPJ_FormatsCorrectly(string input, bool withMask, string expected)
		=> Assert.Equal(expected, input.ToCNPJ(withMask));

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData("abc")]
	public void ToCNPJ_EmptyForBlankOrNoDigits(string? input)
		=> Assert.Equal(string.Empty, input.ToCNPJ());
}
