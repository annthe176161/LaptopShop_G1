import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../service/auth.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css',
})
export class ResetPasswordComponent implements OnInit {
  errorStr: string = '';
  notiStr: string = '';
  email: string = '';
  token: string = '';

  constructor(
    private AuthService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.token = params['token'];
      this.email = params['email'];
    });
  }

  onSubmit(form: any) {
    var form_data = form.value;
    this.AuthService.resetPassword(
      this.token,
      this.email,
      form_data.newPassword,
      form_data.confirmPassword
    ).subscribe(
      (data: any) => {
        console.log(data);
      },
      (error: any) => {
        console.error('Error fetching data:', error);
        if (error.status != 200) {
          try {
            if (Array.isArray(error.error.errors)) {
              this.errorStr = error.error.errors.join(', ');
              return;
            }
            if (error.error.errors.NewPassword) {
              this.errorStr += error.error.errors.NewPassword.join(', ');
            }
            if (error.error.errors.ConfirmPassword) {
              if (this.errorStr.length > 0) {
                this.errorStr += ', ';
              }
              this.errorStr += error.error.errors.ConfirmPassword.join(', ');
            }
          } catch (e) {
            this.errorStr = 'something went wrong';
          }
        } else {
          this.errorStr = '';
          this.notiStr = error.error.text;
        }
      }
    );
  }
}
