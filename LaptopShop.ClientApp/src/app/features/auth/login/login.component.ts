import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../service/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent implements OnInit {
  email: string = '';
  password: string = '';
  rememberMe: boolean = false;
  showPassword: boolean = false;
  errorStr: string = '';

  constructor(
    private AuthService: AuthService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      const rememberedEmail = localStorage.getItem('rememberedEmail');
      const rememberedPassword = localStorage.getItem('rememberedPassword');

      if (rememberedEmail && rememberedPassword) {
        this.email = rememberedEmail;
        this.password = rememberedPassword;
        this.rememberMe = true;
      }
    }
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  onSubmit(form: any) {
    var form_data = form.value;
    this.AuthService.loginUser(form_data.email, form_data.password).subscribe(
      (data: any) => {
        localStorage.setItem('loginData', JSON.stringify(data));
        if (this.rememberMe) {
          localStorage.setItem('rememberedEmail', form_data.email);
          localStorage.setItem('rememberedPassword', form_data.password);
        } else {
          localStorage.removeItem('rememberedEmail');
          localStorage.removeItem('rememberedPassword');
        }
        window.location.href = '/';
      },
      (error: any) => {
        console.error('Error fetching data:', error);
        this.errorStr = error.error;
      }
    );
  }
}
