import { Component, OnInit } from '@angular/core';
import { Order } from '../../models/models.module';
import { OrderService } from '../../services/order.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './order-list.component.html',
  styleUrl: './order-list.component.css',
})
export class OrderListComponent implements OnInit {
  orders: Order[] = [];
  totalItems: number = 0;

  // Các biến cho phân trang và lọc
  pageNumber: number = 1;
  pageSize: number = 10;
  pageSizeOptions: number[] = [5, 10, 20, 50];

  statusFilter: string | null = null;
  searchTerm: string | null = null;

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.orderService
      .getOrders(
        this.statusFilter,
        this.searchTerm,
        this.pageNumber,
        this.pageSize
      )
      .subscribe({
        next: (data) => {
          this.orders = data;
          // Giả sử API trả về tổng số bản ghi, bạn có thể lấy thông tin này nếu cần
          // this.totalItems = data.totalItems;
        },
        error: (err) => console.error('Error loading orders:', err),
      });
  }

  // Các phương thức để xử lý phân trang, tìm kiếm, lọc
  onPageChange(newPage: number): void {
    this.pageNumber = newPage;
    this.loadOrders();
  }

  onPageSizeChange(): void {
    this.pageNumber = 1;
    this.loadOrders();
  }

  onFilterChange(): void {
    this.pageNumber = 1;
    this.loadOrders();
  }

  onSearch(): void {
    this.pageNumber = 1;
    this.loadOrders();
  }

  calculateIndex(index: number): number {
    return (this.pageNumber - 1) * this.pageSize + index + 1;
  }
}
