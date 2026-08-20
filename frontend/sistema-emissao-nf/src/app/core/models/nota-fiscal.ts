export interface NotaFiscalItem {
  id: number;
  produtoId: number;
  produtoCodigo: number;
  produtoDescricao: string;
  quantidade: number;
}

export interface NotaFiscal {
  id: number;
  numeroSequencial: number;
  numeroFormatado: string;
  status: string;
  statusDescricao: string;
  statusImpressao: string;
  statusImpressaoDescricao: string;
  itens: NotaFiscalItem[];
}

export interface CreateNotaFiscalPayload {
  itens: Array<{
    produtoId: number;
    produtoCodigo: number;
    produtoDescricao: string;
    quantidade: number;
  }>;
}
