class Carrinho {

    clickIncremento(button) {
        let data = this.getData(button);
        data.Quantidade++;
        this.postQuantidade(data);
    }

    clickDecremento(button) {
        let data = this.getData(button);
        data.Quantidade--;
        this.postQuantidade(data);
    }

    updateQuantidade(input) {
        let data = this.getData(input);
        this.postQuantidade(data);
    }

    getData(elemento) {
        var linhaDoItem = $(elemento).parents('[item-id]');  //retornando o pai de "btn" que possui o atributo "item-id"
        var itemId = $(linhaDoItem).attr('item-id');    //função jQuery
        var novaQuantidade = $(linhaDoItem).find('input').val();  //retornando o valor do input que está dentro da div linhaDoItem

        return {
            Id: itemId,
            Quantidade: novaQuantidade
        };
    }

    postQuantidade(data) {

        let token = $('[name=__RequestVerificationToken]').val();

        let headers = {};
        headers['RequestVerificationToken'] = token;


        $.ajax({
            url: '/pedido/updatequantidade',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            headers: headers
        }).done(function (response) {       //done indica que a chamada ajax foi realizada com sucesso, recebe uma função que é a resposta do servidor, enviada pelo PedidoController

            let itemPedido = response.itemPedido;                       //Pegando do objeto response(UpdateQuantidadeResponse) o itemPedido
            let linhaDoItem = $('[item-id=' + itemPedido.id + ']');     //a partir do id da response localizamos a div com item-id = itemPedido.id  --  [procurando por esse atributo]
            linhaDoItem.find('input').val(itemPedido.quantidade);       //localizamos o input e alteramos seu valor para itemPedido.quantidade
            linhaDoItem.find('[subtotal]').html((itemPedido.subtotal).duasCasas());

            let carrinhoViewModel = response.carrinhoViewModel;
            $('[numero-itens]').html('Total: ' + carrinhoViewModel.itens.length + ' itens');    //atualizando o número total de itens
            $('[total]').html((carrinhoViewModel.total).duasCasas());

            if (itemPedido.quantidade == 0) {
                linhaDoItem.remove();
            }
        });
    }

}

var carrinho = new Carrinho();

//Método que mantém duas casas decimais e substitui o '.' por ','
Number.prototype.duasCasas = function () {
    return this.toFixed(2).replace('.', ',');
}

