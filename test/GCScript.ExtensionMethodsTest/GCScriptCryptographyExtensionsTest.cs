using System.Security.Cryptography;
using GCScript.ExtensionMethods;

namespace GCScript.ExtensionMethodsTest;

public class GCScriptCryptographyExtensionsTest {

	[Theory]
	[InlineData(128, 16)]
	[InlineData(192, 24)]
	[InlineData(256, 32)]
	public void GenerateAesKey_ProducesCorrectLength(int keySize, int expectedBytes) {
		string keyBase64 = GCScriptCryptography.GenerateAesKey(keySize);
		byte[] key = Convert.FromBase64String(keyBase64);
		Assert.Equal(expectedBytes, key.Length);
	}

	[Fact(DisplayName = "GenerateAesKey throws on invalid key size")]
	public void GenerateAesKey_InvalidSize_Throws() {
		Assert.Throws<ArgumentOutOfRangeException>(() => GCScriptCryptography.GenerateAesKey(123));
	}

	[Fact(DisplayName = "GenerateAesKey produces distinct values on consecutive calls")]
	public void GenerateAesKey_IsRandom() {
		Assert.NotEqual(GCScriptCryptography.GenerateAesKey(), GCScriptCryptography.GenerateAesKey());
	}

	// === AES-GCM ===

	[Theory]
	[InlineData("hello world")]
	[InlineData("texto com acentuação: ção, ã, é, ü")]
	[InlineData("emoji 🔐🚀 e símbolos !@#$%^&*()")]
	[InlineData("a")]
	[InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit.")]
	public void EncryptDecryptAes_RoundTrip(string original) {
		string key = GCScriptCryptography.GenerateAesKey();

		string package = original.AesEncrypt(key);
		string decrypted = package.AesDecrypt(key);

		Assert.Equal(original, decrypted);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void AesEncrypt_NullOrEmpty_ReturnsEmpty(string? input) {
		string key = GCScriptCryptography.GenerateAesKey();
		Assert.Equal(string.Empty, input.AesEncrypt(key));
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void AesDecrypt_NullOrEmpty_ReturnsEmpty(string? input) {
		string key = GCScriptCryptography.GenerateAesKey();
		Assert.Equal(string.Empty, input.AesDecrypt(key));
	}

	[Fact(DisplayName = "AES-GCM produces a different package on each call (random nonce)")]
	public void AesEncrypt_IsNonDeterministic() {
		string key = GCScriptCryptography.GenerateAesKey();
		string text = "mensagem secreta";

		string a = text.AesEncrypt(key);
		string b = text.AesEncrypt(key);

		Assert.NotEqual(a, b);
	}

	[Fact(DisplayName = "AES-GCM detects tampering and throws on decryption")]
	public void AesDecrypt_Tampered_Throws() {
		string key = GCScriptCryptography.GenerateAesKey();
		string package = "mensagem secreta".AesEncrypt(key);

		byte[] bytes = Convert.FromBase64String(package);
		bytes[bytes.Length / 2] ^= 0xFF;
		string tampered = Convert.ToBase64String(bytes);

		Assert.ThrowsAny<CryptographicException>(() => tampered.AesDecrypt(key));
	}

	[Fact(DisplayName = "AES-GCM throws when package is smaller than nonce+tag")]
	public void AesDecrypt_PackageTooSmall_Throws() {
		string key = GCScriptCryptography.GenerateAesKey();
		string tooSmall = Convert.ToBase64String(new byte[10]);
		Assert.Throws<ArgumentException>(() => tooSmall.AesDecrypt(key));
	}
}
