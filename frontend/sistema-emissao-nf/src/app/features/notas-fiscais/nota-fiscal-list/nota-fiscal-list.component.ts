import { HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { RouterLink } from '@angular/router';
import { catchError, EMPTY, filter, finalize, switchMap, take, takeWhile, timer } from 'rxjs';
import { NotaFiscal } from '../../../core/models/nota-fiscal';
import { FeedbackService } from '../../../core/services/feedback.service';
import { NotaFiscalService } from '../../../core/services/nota-fiscal.service';
import { LoadingComponent } from '../../../shared/components/loading/loading.component';

@Component({
  selector: 'app-nota-fiscal-list',
  standalone: true,
  imports: [MatButtonModule, MatIconModule, MatPaginatorModule, MatProgressBarModule, MatTableModule, RouterLink, LoadingComponent],
  templateUrl: './nota-fiscal-list.component.html',
  styleUrl: './nota-fiscal-list.component.scss'
})
export class NotaFiscalListComponent implements OnInit {
  displayedColumns = ['numero', 'status', 'impressao', 'acoes'];
  notas: NotaFiscal[] = [];
  loading = false;
  processingId?: number;
  page = 1;
  pageSize = 10;
  total = 0;

  constructor(
    private readonly notaFiscalService: NotaFiscalService,
    private readonly feedback: FeedbackService
  ) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.loading = true;
    this.notaFiscalService.list(this.page, this.pageSize).subscribe({
      next: response => {
        this.notas = response.data;
        this.total = response.totalData;
        this.loading = false;
      },
      error: () => {
        this.feedback.error('Erro ao carregar notas fiscais.');
        this.loading = false;
      }
    });
  }

  imprimir(nota: NotaFiscal) {
    this.processingId = nota.id;
    this.notaFiscalService.imprimir(nota.id).subscribe({
      next: () => {
        this.feedback.success('Nota fechada e estoque atualizado.');
        this.load();
        this.pollPdf(nota.id);
      },
      error: (error: HttpErrorResponse) => {
        this.processingId = undefined;
        this.feedback.error(error.error?.errors ?? 'Erro ao processar a nota.');
      }
    });
  }

  abrirPdf(nota: NotaFiscal) {
    this.pollPdf(nota.id);
  }

  pageChanged(event: PageEvent) {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.load();
  }

  private pollPdf(id: number) {
    this.processingId = id;

    timer(0, 3000).pipe(
      switchMap(() => this.notaFiscalService.getPdf(id)),
      takeWhile((response: HttpResponse<Blob>) => response.status !== 200, true),
      filter((response: HttpResponse<Blob>) => response.status === 200),
      take(1),
      catchError((error: HttpErrorResponse) => {
        this.feedback.error(error.error?.errors ?? 'Erro ao consultar PDF.');
        return EMPTY;
      }),
      finalize(() => {
        this.processingId = undefined;
        this.load();
      })
    ).subscribe(response => {
      if (!response.body) {
        return;
      }

      const url = URL.createObjectURL(response.body);
      window.open(url, '_blank');
    });
  }
}
