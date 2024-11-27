import { Component } from '@angular/core';
import { AuthService } from '../service/auth.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  showPassword: boolean = false;
  errorStr: string = '';

  constructor(private AuthService: AuthService) {}

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  onSubmit(form: any) {
    var form_data = form.value;
    if (!form_data.agree) {
      this.errorStr = 'You need to agree with the  Privacy Policy';
      return;
    }
    this.AuthService.registerUser(
      form_data.name,
      form_data.email,
      form_data.password,
      form_data.password_confirm
    ).subscribe(
      (data: any) => {
        console.log(data);
      },
      (error: any) => {
        console.log(error);
        if (error.status == 201) {
          alert(error.error.text);
          return;
        }
        if (typeof error.error == 'string') {
          this.errorStr = error.error;
        } else {
          this.errorStr = error.error.title;
        }
      }
    );
  }
}
