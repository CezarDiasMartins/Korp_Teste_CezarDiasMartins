export interface Produto {
  id: number;
  codigo: number;
  descricao: string;
  saldo: number;
}

export interface ProdutoPayload {
  codigo: number;
  descricao: string;
  saldo: number;
}
