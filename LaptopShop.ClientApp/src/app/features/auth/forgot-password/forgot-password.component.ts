import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../service/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css',
})
export class ForgotPasswordComponent {
  errorStr: string = '';
  notiStr: string = '';

  constructor(private AuthService: AuthService, private router: Router) {}

  onSubmit(form: any) {
    var form_data = form.value;
    this.AuthService.forgotPassword(form_data.email).subscribe(
      (data: any) => {
        console.log(data);
      },
      (error: any) => {
        console.error('Error fetching data:', error);
        if (error.status == 200) {
          this.errorStr = '';
          this.notiStr = error.error.text;
        } else {
          this.notiStr = '';
          try {
            this.errorStr = error.error.errors.Email.join(', ');
          } catch (e) {
            this.errorStr = 'unknow error';
          }
        }
      }
    );
  }
}
