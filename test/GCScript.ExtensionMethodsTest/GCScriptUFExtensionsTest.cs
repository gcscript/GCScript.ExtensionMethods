using GCScript.ExtensionMethods;
using GCScript.ExtensionMethods.Enums;

namespace GCScript.ExtensionMethodsTest;

/// <summary>
/// Unidades Federativas: reconhecer a sigla ou o nome do estado e devolver o nome para exibir.
/// </summary>
public class GCScriptUFExtensionsTest
{
    public static TheoryData<EUF> TodasAsUFs()
    {
        var dados = new TheoryData<EUF>();
        foreach (EUF uf in Enum.GetValues(typeof(EUF)))
        {
            dados.Add(uf);
        }

        return dados;
    }

    [Fact]
    public void EUF_TemAs27UnidadesFederativas()
    {
        Assert.Equal(27, Enum.GetValues(typeof(EUF)).Length);
        Assert.False(Enum.IsDefined(typeof(EUF), 0));
    }

    [Theory]
    [MemberData(nameof(TodasAsUFs))]
    public void TryToUF_SiglaEmQualquerCaixa_Reconhece(EUF uf)
    {
        var sigla = uf.ToString();

        Assert.True(sigla.TryToUF(out var maiuscula));
        Assert.True($"  {sigla.ToLowerInvariant()} ".TryToUF(out var minuscula));
        Assert.Equal(uf, maiuscula);
        Assert.Equal(uf, minuscula);
    }

    [Theory]
    [MemberData(nameof(TodasAsUFs))]
    public void TryToUF_NomeDoEstado_IdaEVolta(EUF uf)
    {
        Assert.True(uf.GetStateName().TryToUF(out var reconhecida));
        Assert.Equal(uf, reconhecida);
    }

    [Theory]
    [InlineData("Rio de Janeiro", EUF.RJ)]
    [InlineData("SAO PAULO", EUF.SP)]
    [InlineData("são   paulo", EUF.SP)]
    [InlineData("espirito santo", EUF.ES)]
    [InlineData("Mato Grosso", EUF.MT)]
    [InlineData("Mato Grosso do Sul", EUF.MS)]
    [InlineData("PARA", EUF.PA)]
    [InlineData("Paraná", EUF.PR)]
    [InlineData("Distrito Federal", EUF.DF)]
    public void TryToUF_NomeSemAcentoOuComEspacos_Reconhece(string texto, EUF esperada)
    {
        Assert.True(texto.TryToUF(out var uf));
        Assert.Equal(esperada, uf);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("XX")]
    [InlineData("R J")]
    [InlineData("RJ.")]
    [InlineData("Rio")]
    [InlineData("Janeiro")]
    [InlineData("Rio Grande")]
    [InlineData("1")]
    [InlineData("19")]
    [InlineData("0")]
    public void TryToUF_TextoQueNaoEUF_Recusa(string? texto)
    {
        Assert.False(texto.TryToUF(out var uf));
        Assert.Equal(default, uf);
        Assert.False(texto.IsValidUF());
    }

    [Theory]
    [InlineData(EUF.RJ, "Rio de Janeiro")]
    [InlineData(EUF.SP, "São Paulo")]
    [InlineData(EUF.PI, "Piauí")]
    [InlineData(EUF.RO, "Rondônia")]
    public void GetStateName_UF_DevolveONomeComAcentos(EUF uf, string esperado)
    {
        Assert.Equal(esperado, uf.GetStateName());
    }

    [Fact]
    public void GetStateName_ValorQueNaoEUF_LancaExcecao()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => default(EUF).GetStateName());
        Assert.Throws<ArgumentOutOfRangeException>(() => ((EUF)99).GetStateName());
    }
}
