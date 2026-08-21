import { Component, OnInit, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ActivatedRoute, Router } from '@angular/router';
import { ProdutoPayload } from '../../../core/models/produto';
import { FeedbackService } from '../../../core/services/feedback.service';
import { ProdutoService } from '../../../core/services/produto.service';
import { FormActionsComponent } from '../../../shared/components/form-actions/form-actions.component';
import { LoadingComponent } from '../../../shared/components/loading/loading.component';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';

@Component({
  selector: 'app-produto-form',
  standalone: true,
  imports: [ReactiveFormsModule, MatFormFieldModule, MatInputModule, FormActionsComponent, LoadingComponent, PageHeaderComponent],
  templateUrl: './produto-form.component.html',
  styleUrl: './produto-form.component.scss'
})
export class ProdutoFormComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly produtoService = inject(ProdutoService);
  private readonly feedback = inject(FeedbackService);

  form = new FormGroup({
    codigo: new FormControl<number | null>(null, [Validators.required, Validators.min(1)]),
    descricao: new FormControl('', [Validators.required]),
    saldo: new FormControl<number | null>(0, [Validators.required, Validators.min(0)])
  });
  id?: number;
  loading = false;
  readonly = false;

  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id')) || undefined;
    this.readonly = this.route.snapshot.routeConfig?.path?.includes('visualizar') ?? false;

    if (this.id)
      this.load(this.id);

    if (this.readonly)
      this.form.disable();
  }

  get title() {
    if (this.readonly)
      return 'Visualizar Produto';

    return this.id ? 'Alterar Produto' : 'Novo Produto';
  }

  save() {
    if (this.readonly)
      return;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.feedback.error('Verifique os campos obrigatórios.');
      return;
    }

    const payload = this.form.getRawValue() as ProdutoPayload;
    const request = this.id
      ? this.produtoService.update(this.id, payload)
      : this.produtoService.create(payload);

    this.loading = true;
    request.subscribe({
      next: response => {
        this.loading = false;
        if (!response.success) {
          this.feedback.error(response.errors);
          return;
        }

        this.feedback.success('Produto salvo com sucesso.');
        this.router.navigate(['/produtos']);
      },
      error: error => {
        this.loading = false;
        this.feedback.error(error.error?.errors ?? 'Erro ao salvar produto.');
      }
    });
  }

  private load(id: number) {
    this.loading = true;
    this.produtoService.get(id).subscribe({
      next: response => {
        this.loading = false;
        if (!response.data) {
          this.feedback.error(response.errors);
          return;
        }

        this.form.patchValue(response.data);
      },
      error: () => {
        this.loading = false;
        this.feedback.error('Produto não encontrado.');
      }
    });
  }
}