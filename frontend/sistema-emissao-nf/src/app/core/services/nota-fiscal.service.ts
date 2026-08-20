import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GenericDataResponse, GenericNoDataResponse, ListPagedResponse } from '../models/api-response';
import { CreateNotaFiscalPayload, NotaFiscal } from '../models/nota-fiscal';

@Injectable({ providedIn: 'root' })
export class NotaFiscalService {
  private readonly baseUrl = 'http://localhost:5198/api/notas-fiscais';

  constructor(private readonly http: HttpClient) {}

  list(page = 1, quantityData = 10) {
    const params = new HttpParams()
      .set('page', page)
      .set('quantityData', quantityData);

    return this.http.get<ListPagedResponse<NotaFiscal>>(this.baseUrl, { params });
  }

  get(id: number) {
    return this.http.get<GenericDataResponse<NotaFiscal>>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateNotaFiscalPayload) {
    return this.http.post<GenericDataResponse<NotaFiscal>>(this.baseUrl, payload);
  }

  imprimir(id: number) {
    return this.http.post<GenericNoDataResponse>(`${this.baseUrl}/${id}/imprimir`, {});
  }

  getPdf(id: number): Observable<HttpResponse<Blob>> {
    return this.http.get(`${this.baseUrl}/${id}/pdf`, {
      observe: 'response',
      responseType: 'blob'
    });
  }
}
