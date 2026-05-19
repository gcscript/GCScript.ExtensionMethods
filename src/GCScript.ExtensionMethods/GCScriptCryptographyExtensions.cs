using System.Security.Cryptography;
using System.Text;

namespace GCScript.ExtensionMethods;

public static class GCScriptCryptographyExtensions {

	/// <summary>
	/// [PT-BR] Criptografa o texto usando AES-GCM. Gera nonce aleatório internamente e retorna
	/// um pacote Base64 contendo: nonce (12 bytes) + ciphertext + tag (16 bytes).
	/// [EN] Encrypts the text using AES-GCM. Generates a random nonce internally and returns
	/// a Base64 package containing: nonce (12 bytes) + ciphertext + tag (16 bytes).
	/// </summary>
	/// <param name="text">
	/// [PT-BR] Texto em claro a ser criptografado.
	/// [EN] Plain text to encrypt.
	/// </param>
	/// <param name="key">
	/// [PT-BR] Chave em Base64 (16, 24 ou 32 bytes após decodificar).
	/// [EN] Base64 key (16, 24 or 32 bytes after decoding).
	/// </param>
	/// <returns>
	/// [PT-BR] Pacote Base64 (nonce + ciphertext + tag), ou string vazia se entrada nula/vazia.
	/// [EN] Base64 package (nonce + ciphertext + tag), or empty string if input is null/empty.
	/// </returns>
	public static string AesEncrypt(this string? text, string key) {
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }

		byte[] keyBytes = Convert.FromBase64String(key);
		byte[] plain = Encoding.UTF8.GetBytes(text);

		int nonceSize = AesGcm.NonceByteSizes.MaxSize;
		int tagSize = AesGcm.TagByteSizes.MaxSize;

		byte[] package = new byte[nonceSize + plain.Length + tagSize];
		Span<byte> nonce = package.AsSpan(0, nonceSize);
		Span<byte> cipher = package.AsSpan(nonceSize, plain.Length);
		Span<byte> tag = package.AsSpan(nonceSize + plain.Length, tagSize);

		RandomNumberGenerator.Fill(nonce);

		using var aes = new AesGcm(keyBytes, tagSize);
		aes.Encrypt(nonce, plain, cipher, tag);

		return Convert.ToBase64String(package);
	}

	/// <summary>
	/// [PT-BR] Descriptografa um pacote Base64 produzido por AesEncrypt (nonce + ciphertext + tag).
	/// [EN] Decrypts a Base64 package produced by AesEncrypt (nonce + ciphertext + tag).
	/// </summary>
	/// <param name="cipher">
	/// [PT-BR] Pacote Base64 contendo nonce (12) + ciphertext + tag (16).
	/// [EN] Base64 package containing nonce (12) + ciphertext + tag (16).
	/// </param>
	/// <param name="key">
	/// [PT-BR] Chave em Base64 (mesma usada na criptografia).
	/// [EN] Base64 key (same used for encryption).
	/// </param>
	/// <returns>
	/// [PT-BR] Texto em claro, ou string vazia se a entrada for nula/vazia.
	/// [EN] Plain text, or empty string if input is null/empty.
	/// </returns>
	public static string AesDecrypt(this string? cipher, string key) {
		if (cipher.IsNullOrWhiteSpace()) { return string.Empty; }

		byte[] keyBytes = Convert.FromBase64String(key);
		byte[] package = Convert.FromBase64String(cipher);

		int nonceSize = AesGcm.NonceByteSizes.MaxSize;
		int tagSize = AesGcm.TagByteSizes.MaxSize;

		if (package.Length < nonceSize + tagSize) {
			throw new ArgumentException("Package is too small to contain a valid AES-GCM payload.", nameof(cipher));
		}

		int cipherLength = package.Length - nonceSize - tagSize;
		ReadOnlySpan<byte> nonce = package.AsSpan(0, nonceSize);
		ReadOnlySpan<byte> cipherBytes = package.AsSpan(nonceSize, cipherLength);
		ReadOnlySpan<byte> tag = package.AsSpan(nonceSize + cipherLength, tagSize);

		byte[] plain = new byte[cipherLength];
		using var aes = new AesGcm(keyBytes, tagSize);
		aes.Decrypt(nonce, cipherBytes, tag, plain);

		return Encoding.UTF8.GetString(plain);
	}
}
