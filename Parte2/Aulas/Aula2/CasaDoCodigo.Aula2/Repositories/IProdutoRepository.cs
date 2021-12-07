using CasaDoCodigo.Models;
using System.Collections.Generic;

namespace CasaDoCodigo.Aula2.Repositories
{
    public interface IProdutoRepository
    {
        void SaveProdutos(List<Livro> livros);
        IList<Produto> GetProdutos();
    }
}