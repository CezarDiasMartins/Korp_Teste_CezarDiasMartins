import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { NotaFiscalItem } from '../../../core/models/nota-fiscal';
import { ItemDialogData } from './item-dialog.type';

@Component({
  selector: 'app-item-dialog',
  standalone: true,
  imports: [ReactiveFormsModule, MatButtonModule, MatDialogModule, MatFormFieldModule, MatIconModule, MatInputModule, MatSelectModule],
  templateUrl: './item-dialog.component.html',
  styleUrl: './item-dialog.component.scss'
})
export class ItemDialogComponent {
  private readonly dialogRef = inject(MatDialogRef<ItemDialogComponent>);
  readonly data = inject<ItemDialogData>(MAT_DIALOG_DATA);

  form = new FormGroup({
    produtoId: new FormControl<number | null>(null, [Validators.required]),
    quantidade: new FormControl<number | null>(1, [Validators.required, Validators.min(1)])
  });

  confirm() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const produto = this.data.produtos.find(x => x.id === this.form.value.produtoId)!;
    const item: NotaFiscalItem = {
      id: 0,
      produtoId: produto.id,
      produtoCodigo: produto.codigo,
      produtoDescricao: produto.descricao,
      quantidade: Number(this.form.value.quantidade)
    };

    this.dialogRef.close(item);
  }
}