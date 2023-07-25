import { Component, OnInit } from '@angular/core';
import { ProdutoService } from '../services/produtoService';
import { Produto } from '../models/Produto';

@Component({
  selector: 'app-lista',
  templateUrl: './lista.component.html'
})
export class ListaComponent implements OnInit {

  constructor(private produtoService: ProdutoService) { }

  public produtos: Produto[];
  public imageURL: string;
  errorMessage: string;

  ngOnInit() {
    this.produtoService.obterTodos()
      .subscribe(
        result => this.produtos = result[`data`],
        error => this.errorMessage = error
    );   
  }
}
