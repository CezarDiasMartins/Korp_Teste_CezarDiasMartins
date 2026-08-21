import { Component, OnInit, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { RouterLink } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { NotaFiscal } from '../../../core/models/nota-fiscal';
import { FeedbackService } from '../../../core/services/feedback.service';
import { NotaFiscalService } from '../../../core/services/nota-fiscal.service';
import { LoadingComponent } from '../../../shared/components/loading/loading.component';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';

@Component({
  selector: 'app-nota-fiscal-detail',
  standalone: true,
  imports: [MatButtonModule, MatIconModule, MatTableModule, RouterLink, LoadingComponent, PageHeaderComponent],
  templateUrl: './nota-fiscal-detail.component.html',
  styleUrl: './nota-fiscal-detail.component.scss'
})
export class NotaFiscalDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly notaFiscalService = inject(NotaFiscalService);
  private readonly feedback = inject(FeedbackService);

  displayedColumns = ['produto', 'quantidade'];
  nota?: NotaFiscal;
  loading = false;

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loading = true;
    this.notaFiscalService.get(id).subscribe({
      next: response => {
        this.loading = false;
        if (!response.data) {
          this.feedback.error(response.errors);
          return;
        }

        this.nota = response.data;
      },
      error: () => {
        this.loading = false;
        this.feedback.error('Nota fiscal não encontrada.');
      }
    });
  }
}