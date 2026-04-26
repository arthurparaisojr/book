using Book.Application.Abstractions.Persistence;
using Book.Application.Contracts.Assuntos;
using Book.Application.Exceptions;
using Book.Application.Services.Assuntos;
using Book.Domain.Entities;

namespace Book.Api.Tests.Assuntos;

public sealed class AssuntoAppServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenDescricaoIsMissing()
    {
        var repository = new FakeAssuntoRepository();
        var service = new AssuntoAppService(repository);

        var action = async () => await service.CreateAsync(new CreateAssuntoRequest
        {
            Descricao = " "
        });

        await Assert.ThrowsAsync<ValidationException>(action);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnAssunto_WhenAssuntoExists()
    {
        var repository = new FakeAssuntoRepository();
        repository.Seed(new Assunto
        {
            CodAs = 1,
            Descricao = "DDD"
        });

        var service = new AssuntoAppService(repository);

        var response = await service.GetByIdAsync(1);

        Assert.Equal(1, response.CodAs);
        Assert.Equal("DDD", response.Descricao);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenAssuntoDoesNotExist()
    {
        var repository = new FakeAssuntoRepository();
        var service = new AssuntoAppService(repository);

        var action = async () => await service.UpdateAsync(
            99,
            new UpdateAssuntoRequest
            {
                Descricao = "Assunto inexistente"
            });

        await Assert.ThrowsAsync<NotFoundException>(action);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveAssunto_WhenAssuntoExists()
    {
        var repository = new FakeAssuntoRepository();
        repository.Seed(new Assunto
        {
            CodAs = 1,
            Descricao = "DDD"
        });

        var service = new AssuntoAppService(repository);

        await service.DeleteAsync(1);

        var assunto = await repository.GetByIdAsync(1);
        Assert.Null(assunto);
    }

    private sealed class FakeAssuntoRepository : IAssuntoRepository
    {
        private readonly List<Assunto> _assuntos = [];
        private int _nextId = 1;

        public Task<IReadOnlyList<Assunto>> ListAsync(ListAssuntosRequest request, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Assunto> result = _assuntos;
            return Task.FromResult(result);
        }

        public Task<Assunto?> GetByIdAsync(int codAs, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_assuntos.SingleOrDefault(x => x.CodAs == codAs));
        }

        public Task<int> CreateAsync(Assunto assunto, CancellationToken cancellationToken = default)
        {
            assunto.CodAs = _nextId++;
            _assuntos.Add(assunto);
            return Task.FromResult(assunto.CodAs);
        }

        public Task<bool> UpdateAsync(Assunto assunto, CancellationToken cancellationToken = default)
        {
            var index = _assuntos.FindIndex(x => x.CodAs == assunto.CodAs);
            if (index < 0)
            {
                return Task.FromResult(false);
            }

            _assuntos[index] = assunto;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(int codAs, CancellationToken cancellationToken = default)
        {
            var removed = _assuntos.RemoveAll(x => x.CodAs == codAs) > 0;
            return Task.FromResult(removed);
        }

        public void Seed(Assunto assunto)
        {
            _assuntos.Add(assunto);
            _nextId = Math.Max(_nextId, assunto.CodAs + 1);
        }
    }
}
