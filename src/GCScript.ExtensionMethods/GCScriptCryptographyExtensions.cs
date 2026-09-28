using System.Security.Cryptography;
using System.Text;
#if NETFRAMEWORK
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
#endif

namespace GCScript.ExtensionMethods;

public static class GCScriptCryptographyExtensions {
	// Fixos, e não lidos de AesGcm, para o pacote ter o mesmo formato em todos os alvos: o que um cifra, o outro decifra.
	private const int NonceSize = 12;
	private const int TagSize = 16;

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

		byte[] package = new byte[NonceSize + plain.Length + TagSize];
		byte[] nonce = new byte[NonceSize];
		using (var rng = RandomNumberGenerator.Create()) { rng.GetBytes(nonce); }
		Buffer.BlockCopy(nonce, 0, package, 0, NonceSize);

#if NETFRAMEWORK
		// O BouncyCastle devolve ciphertext + tag juntos, que é exatamente o trecho do pacote depois do nonce.
		GcmBlockCipher gcm = CreateGcm(forEncryption: true, keyBytes, nonce);
		int written = gcm.ProcessBytes(plain, 0, plain.Length, package, NonceSize);
		gcm.DoFinal(package, NonceSize + written);
#else
		Span<byte> cipher = package.AsSpan(NonceSize, plain.Length);
		Span<byte> tag = package.AsSpan(NonceSize + plain.Length, TagSize);

		using var aes = new AesGcm(keyBytes, TagSize);
		aes.Encrypt(nonce, plain, cipher, tag);
#endif

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

		if (package.Length < NonceSize + TagSize) {
			throw new ArgumentException("Package is too small to contain a valid AES-GCM payload.", nameof(cipher));
		}

		int cipherLength = package.Length - NonceSize - TagSize;
		byte[] plain = new byte[cipherLength];

#if NETFRAMEWORK
		byte[] nonce = new byte[NonceSize];
		Buffer.BlockCopy(package, 0, nonce, 0, NonceSize);

		GcmBlockCipher gcm = CreateGcm(forEncryption: false, keyBytes, nonce);
		try {
			int written = gcm.ProcessBytes(package, NonceSize, cipherLength + TagSize, plain, 0);
			gcm.DoFinal(plain, written);
		}
		catch (InvalidCipherTextException ex) {
			// Mesma família de exceção do AesGcm, para quem chama tratar adulteração igual em todos os alvos.
			throw new CryptographicException("The computed authentication tag did not match the input authentication tag.", ex);
		}
#else
		ReadOnlySpan<byte> nonce = package.AsSpan(0, NonceSize);
		ReadOnlySpan<byte> cipherBytes = package.AsSpan(NonceSize, cipherLength);
		ReadOnlySpan<byte> tag = package.AsSpan(NonceSize + cipherLength, TagSize);

		using var aes = new AesGcm(keyBytes, TagSize);
		aes.Decrypt(nonce, cipherBytes, tag, plain);
#endif

		return Encoding.UTF8.GetString(plain);
	}

#if NETFRAMEWORK
	private static GcmBlockCipher CreateGcm(bool forEncryption, byte[] keyBytes, byte[] nonce) {
		// O BouncyCastle recusaria a chave com ArgumentException; o AesGcm recusa com CryptographicException, e é esse contrato que vale.
		if (keyBytes.Length is not 16 and not 24 and not 32) {
			throw new CryptographicException("Specified key is not a valid size for this algorithm.");
		}
		var gcm = new GcmBlockCipher(new AesEngine());
		gcm.Init(forEncryption, new AeadParameters(new KeyParameter(keyBytes), TagSize * 8, nonce));
		return gcm;
	}
#endif
}
