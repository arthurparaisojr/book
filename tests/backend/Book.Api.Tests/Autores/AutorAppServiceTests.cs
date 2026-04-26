using Book.Application.Abstractions.Persistence;
using Book.Application.Contracts.Autores;
using Book.Application.Exceptions;
using Book.Application.Services.Autores;
using Book.Domain.Entities;

namespace Book.Api.Tests.Autores;

public sealed class AutorAppServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenNomeIsMissing()
    {
        var repository = new FakeAutorRepository();
        var service = new AutorAppService(repository);

        var action = async () => await service.CreateAsync(new CreateAutorRequest
        {
            Nome = " "
        });

        await Assert.ThrowsAsync<ValidationException>(action);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnAutor_WhenAutorExists()
    {
        var repository = new FakeAutorRepository();
        repository.Seed(new Autor
        {
            CodAu = 1,
            Nome = "Autor teste"
        });

        var service = new AutorAppService(repository);

        var response = await service.GetByIdAsync(1);

        Assert.Equal(1, response.CodAu);
        Assert.Equal("Autor teste", response.Nome);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenAutorDoesNotExist()
    {
        var repository = new FakeAutorRepository();
        var service = new AutorAppService(repository);

        var action = async () => await service.UpdateAsync(
            99,
            new UpdateAutorRequest
            {
                Nome = "Autor inexistente"
            });

        await Assert.ThrowsAsync<NotFoundException>(action);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveAutor_WhenAutorExists()
    {
        var repository = new FakeAutorRepository();
        repository.Seed(new Autor
        {
            CodAu = 1,
            Nome = "Autor teste"
        });

        var service = new AutorAppService(repository);

        await service.DeleteAsync(1);

        var autor = await repository.GetByIdAsync(1);
        Assert.Null(autor);
    }

    private sealed class FakeAutorRepository : IAutorRepository
    {
        private readonly List<Autor> _autores = [];
        private int _nextId = 1;

        public Task<IReadOnlyList<Autor>> ListAsync(ListAutoresRequest request, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Autor> result = _autores;
            return Task.FromResult(result);
        }

        public Task<Autor?> GetByIdAsync(int codAu, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_autores.SingleOrDefault(x => x.CodAu == codAu));
        }

        public Task<int> CreateAsync(Autor autor, CancellationToken cancellationToken = default)
        {
            autor.CodAu = _nextId++;
            _autores.Add(autor);
            return Task.FromResult(autor.CodAu);
        }

        public Task<bool> UpdateAsync(Autor autor, CancellationToken cancellationToken = default)
        {
            var index = _autores.FindIndex(x => x.CodAu == autor.CodAu);
            if (index < 0)
            {
                return Task.FromResult(false);
            }

            _autores[index] = autor;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(int codAu, CancellationToken cancellationToken = default)
        {
            var removed = _autores.RemoveAll(x => x.CodAu == codAu) > 0;
            return Task.FromResult(removed);
        }

        public void Seed(Autor autor)
        {
            _autores.Add(autor);
            _nextId = Math.Max(_nextId, autor.CodAu + 1);
        }
    }
}
