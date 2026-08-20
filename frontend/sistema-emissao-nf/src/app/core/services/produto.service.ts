import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GenericDataResponse, ListPagedResponse } from '../models/api-response';
import { Produto, ProdutoPayload } from '../models/produto';

@Injectable({ providedIn: 'root' })
export class ProdutoService {
  private readonly baseUrl = 'http://localhost:5153/api/produtos';

  constructor(private readonly http: HttpClient) {}

  list(page = 1, quantityData = 10) {
    const params = new HttpParams()
      .set('page', page)
      .set('quantityData', quantityData);

    return this.http.get<ListPagedResponse<Produto>>(this.baseUrl, { params });
  }

  get(id: number) {
    return this.http.get<GenericDataResponse<Produto>>(`${this.baseUrl}/${id}`);
  }

  create(payload: ProdutoPayload) {
    return this.http.post<GenericDataResponse<Produto>>(this.baseUrl, payload);
  }

  update(id: number, payload: ProdutoPayload) {
    return this.http.put<GenericDataResponse<Produto>>(`${this.baseUrl}/${id}`, payload);
  }
}
