import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { Router, RouterLink } from '@angular/router';
import { NotaFiscalItem } from '../../../core/models/nota-fiscal';
import { Produto } from '../../../core/models/produto';
import { FeedbackService } from '../../../core/services/feedback.service';
import { NotaFiscalService } from '../../../core/services/nota-fiscal.service';
import { ProdutoService } from '../../../core/services/produto.service';
import { LoadingComponent } from '../../../shared/components/loading/loading.component';
import { ItemDialogComponent } from '../item-dialog/item-dialog.component';

@Component({
  selector: 'app-nota-fiscal-form',
  standalone: true,
  imports: [MatButtonModule, MatDialogModule, MatIconModule, MatTableModule, RouterLink, LoadingComponent],
  templateUrl: './nota-fiscal-form.component.html',
  styleUrl: './nota-fiscal-form.component.scss'
})
export class NotaFiscalFormComponent implements OnInit {
  displayedColumns = ['produto', 'quantidade', 'acao'];
  produtos: Produto[] = [];
  itens: NotaFiscalItem[] = [];
  loading = false;

  constructor(
    private readonly dialog: MatDialog,
    private readonly router: Router,
    private readonly notaFiscalService: NotaFiscalService,
    private readonly produtoService: ProdutoService,
    private readonly feedback: FeedbackService
  ) {}

  ngOnInit() {
    this.loading = true;
    this.produtoService.list(1, 100).subscribe({
      next: response => {
        this.produtos = response.data;
        this.loading = false;
      },
      error: () => {
        this.feedback.error('Erro ao carregar produtos.');
        this.loading = false;
      }
    });
  }

  openItemDialog() {
    const selected = new Set(this.itens.map(x => x.produtoId));
    const produtosDisponiveis = this.produtos.filter(x => !selected.has(x.id));

    if (produtosDisponiveis.length === 0) {
      this.feedback.error('Nao ha produtos disponiveis para incluir.');
      return;
    }

    this.dialog.open(ItemDialogComponent, {
      width: '520px',
      data: { produtos: produtosDisponiveis }
    }).afterClosed().subscribe((item?: NotaFiscalItem) => {
      if (item) {
        this.itens = [...this.itens, item];
      }
    });
  }

  remove(item: NotaFiscalItem) {
    this.itens = this.itens.filter(x => x.produtoId !== item.produtoId);
  }

  save() {
    if (this.itens.length === 0) {
      this.feedback.error('Informe ao menos um item para a nota fiscal.');
      return;
    }

    this.loading = true;
    this.notaFiscalService.create({ itens: this.itens }).subscribe({
      next: response => {
        this.loading = false;
        if (!response.success) {
          this.feedback.error(response.errors);
          return;
        }

        this.feedback.success('Nota Fiscal criada com sucesso.');
        this.router.navigate(['/notas-fiscais']);
      },
      error: error => {
        this.loading = false;
        this.feedback.error(error.error?.errors ?? 'Erro ao criar nota fiscal.');
      }
    });
  }
}
