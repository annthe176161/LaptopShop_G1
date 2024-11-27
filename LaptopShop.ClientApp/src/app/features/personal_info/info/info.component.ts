import { Component, OnInit } from '@angular/core';
import { Order } from '../model/order.model';
import { ServiceService } from '../service/service.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-info',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './info.component.html',
  styleUrl: './info.component.css',
})
export class InfoComponent implements OnInit {
  orders: Order[] = [];
  isLoading: boolean = true;
  errorMessage: string | null = null;

  constructor(private orderService: ServiceService) {}

  ngOnInit(): void {
    const loginData = localStorage.getItem('loginData');
    if (!loginData) {
      this.errorMessage = 'Login data not found. Please log in.';
      this.isLoading = false;
      return;
    }

    const token = JSON.parse(loginData)?.token;
    if (!token) {
      this.errorMessage = 'Token not found. Please log in.';
      this.isLoading = false;
      return;
    }

    this.fetchOrders(token);
  }

  // Lấy danh sách đơn hàng
  fetchOrders(token: string): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.orderService.getOrders(token).subscribe(
      (data) => {
        this.orders = data;
        this.isLoading = false;
      },
      (error) => {
        console.error('Error fetching orders:', error);
        this.errorMessage = 'Failed to fetch orders. Please try again.';
        this.isLoading = false;
      }
    );
  }

  // Hủy đơn hàng
  cancelOrder(orderId: number): void {
    const loginData = localStorage.getItem('loginData');
    const token = loginData ? JSON.parse(loginData).token : null;

    if (!token) {
      alert('Token not found. Please log in again.');
      return;
    }

    if (confirm('Are you sure you want to cancel this order?')) {
      this.orderService.cancelOrder(orderId, token).subscribe(
        () => {
          alert('Order canceled successfully!');
          this.fetchOrders(token); // Cập nhật lại danh sách sau khi hủy
        },
        (error) => {
          console.error('Error canceling order:', error);
          alert('Failed to cancel the order.');
        }
      );
    }
  }
}
