import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Order } from '../model/order.model';

@Injectable({
  providedIn: 'root',
})
export class ServiceService {
  private baseUrl = 'https://localhost:7204/api/OrderCustomer'; // Đường dẫn API

  constructor(private http: HttpClient) {}

  // Lấy JWT token từ local storage
  private getAuthHeaders(token: string): HttpHeaders {
    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
  }

  // Lấy danh sách đơn hàng
  getOrders(token: string): Observable<Order[]> {
    const params = new HttpParams().set('token', token);
    return this.http.get<Order[]>(`${this.baseUrl}/user-orders`, { params });
  }

  // Hủy đơn hàng
  cancelOrder(orderId: number, token: string): Observable<void> {
    const params = new HttpParams().set('token', token);
    return this.http.post<void>(
      `${this.baseUrl}/cancel/${orderId}`,
      {},
      { params }
    );
  }
}
