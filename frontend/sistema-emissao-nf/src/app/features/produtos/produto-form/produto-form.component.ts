import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ProdutoPayload } from '../../../core/models/produto';
import { FeedbackService } from '../../../core/services/feedback.service';
import { ProdutoService } from '../../../core/services/produto.service';
import { LoadingComponent } from '../../../shared/components/loading/loading.component';

@Component({
  selector: 'app-produto-form',
  standalone: true,
  imports: [ReactiveFormsModule, MatButtonModule, MatFormFieldModule, MatIconModule, MatInputModule, RouterLink, LoadingComponent],
  templateUrl: './produto-form.component.html',
  styleUrl: './produto-form.component.scss'
})
export class ProdutoFormComponent implements OnInit {
  form = new FormGroup({
    codigo: new FormControl<number | null>(null, [Validators.required, Validators.min(1)]),
    descricao: new FormControl('', [Validators.required]),
    saldo: new FormControl<number | null>(0, [Validators.required, Validators.min(0)])
  });
  id?: number;
  loading = false;
  readonly = false;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly produtoService: ProdutoService,
    private readonly feedback: FeedbackService
  ) {}

  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id')) || undefined;
    this.readonly = this.route.snapshot.routeConfig?.path?.includes('visualizar') ?? false;

    if (this.id) {
      this.load(this.id);
    }

    if (this.readonly) {
      this.form.disable();
    }
  }

  get title() {
    if (this.readonly) {
      return 'Visualizar Produto';
    }

    return this.id ? 'Alterar Produto' : 'Novo Produto';
  }

  save() {
    if (this.readonly) {
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.feedback.error('Verifique os campos obrigatorios.');
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
        this.feedback.error('Produto nao encontrado.');
      }
    });
  }
}
