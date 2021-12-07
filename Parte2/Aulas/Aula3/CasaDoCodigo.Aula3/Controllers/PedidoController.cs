using CasaDoCodigo.Aula3.Models;
using CasaDoCodigo.Aula3.Models.ViewModels;
using CasaDoCodigo.Aula3.Repositories;
using CasaDoCodigo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CasaDoCodigo.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IProdutoRepository produtoRepository;
        private readonly IPedidoRepository pedidoRepository;
        private readonly IItemPedidoRepository itemPedidoRepository;

        public PedidoController(IProdutoRepository produtoRepository,
            IPedidoRepository pedidoRepository, IItemPedidoRepository itemPedidoRepository)
        {
            this.produtoRepository = produtoRepository;
            this.pedidoRepository = pedidoRepository;
            this.itemPedidoRepository = itemPedidoRepository;
        }

        public IActionResult Carrossel()
        {
            return View(produtoRepository.GetProdutos());
        }

        public IActionResult Carrinho(string codigo)
        {
            if (!string.IsNullOrEmpty(codigo))
            {
                //Adicionando o código do produto ao Carrinho
                pedidoRepository.AddItem(codigo);
            }

            List<ItemPedido> itens = pedidoRepository.GetPedido().Itens;
            CarrinhoViewModel carrinhoViewModel = new CarrinhoViewModel(itens);

            return base.View(carrinhoViewModel);
        }

        public IActionResult Cadastro()
        {
            return View();
        }

        public IActionResult Resumo()
        {
            Pedido pedido = pedidoRepository.GetPedido();

            return View(pedido);
        }

        //atributo de método
        [HttpPost]
        public UpdateQuantidadeResponse UpdateQuantidade([FromBody] ItemPedido itemPedido)  //atributo vem pelo corpo da requisição
        {
            return pedidoRepository.UpdateQuantidade(itemPedido);
        }
    }
}
