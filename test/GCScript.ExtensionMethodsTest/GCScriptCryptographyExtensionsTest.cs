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

	[Fact(DisplayName = "GenerateAesIv produces 16 bytes")]
	public void GenerateAesIv_Length() {
		byte[] iv = Convert.FromBase64String(GCScriptCryptography.GenerateAesIv());
		Assert.Equal(16, iv.Length);
	}

	[Fact(DisplayName = "GenerateAesIv produces distinct values on consecutive calls")]
	public void GenerateAesIv_IsRandom() {
		Assert.NotEqual(GCScriptCryptography.GenerateAesIv(), GCScriptCryptography.GenerateAesIv());
	}

	[Fact(DisplayName = "GenerateAesNonce produces 12 bytes")]
	public void GenerateAesNonce_Length() {
		byte[] nonce = Convert.FromBase64String(GCScriptCryptography.GenerateAesNonce());
		Assert.Equal(12, nonce.Length);
	}

	[Fact(DisplayName = "GenerateAesNonce produces distinct values on consecutive calls")]
	public void GenerateAesNonce_IsRandom() {
		Assert.NotEqual(GCScriptCryptography.GenerateAesNonce(), GCScriptCryptography.GenerateAesNonce());
	}

	// === AES-CBC nova API (IV interno empacotado) ===

	[Theory]
	[InlineData("hello world")]
	[InlineData("texto com acentuação: ção, ã, é, ü")]
	[InlineData("emoji 🔐🚀 e símbolos !@#$%^&*()")]
	[InlineData("a")]
	[InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.")]
	public void EncryptDecryptAesCbc_RoundTrip(string original) {
		string key = GCScriptCryptography.GenerateAesKey();

		string package = original.EncryptAesCbc(key);
		string decrypted = package.DecryptAesCbc(key);

		Assert.Equal(original, decrypted);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void EncryptAesCbc_NullOrEmpty_ReturnsEmpty(string? input) {
		string key = GCScriptCryptography.GenerateAesKey();
		Assert.Equal(string.Empty, input.EncryptAesCbc(key));
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void DecryptAesCbc_NullOrEmpty_ReturnsEmpty(string? input) {
		string key = GCScriptCryptography.GenerateAesKey();
		Assert.Equal(string.Empty, input.DecryptAesCbc(key));
	}

	[Fact(DisplayName = "AES-CBC produces a different package on each call (random IV)")]
	public void EncryptAesCbc_IsNonDeterministic() {
		string key = GCScriptCryptography.GenerateAesKey();
		string text = "mensagem secreta";

		string a = text.EncryptAesCbc(key);
		string b = text.EncryptAesCbc(key);

		Assert.NotEqual(a, b);
	}

	[Fact(DisplayName = "AES-CBC throws when package is smaller than IV + 1 block")]
	public void DecryptAesCbc_PackageTooSmall_Throws() {
		string key = GCScriptCryptography.GenerateAesKey();
		string tooSmall = Convert.ToBase64String(new byte[16]);
		Assert.Throws<ArgumentException>(() => tooSmall.DecryptAesCbc(key));
	}

	[Fact(DisplayName = "AES-CBC throws when decrypting with the wrong key")]
	public void DecryptAesCbc_WrongKey_Throws() {
		string key = GCScriptCryptography.GenerateAesKey();
		string wrongKey = GCScriptCryptography.GenerateAesKey();
		string package = "mensagem secreta".EncryptAesCbc(key);

		Assert.Throws<CryptographicException>(() => package.DecryptAesCbc(wrongKey));
	}

	// === AES-CBC API legada (IV externo) — mantida para compatibilidade ===

#pragma warning disable CS0618 // Member is obsolete

	[Theory]
	[InlineData("hello world")]
	[InlineData("texto com acentuação: ção, ã, é, ü")]
	[InlineData("emoji 🔐🚀 e símbolos !@#$%^&*()")]
	[InlineData("a")]
	public void EncryptDecryptAesCbc_Legacy_RoundTrip(string original) {
		string key = GCScriptCryptography.GenerateAesKey();
		string iv = GCScriptCryptography.GenerateAesIv();

		string cipher = original.EncryptAesCbc(key, iv);
		string decrypted = cipher.DecryptAesCbc(key, iv);

		Assert.Equal(original, decrypted);
	}

	[Fact(DisplayName = "AES-CBC legado com mesma key/IV produz ciphertext determinístico")]
	public void EncryptAesCbc_Legacy_IsDeterministic() {
		string key = GCScriptCryptography.GenerateAesKey();
		string iv = GCScriptCryptography.GenerateAesIv();
		string text = "mensagem secreta";

		string a = text.EncryptAesCbc(key, iv);
		string b = text.EncryptAesCbc(key, iv);

		Assert.Equal(a, b);
	}

	[Fact(DisplayName = "AES-CBC legado lança em key de tamanho inválido")]
	public void EncryptAesCbc_Legacy_InvalidKey_Throws() {
		string badKey = Convert.ToBase64String(new byte[10]);
		string iv = GCScriptCryptography.GenerateAesIv();
		Assert.Throws<CryptographicException>(() => "x".EncryptAesCbc(badKey, iv));
	}

	[Fact(DisplayName = "AES-CBC legado lança em IV de tamanho inválido")]
	public void EncryptAesCbc_Legacy_InvalidIv_Throws() {
		string key = GCScriptCryptography.GenerateAesKey();
		string badIv = Convert.ToBase64String(new byte[8]);
		Assert.Throws<CryptographicException>(() => "x".EncryptAesCbc(key, badIv));
	}

#pragma warning restore CS0618

	// === AES-GCM ===

	[Theory]
	[InlineData("hello world")]
	[InlineData("texto com acentuação: ção, ã, é, ü")]
	[InlineData("emoji 🔐🚀 e símbolos !@#$%^&*()")]
	[InlineData("a")]
	[InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit.")]
	public void EncryptDecryptAesGcm_RoundTrip(string original) {
		string key = GCScriptCryptography.GenerateAesKey();

		string package = original.EncryptAesGcm(key);
		string decrypted = package.DecryptAesGcm(key);

		Assert.Equal(original, decrypted);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void EncryptAesGcm_NullOrEmpty_ReturnsEmpty(string? input) {
		string key = GCScriptCryptography.GenerateAesKey();
		Assert.Equal(string.Empty, input.EncryptAesGcm(key));
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void DecryptAesGcm_NullOrEmpty_ReturnsEmpty(string? input) {
		string key = GCScriptCryptography.GenerateAesKey();
		Assert.Equal(string.Empty, input.DecryptAesGcm(key));
	}

	[Fact(DisplayName = "AES-GCM produces a different package on each call (random nonce)")]
	public void EncryptAesGcm_IsNonDeterministic() {
		string key = GCScriptCryptography.GenerateAesKey();
		string text = "mensagem secreta";

		string a = text.EncryptAesGcm(key);
		string b = text.EncryptAesGcm(key);

		Assert.NotEqual(a, b);
	}

	[Fact(DisplayName = "AES-GCM detects tampering and throws on decryption")]
	public void DecryptAesGcm_Tampered_Throws() {
		string key = GCScriptCryptography.GenerateAesKey();
		string package = "mensagem secreta".EncryptAesGcm(key);

		byte[] bytes = Convert.FromBase64String(package);
		bytes[bytes.Length / 2] ^= 0xFF;
		string tampered = Convert.ToBase64String(bytes);

		Assert.ThrowsAny<CryptographicException>(() => tampered.DecryptAesGcm(key));
	}

	[Fact(DisplayName = "AES-GCM throws when package is smaller than nonce+tag")]
	public void DecryptAesGcm_PackageTooSmall_Throws() {
		string key = GCScriptCryptography.GenerateAesKey();
		string tooSmall = Convert.ToBase64String(new byte[10]);
		Assert.Throws<ArgumentException>(() => tooSmall.DecryptAesGcm(key));
	}
}
