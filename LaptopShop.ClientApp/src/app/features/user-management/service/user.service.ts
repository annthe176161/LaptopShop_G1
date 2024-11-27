import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../model/user.model';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl = 'https://localhost:7204/api/User'; // Base URL của API

  constructor(private http: HttpClient) {}

  // Lấy danh sách người dùng
  getUsers(
    page: number,
    size: number,
    search: string = '',
    inactivePeriod: number = -1
  ): Observable<User[]> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', size.toString())
      .set('search', search);

    if (inactivePeriod !== -1) {
      params = params.set('inactivePeriod', inactivePeriod.toString());
    }

    return this.http.get<User[]>(`${this.apiUrl}/customers`, { params });
  }

  // Gửi email mời người dùng quay lại hệ thống
  inviteUser(userId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/invite-user/${userId}`, {});
  }

  // Xuất file Excel người dùng
  exportUsers(inactivePeriod: number = -1): Observable<Blob> {
    const params = new HttpParams().set(
      'inactivePeriod',
      inactivePeriod.toString()
    );
    return this.http.get(`${this.apiUrl}/export-users`, {
      params,
      responseType: 'blob',
    });
  }

  // Thay đổi quyền truy cập người dùng
  toggleAccess(userId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/toggle-access/${userId}`, {});
  }
}
