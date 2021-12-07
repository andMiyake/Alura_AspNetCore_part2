using CasaDoCodigo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Aula1.Repositories
{
    public interface IPedidoRepository
    {
        Pedido GetPedido();
        void AddItem(string codigo);
    }

    public class PedidoRepository : BaseRepository<Pedido>, IPedidoRepository
    {
        private readonly IHttpContextAccessor contextAccessor;

        public PedidoRepository(ApplicationContext contexto, 
            IHttpContextAccessor contextAccessor) : base(contexto)
        {
            this.contextAccessor = contextAccessor;
        }

        //Adiciona código do item ao carrinho
        public void AddItem(string codigo)
        {
            //verificando se o produto existe na tabela de Produtos
            var produto = contexto.Set<Produto>()
                            .Where(p => p.Codigo == codigo)
                            .SingleOrDefault();

            if (produto == null)
            {
                throw new ArgumentException("Produto não encontrado.");
            }

            //Em seguida adicionamos ao item de pedido
            var pedido = GetPedido();

            //Verificamos se esse item já não existe para esse pedido
            var itemPedido = contexto.Set<ItemPedido>()
                            .Where(i => i.Produto.Codigo == codigo
                                    && i.Pedido.Id == pedido.Id)
                            .SingleOrDefault();

            //Se ele não encontrar o item de pedido precisamos adicionar no carrinho
            if (itemPedido == null)
            {
                itemPedido = new ItemPedido(pedido, produto, 1, produto.Preco);
                contexto.Set<ItemPedido>()
                    .Add(itemPedido);

                contexto.SaveChanges();
            }
        }

        //Retorna o pedido da Sessão
        public Pedido GetPedido()
        {
            //Pegando o pedido da sessão
            var pedidoId = GetPedidoId();
            var pedido = dbSet
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Where(p => p.Id == pedidoId)
                .SingleOrDefault();

            //Se nenhum pedido com pedidoId for encontrado criamos um novo pedido
            // e adicionamos a tabela de pedidos
            if (pedido == null)
            {
                pedido = new Pedido();
                dbSet.Add(pedido);
                contexto.SaveChanges();
                SetPedidoId(pedido.Id);     //Salvando Id do pedido na sessão
            }

            return pedido;
        }

        //Retorna o pedidoId da Sessão
        private int? GetPedidoId()
        {
            return contextAccessor.HttpContext.Session.GetInt32("pedidoId");
        }

        //Altera o pedidoId da Sessão
        private void SetPedidoId(int pedidoId)
        {
            contextAccessor.HttpContext.Session.SetInt32("pedidoId", pedidoId);
        }
    }
}
