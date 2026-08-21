import { Component, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-form-actions',
  standalone: true,
  imports: [MatButtonModule, MatIconModule, RouterLink],
  templateUrl: './form-actions.component.html',
  styleUrl: './form-actions.component.scss'
})
export class FormActionsComponent {
  readonly backLink = input.required<string>();
  readonly backLabel = input('Voltar');
  readonly backIcon = input('arrow_back');
  readonly primaryLabel = input('Salvar');
  readonly primaryIcon = input('save');
  readonly showPrimary = input(true);
  readonly primaryClick = output<void>();
}