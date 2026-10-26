using ClyvoVetChallenge.Domain.Exceptions;

namespace ClyvoVetChallenge.Tests.Unit.Domain;

public class DomainExceptionTests
{
    [Fact]
    public void Construtor_ComMensagem_DeveDefinirMensagemCorretamente()
    {
        var mensagem = "Regra de negocio violada";

        var exception = new DomainException(mensagem);

        Assert.Equal(mensagem, exception.Message);
    }

    [Fact]
    public void ConstrutorNotFound_ComEntidadeEChave_DeveFormatarMensagemAmigavel()
    {
        var id = Guid.NewGuid();

        var exception = new NotFoundException("Animal", id);

        Assert.Contains("Animal", exception.Message);
        Assert.Contains(id.ToString(), exception.Message);
    }

    [Fact]
    public void ConstrutorBusinessRule_ComMensagemEInnerException_DevePreservarExcecaoInterna()
    {
        var inner = new InvalidOperationException("Operacao de banco falhou");

        var exception = new BusinessRuleException("Falha na regra de negocio", inner);

        Assert.Equal(inner, exception.InnerException);
    }
}
