import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private baseUrl = 'https://localhost:7204/api/User';

  constructor(private http: HttpClient) {}

  // Lấy danh sách người dùng với phân trang
  getCustomers(
    page: number,
    pageSize: number,
    search: string = ''
  ): Observable<any[]> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (search) {
      params = params.set('search', search);
    }

    return this.http.get<any[]>(`${this.baseUrl}/customers`, { params }).pipe(
      catchError((error) => {
        console.error('Error fetching customers:', error);
        return throwError(error);
      })
    );
  }

  // Thay đổi quyền truy cập của người dùng
  toggleUserAccess(userId: string): Observable<any> {
    return this.http
      .post<any>(`${this.baseUrl}/toggle-access/${userId}`, {})
      .pipe(
        catchError((error) => {
          console.error('Error toggling user access:', error);
          return throwError(error);
        })
      );
  }

  // Lọc người dùng không hoạt động
  getInactiveUsers(daysInactive: number): Observable<any> {
    const params = new HttpParams().set(
      'daysInactive',
      daysInactive.toString()
    );

    return this.http
      .get<any>(`${this.baseUrl}/inactive-users`, { params })
      .pipe(
        catchError((error) => {
          console.error('Error fetching inactive users:', error);
          return throwError(error);
        })
      );
  }

  // Gửi lời mời cho người dùng
  inviteUser(userId: string): Observable<any> {
    return this.http
      .post<any>(`${this.baseUrl}/invite-user/${userId}`, {})
      .pipe(
        catchError((error) => {
          console.error('Error inviting user:', error);
          return throwError(error);
        })
      );
  }

  //them
  getProfile(token: string): Observable<any> {
    const params = new HttpParams().set('token', token);
    return this.http.get(`${this.baseUrl}/profile`, { params }).pipe(
      catchError((error) => {
        console.error('Error fetching user profile:', error);
        return throwError(() => new Error('Error fetching user profile'));
      })
    );
  }

  updatePhoneNumber(token: string, phoneNumber: string): Observable<any> {
    const payload = { phoneNumber }; // Payload for phone update
    const params = new HttpParams().set('token', token);
    return this.http
      .put(`${this.baseUrl}/update-phone`, payload, { params })
      .pipe(
        catchError((error) => {
          console.error('Error updating phone number:', error);
          return throwError(() => new Error('Error updating phone number'));
        })
      );
  }
}
