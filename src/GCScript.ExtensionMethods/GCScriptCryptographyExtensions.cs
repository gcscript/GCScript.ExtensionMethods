using System.Security.Cryptography;
using System.Text;

namespace GCScript.ExtensionMethods;

public static class GCScriptCryptographyExtensions {

	/// <summary>
	/// [PT-BR] Criptografa o texto usando AES-CBC com padding PKCS7. Gera um IV aleatório
	/// internamente e retorna um pacote Base64 contendo: IV (16 bytes) + ciphertext.
	/// [EN] Encrypts the text using AES-CBC with PKCS7 padding. Generates a random IV
	/// internally and returns a Base64 package containing: IV (16 bytes) + ciphertext.
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
	/// [PT-BR] Pacote Base64 (IV + ciphertext), ou string vazia se a entrada for nula/vazia.
	/// [EN] Base64 package (IV + ciphertext), or empty string if input text is null/empty.
	/// </returns>
	public static string EncryptAesCbc(this string? text, string key) {
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }

		byte[] keyBytes = Convert.FromBase64String(key);
		byte[] plain = Encoding.UTF8.GetBytes(text);
		byte[] iv = RandomNumberGenerator.GetBytes(16);

		using var aes = Aes.Create();
		aes.Key = keyBytes;
		byte[] cipher = aes.EncryptCbc(plain, iv, PaddingMode.PKCS7);

		byte[] package = new byte[iv.Length + cipher.Length];
		Buffer.BlockCopy(iv, 0, package, 0, iv.Length);
		Buffer.BlockCopy(cipher, 0, package, iv.Length, cipher.Length);

		return Convert.ToBase64String(package);
	}

	/// <summary>
	/// [PT-BR] Descriptografa um pacote Base64 produzido por EncryptAesCbc (IV + ciphertext).
	/// [EN] Decrypts a Base64 package produced by EncryptAesCbc (IV + ciphertext).
	/// </summary>
	/// <param name="cipher">
	/// [PT-BR] Pacote Base64 contendo IV (16 bytes) + ciphertext.
	/// [EN] Base64 package containing IV (16 bytes) + ciphertext.
	/// </param>
	/// <param name="key">
	/// [PT-BR] Chave em Base64 (mesma usada na criptografia).
	/// [EN] Base64 key (same used for encryption).
	/// </param>
	/// <returns>
	/// [PT-BR] Texto em claro, ou string vazia se a entrada for nula/vazia.
	/// [EN] Plain text, or empty string if input is null/empty.
	/// </returns>
	public static string DecryptAesCbc(this string? cipher, string key) {
		if (cipher.IsNullOrWhiteSpace()) { return string.Empty; }

		byte[] keyBytes = Convert.FromBase64String(key);
		byte[] package = Convert.FromBase64String(cipher);

		const int ivSize = 16;
		if (package.Length < ivSize + ivSize) {
			throw new ArgumentException("Package is too small to contain a valid AES-CBC payload.", nameof(cipher));
		}

		ReadOnlySpan<byte> iv = package.AsSpan(0, ivSize);
		ReadOnlySpan<byte> cipherBytes = package.AsSpan(ivSize);

		using var aes = Aes.Create();
		aes.Key = keyBytes;
		byte[] plain = aes.DecryptCbc(cipherBytes, iv, PaddingMode.PKCS7);

		return Encoding.UTF8.GetString(plain);
	}

	/// <summary>
	/// [PT-BR] [LEGADO] Criptografa o texto usando AES-CBC com IV fornecido externamente.
	/// Mantido apenas para compatibilidade. Prefira a sobrecarga sem IV, que gera e empacota
	/// um IV aleatório a cada chamada (evita reuso de IV).
	/// [EN] [LEGACY] Encrypts the text using AES-CBC with an externally provided IV.
	/// Kept for compatibility only. Prefer the overload without IV, which generates and packs
	/// a random IV per call (avoids IV reuse).
	/// </summary>
	[Obsolete("Use EncryptAesCbc(key). This overload is kept only for compatibility with legacy data/flows that rely on an external IV.")]
	public static string EncryptAesCbc(this string? text, string key, string iv) {
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }

		byte[] keyBytes = Convert.FromBase64String(key);
		byte[] ivBytes = Convert.FromBase64String(iv);
		byte[] plain = Encoding.UTF8.GetBytes(text);

		using var aes = Aes.Create();
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;
		aes.Key = keyBytes;
		aes.IV = ivBytes;

		using var encryptor = aes.CreateEncryptor();
		byte[] cipher = encryptor.TransformFinalBlock(plain, 0, plain.Length);
		return Convert.ToBase64String(cipher);
	}

	/// <summary>
	/// [PT-BR] [LEGADO] Descriptografa um ciphertext em Base64 produzido com AES-CBC + PKCS7
	/// usando IV externo. Mantido para descriptografar dados cifrados pela API legada.
	/// [EN] [LEGACY] Decrypts a Base64 ciphertext produced with AES-CBC + PKCS7 using an
	/// external IV. Kept for decrypting data encrypted by the legacy API.
	/// </summary>
	[Obsolete("Use DecryptAesCbc(key) for data encrypted with the new API. This overload is kept only to decrypt legacy data.")]
	public static string DecryptAesCbc(this string? cipher, string key, string iv) {
		if (cipher.IsNullOrWhiteSpace()) { return string.Empty; }

		byte[] keyBytes = Convert.FromBase64String(key);
		byte[] ivBytes = Convert.FromBase64String(iv);
		byte[] cipherBytes = Convert.FromBase64String(cipher);

		using var aes = Aes.Create();
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;
		aes.Key = keyBytes;
		aes.IV = ivBytes;

		using var decryptor = aes.CreateDecryptor();
		byte[] plain = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
		return Encoding.UTF8.GetString(plain);
	}

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
	public static string EncryptAesGcm(this string? text, string key) {
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
	/// [PT-BR] Descriptografa um pacote Base64 produzido por EncryptAesGcm (nonce + ciphertext + tag).
	/// [EN] Decrypts a Base64 package produced by EncryptAesGcm (nonce + ciphertext + tag).
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
	public static string DecryptAesGcm(this string? cipher, string key) {
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
