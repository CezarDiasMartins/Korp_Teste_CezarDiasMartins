export interface GenericDataResponse<T> {
  success: boolean;
  data: T | null;
  errors: string[];
}

export interface GenericNoDataResponse {
  success: boolean;
  errors: string[];
}

export interface ListPagedResponse<T> {
  success: boolean;
  data: T[];
  page: number;
  quantityData: number;
  totalData: number;
  totalPage: number;
  previous: boolean;
  next: boolean;
  errors: string[];
}
