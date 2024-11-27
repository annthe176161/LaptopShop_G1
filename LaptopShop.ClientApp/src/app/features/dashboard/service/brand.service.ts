import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Brand } from '../model/Brand.model';

@Injectable({
  providedIn: 'root',
})
export class BrandService {
  private apiUrl = 'https://localhost:7204/api/Brand'; // Địa chỉ API của bạn

  constructor(private http: HttpClient) {}

  // Phương thức để lấy tất cả các BrandDTO
  getAllBrands(): Observable<Brand[]> {
    return this.http.get<Brand[]>(this.apiUrl);
  }
  // Phương thức để thêm một brand mới
  addBrand(brand: Brand): Observable<Brand> {
    return this.http.post<Brand>(this.apiUrl, brand);
  }

  updateBrand(brand: Brand): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${brand.brandID}`, brand);
  }

  deleteBrand(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
