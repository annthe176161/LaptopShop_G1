import { Component, OnInit } from '@angular/core';
import { CartService } from './service/cart.service';
import { CartItem } from './model/cart-model';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ProductService } from '../homepage/service/product.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css',
})
export class CartComponent implements OnInit {
  items: CartItem[] = []; // Danh sách sản phẩm trong giỏ hàng
  totalPrice: number = 0; // Tổng tiền
  shippingAddress: string = '';
  shippingMethod: string = '';
  paymentMethod: string = '';
  notes: string = '';

  constructor(
    private cartService: CartService,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    // Lấy danh sách sản phẩm từ CartService
    this.items = this.cartService.getItems();

    // Tính tổng tiền
    this.totalPrice = this.cartService.getTotalPrice();
  }

  removeItem(productID: number) {
    this.cartService.removeItem(productID); // Xóa sản phẩm trong CartService
    this.items = this.cartService.getItems(); // Cập nhật danh sách giỏ hàng
    this.totalPrice = this.cartService.getTotalPrice(); // Cập nhật tổng tiền
  }

  // Xóa toàn bộ giỏ hàng
  clearCart() {
    this.cartService.clearCart();
    this.items = []; // Đặt lại danh sách giỏ hàng
    this.totalPrice = 0; // Đặt lại tổng tiền
  }

  updateQuantity(productId: number, num: number) {
    if (num === -1 && this.cartService.findQuantity(productId) === 1) {
      this.removeItem(productId);
    } else {
      this.productService.getProductById(productId).subscribe({
        next: (product) => {
          console.log('Product fetched:', product);
          this.cartService.addToCart(product, num); // Gửi sản phẩm và số lượng vào giỏ hàng
          console.log('Current cart items:', this.cartService.getItems());
          this.totalPrice = this.cartService.getTotalPrice();
        },
      });
    }
  }

  submitForm() {
    if (this.notes === null || this.notes === '') {
      this.notes = 'No further notes!';
    }
    if (
      this.paymentMethod === '' ||
      this.shippingMethod === '' ||
      this.shippingAddress === ''
    ) {
      Swal.fire('ERROR!', 'All required fields must be filled!', 'error');
      return;
    }

    // // Kiểm tra định dạng địa chỉ giao hàng (bao gồm địa chỉ và số điện thoại)
    // const addressWithPhoneRegex = /.*[a-zA-Z0-9,]+.*\d{10,}.*/;
    // if (!addressWithPhoneRegex.test(this.shippingAddress)) {
    //   Swal.fire(
    //     'ERROR!',
    //     'Shipping address must include both a detailed address and a valid phone number (at least 10 digits).',
    //     'error'
    //   );
    //   return;
    // }

    // Gửi request đặt hàng
    this.cartService
      .placeOrder(
        this.items,
        this.shippingAddress,
        this.paymentMethod,
        this.shippingMethod,
        this.notes,
        localStorage.getItem('rememberedEmail') || ''
      )
      .subscribe({
        next: (response) => {
          console.log('Order placed successfully:', response);
          Swal.fire('Success!', 'Your order has been placed!', 'success');
          this.clearCart(); // Xóa giỏ hàng sau khi đặt hàng thành công
        },
        error: (err) => {
          console.error('Error placing order:', err);
          Swal.fire('ERROR!', 'Failed to place order!', 'error');
        },
      });
  }
}
