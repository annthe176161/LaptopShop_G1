import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { UserService } from '../service/user.service';

@Component({
  selector: 'user-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent implements OnInit {
  email: string = '';
  phone: string = '';
  role: string = '';
  username: string = '';
  name: string = '';
  isEditingPhone: boolean = false; // Trạng thái chỉnh sửa số điện thoại
  newPhone: string = ''; // Số điện thoại mới

  constructor(
    private UserService: UserService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      var loginData = localStorage.getItem('loginData');
      if (loginData == null) return;
      var TokenData = JSON.parse(loginData);
      this.UserService.getProfile(TokenData['token']).subscribe(
        (data: any) => {
          //console.log(data);
          this.email = data['email'];
          this.phone = data['phoneNumber'];
          this.name = data['name'];
          this.role = data['role'];
          this.username = data['userName'];
        },
        (error: any) => {
          console.error('Error fetching data:', error);
        }
      );
    }
  }

  toggleEditPhone(): void {
    this.isEditingPhone = !this.isEditingPhone;
    if (!this.isEditingPhone) {
      this.loadUserProfile(); // Reload if user cancels editing
    }
  }

  validatePhoneNumber(phoneNumber: string): boolean {
    const vietnamPhoneRegex = /^(0)(3|5|7|8|9)\d{8}$/;
    return vietnamPhoneRegex.test(phoneNumber);
  }

  savePhoneNumber(): void {
    const phoneNumber = this.phone.trim();
    const loginData = JSON.parse(localStorage.getItem('loginData') || '{}');
    const token = loginData['token'];

    if (!phoneNumber) {
      alert('Phone number cannot be empty!');
      return;
    }

    if (!this.validatePhoneNumber(phoneNumber)) {
      alert(
        'Invalid phone number! Please enter a valid Vietnamese phone number.'
      );
      return;
    }

    this.UserService.updatePhoneNumber(token, phoneNumber).subscribe(
      (response: any) => {
        alert('Phone number updated successfully!');
        this.isEditingPhone = false;
        this.loadUserProfile();
      },
      (error: any) => {
        console.error('Error updating phone number:', error);
        alert('Failed to update phone number.');
      }
    );
  }

  private loadUserProfile(): void {
    const loginData = JSON.parse(localStorage.getItem('loginData') || '{}');
    const token = loginData['token'];

    this.UserService.getProfile(token).subscribe(
      (data: any) => {
        this.email = data['email'];
        this.phone = data['phoneNumber'];
        this.name = data['name'];
        this.role = data['role'];
        this.username = data['userName'];
      },
      (error: any) => {
        console.error('Error fetching user profile:', error);
      }
    );
  }
}
