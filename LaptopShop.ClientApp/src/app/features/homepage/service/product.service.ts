import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Product } from '../model/Products.models';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = 'https://localhost:7204/api/Product';
  constructor(private http: HttpClient) {}

  getAllProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl);
  }
  getProductById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  getProductsByBrand(brandID: number): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/brand/${brandID}`);
  }

  getProductsByPriceRange(
    minPrice: number,
    maxPrice: number
  ): Observable<Product[]> {
    return this.http.get<Product[]>(
      `${this.apiUrl}/price?minPrice=${minPrice}&maxPrice=${maxPrice}`
    );
  }
}
