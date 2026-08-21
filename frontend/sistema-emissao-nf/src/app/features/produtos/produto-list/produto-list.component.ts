import { Component, OnInit, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { RouterLink } from '@angular/router';
import { Produto } from '../../../core/models/produto';
import { FeedbackService } from '../../../core/services/feedback.service';
import { ProdutoService } from '../../../core/services/produto.service';
import { LoadingComponent } from '../../../shared/components/loading/loading.component';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';

@Component({
  selector: 'app-produto-list',
  standalone: true,
  imports: [MatButtonModule, MatIconModule, MatPaginatorModule, MatTableModule, RouterLink, LoadingComponent, PageHeaderComponent],
  templateUrl: './produto-list.component.html',
  styleUrl: './produto-list.component.scss'
})
export class ProdutoListComponent implements OnInit {
  private readonly produtoService = inject(ProdutoService);
  private readonly feedback = inject(FeedbackService);

  displayedColumns = ['codigo', 'descricao', 'saldo', 'acoes'];
  produtos: Produto[] = [];
  loading = false;
  page = 1;
  pageSize = 10;
  total = 0;

  ngOnInit() {
    this.load();
  }

  load() {
    this.loading = true;
    this.produtoService.list(this.page, this.pageSize).subscribe({
      next: response => {
        this.produtos = response.data;
        this.total = response.totalData;
        this.loading = false;
      },
      error: () => {
        this.feedback.error('Erro ao carregar produtos.');
        this.loading = false;
      }
    });
  }

  pageChanged(event: PageEvent) {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.load();
  }
}