import { isPlatformBrowser } from '@angular/common';
import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../features/auth/service/auth.service';
import { CartItem } from '../../../features/cart/model/cart-model';
import { CartService } from '../../../features/cart/service/cart.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent implements OnInit {
  loggined: boolean = false;
  isAdmin = false;

  totalPrice: number = 0;
  totalQuantity: number = 0;
  items: CartItem[] = [];

  constructor(
    private AuthService: AuthService,
    private cartService: CartService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {
      setTimeout(() => {
        const loginData = localStorage.getItem('loginData');
        if (loginData) {
          this.loggined = true;
          var TokenData = JSON.parse(loginData);
          this.AuthService.getRole(TokenData['token']).subscribe(
            (data: any) => {
              if (data.includes('Admin')) {
                this.isAdmin = true;
              }
            },
            (error: any) => {
              console.error('Error fetching data:', error);
            }
          );
        } else {
          this.loggined = false;
        }
      }, 0);
    }

    if (this.cartService.localStorageAvailable) {
      // Lấy danh sách sản phẩm từ CartService
      this.items = this.cartService.getItems();
      // Tính tổng tiền
      // Lắng nghe sự thay đổi tổng giá
      this.cartService.totalPrice$.subscribe((price) => {
        this.totalPrice = price;
      });

      // Lắng nghe sự thay đổi tổng số lượng
      this.cartService.totalQuantity$.subscribe((quantity) => {
        this.totalQuantity = quantity;
      });
    } else {
      this.items = [];
      this.totalPrice = 0;
      this.totalQuantity = 0;
    }
  }
}
