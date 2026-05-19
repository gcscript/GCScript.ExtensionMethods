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
}
