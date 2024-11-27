import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  GuardResult,
  MaybeAsync,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService implements CanActivate {
  private baseUrl = 'https://localhost:7204/api/Authentication';

  constructor(
    private http: HttpClient,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  async canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Promise<boolean> {
    if (isPlatformBrowser(this.platformId)) {
      const requiredRole = route.data['role'].map((item: string) =>
        item.toLowerCase()
      );
      var loginData = localStorage.getItem('loginData');
      if (!loginData) {
        return false;
      }
      var TokenData = JSON.parse(loginData);
      //console.log(TokenData['token']);
      try {
        var data: any = await firstValueFrom(this.getRole(TokenData['token']));
        var containRole = data.some((item: string) =>
          requiredRole.includes(item.toLowerCase())
        );
        return containRole;
      } catch (e) {
        console.log(e);
      }
      /*
            this.getRole(TokenData['token']).subscribe(
                (data: any) => {
                    var containRole = data.some((item: string) => requiredRole.includes(item.toLowerCase()));
                    console.log(data);
                    console.log(requiredRole);
                    console.log(containRole);
                    return containRole;
                },
                (error: any) => {
                    console.error('Error fetching data:', error);
                });
                */
      return false;
    }
    return false;
  }

  loginUser(email: string, password: string) {
    const body = { email: email, password: password };
    return this.http.post(`${this.baseUrl}/login-user`, body);
  }

  registerUser(
    name: string,
    email: string,
    password: string,
    confirm_password: string
  ) {
    const body = {
      name: name,
      email: email,
      password: password,
      confirmPassword: confirm_password,
    };
    return this.http.post(`${this.baseUrl}/register-user`, body);
  }

  forgotPassword(email: string) {
    const body = { email: email };
    return this.http.post(`${this.baseUrl}/forgot-password`, body);
  }

  resetPassword(
    token: string,
    email: string,
    newPassword: string,
    confirmPassword: string
  ) {
    const body = {
      email: email,
      token: token,
      newPassword: newPassword,
      confirmPassword: confirmPassword,
    };
    return this.http.post(`${this.baseUrl}/reset-password`, body);
  }

  getRole(token: string) {
    const params = new HttpParams().set('token', token);
    return this.http.get(`${this.baseUrl}/get-role`, { params: params });
  }
}
