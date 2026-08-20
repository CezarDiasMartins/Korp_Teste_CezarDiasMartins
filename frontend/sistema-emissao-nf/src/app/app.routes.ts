import { Routes } from '@angular/router';
import { NotaFiscalDetailComponent } from './features/notas-fiscais/nota-fiscal-detail/nota-fiscal-detail.component';
import { NotaFiscalFormComponent } from './features/notas-fiscais/nota-fiscal-form/nota-fiscal-form.component';
import { NotaFiscalListComponent } from './features/notas-fiscais/nota-fiscal-list/nota-fiscal-list.component';
import { ProdutoFormComponent } from './features/produtos/produto-form/produto-form.component';
import { ProdutoListComponent } from './features/produtos/produto-list/produto-list.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'notas-fiscais' },
  { path: 'notas-fiscais', component: NotaFiscalListComponent },
  { path: 'incluir', component: NotaFiscalFormComponent },
  { path: 'visualizar/:id', component: NotaFiscalDetailComponent },
  { path: 'produtos', component: ProdutoListComponent },
  { path: 'produtos/incluir', component: ProdutoFormComponent },
  { path: 'produtos/alterar/:id', component: ProdutoFormComponent },
  { path: 'produtos/visualizar/:id', component: ProdutoFormComponent },
  { path: '**', redirectTo: 'notas-fiscais' }
];
