using Book.Application.Abstractions.Persistence;
using Book.Application.Contracts.Livros;
using Book.Application.Exceptions;
using Book.Application.Services.Livros;
using Book.Domain.Entities;

namespace Book.Api.Tests.Livros;

public sealed class LivroAppServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenTituloIsMissing()
    {
        var repository = new FakeLivroRepository();
        var service = new LivroAppService(repository);

        var action = async () => await service.CreateAsync(new CreateLivroRequest
        {
            Titulo = " ",
            Editora = "Editora A",
            Edicao = 1,
            AnoPublicacao = "2024",
            Valor = 10
        });

        await Assert.ThrowsAsync<ValidationException>(action);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnLivro_WhenLivroExists()
    {
        var repository = new FakeLivroRepository();
        repository.Seed(new Livro
        {
            Codl = 1,
            Titulo = "Livro teste",
            Editora = "Editora teste",
            Edicao = 1,
            AnoPublicacao = "2024",
            Valor = 99.90m
        });

        var service = new LivroAppService(repository);

        var response = await service.GetByIdAsync(1);

        Assert.Equal(1, response.Codl);
        Assert.Equal("Livro teste", response.Titulo);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenLivroDoesNotExist()
    {
        var repository = new FakeLivroRepository();
        var service = new LivroAppService(repository);

        var action = async () => await service.UpdateAsync(
            99,
            new UpdateLivroRequest
            {
                Titulo = "Livro inexistente",
                Editora = "Editora teste",
                Edicao = 1,
                AnoPublicacao = "2024",
                Valor = 50
            });

        await Assert.ThrowsAsync<NotFoundException>(action);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveLivro_WhenLivroExists()
    {
        var repository = new FakeLivroRepository();
        repository.Seed(new Livro
        {
            Codl = 1,
            Titulo = "Livro teste",
            Editora = "Editora teste",
            Edicao = 1,
            AnoPublicacao = "2024",
            Valor = 99.90m
        });

        var service = new LivroAppService(repository);

        await service.DeleteAsync(1);

        var livro = await repository.GetByIdAsync(1);
        Assert.Null(livro);
    }

    private sealed class FakeLivroRepository : ILivroRepository
    {
        private readonly List<Livro> _livros = [];
        private int _nextId = 1;

        public Task<IReadOnlyList<Livro>> ListAsync(ListLivrosRequest request, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Livro> result = _livros;
            return Task.FromResult(result);
        }

        public Task<Livro?> GetByIdAsync(int codl, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_livros.SingleOrDefault(x => x.Codl == codl));
        }

        public Task<int> CreateAsync(Livro livro, CancellationToken cancellationToken = default)
        {
            livro.Codl = _nextId++;
            _livros.Add(livro);
            return Task.FromResult(livro.Codl);
        }

        public Task<bool> UpdateAsync(Livro livro, CancellationToken cancellationToken = default)
        {
            var index = _livros.FindIndex(x => x.Codl == livro.Codl);
            if (index < 0)
            {
                return Task.FromResult(false);
            }

            _livros[index] = livro;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(int codl, CancellationToken cancellationToken = default)
        {
            var removed = _livros.RemoveAll(x => x.Codl == codl) > 0;
            return Task.FromResult(removed);
        }

        public void Seed(Livro livro)
        {
            _livros.Add(livro);
            _nextId = Math.Max(_nextId, livro.Codl + 1);
        }
    }
}
