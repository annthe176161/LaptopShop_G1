import {
  HttpClient,
  HttpErrorResponse,
  HttpParams,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { Order } from '../models/models.module';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  private apiUrl = 'https://localhost:7204/api/OrderAdmin';

  constructor(private http: HttpClient) {}

  // Lấy danh sách đơn hàng
  getOrders(
    status: string | null,
    searchTerm: string | null,
    pageNumber: number,
    pageSize: number
  ): Observable<Order[]> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (status) {
      params = params.set('status', status);
    }

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    const url = `${this.apiUrl}/GetOrders`;
    return this.http.get<Order[]>(url, { params });
  }

  // Lấy chi tiết đơn hàng
  getOrderById(orderId: number): Observable<Order> {
    const url = `${this.apiUrl}/GetOrderById/${orderId}`;
    return this.http.get<Order>(url).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(error);
      })
    );
  }

  // Cập nhật trạng thái đơn hàng
  updateOrderStatus(
    orderId: number,
    requestBody: { status: string; reason?: string }
  ): Observable<any> {
    const url = `${this.apiUrl}/UpdateOrderStatus/${orderId}`;
    return this.http.put(url, requestBody, {
      headers: { 'Content-Type': 'application/json' },
    });
  }
}
