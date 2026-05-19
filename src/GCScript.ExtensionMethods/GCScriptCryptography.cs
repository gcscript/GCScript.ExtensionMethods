using System.Security.Cryptography;

namespace GCScript.ExtensionMethods;

public static class GCScriptCryptography {

	/// <summary>
	/// [PT-BR] Gera uma chave AES aleatória criptograficamente segura e retorna em Base64.
	/// [EN] Generates a cryptographically secure random AES key and returns it as Base64.
	/// </summary>
	/// <param name="keySize">
	/// [PT-BR] Tamanho da chave em bits. Valores válidos: 128, 192 ou 256. Padrão: 256.
	/// [EN] Key size in bits. Valid values: 128, 192 or 256. Default: 256.
	/// </param>
	/// <returns>
	/// [PT-BR] Chave aleatória codificada em Base64.
	/// [EN] Random key encoded in Base64.
	/// </returns>
	public static string GenerateAesKey(int keySize = 256) {
		if (keySize is not 128 and not 192 and not 256) {
			throw new ArgumentOutOfRangeException(nameof(keySize), keySize, "Key size must be 128, 192 or 256 bits.");
		}
		byte[] key = RandomNumberGenerator.GetBytes(keySize / 8);
		return Convert.ToBase64String(key);
	}

	/// <summary>
	/// [PT-BR] Gera um IV (Initialization Vector) aleatório de 16 bytes para AES-CBC, codificado em Base64.
	/// [EN] Generates a random 16-byte Initialization Vector for AES-CBC, encoded in Base64.
	/// </summary>
	/// <returns>
	/// [PT-BR] IV aleatório codificado em Base64.
	/// [EN] Random IV encoded in Base64.
	/// </returns>
	public static string GenerateAesIv() {
		byte[] iv = RandomNumberGenerator.GetBytes(16);
		return Convert.ToBase64String(iv);
	}

	/// <summary>
	/// [PT-BR] Gera um nonce aleatório de 12 bytes para AES-GCM, codificado em Base64.
	/// [EN] Generates a random 12-byte nonce for AES-GCM, encoded in Base64.
	/// </summary>
	/// <returns>
	/// [PT-BR] Nonce aleatório codificado em Base64.
	/// [EN] Random nonce encoded in Base64.
	/// </returns>
	public static string GenerateAesNonce() {
		byte[] nonce = RandomNumberGenerator.GetBytes(AesGcm.NonceByteSizes.MaxSize);
		return Convert.ToBase64String(nonce);
	}
}
