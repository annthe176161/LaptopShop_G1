import { Component, OnInit } from '@angular/core';
import { Order } from '../../models/models.module';
import { ActivatedRoute } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './order-detail.component.html',
  styleUrl: './order-detail.component.css',
})
export class OrderDetailComponent implements OnInit {
  order: Order | null = null;
  newStatus: string = '';
  rejectionReason: string = '';
  errorMessage: string | null = null;

  // Define available statuses
  statuses: string[] = ['Confirmed', 'Rejected', 'Shipped', 'Completed'];

  constructor(
    private route: ActivatedRoute,
    private orderService: OrderService
  ) {}

  ngOnInit(): void {
    const orderId = Number(this.route.snapshot.paramMap.get('orderId'));
    this.loadOrder(orderId);
  }

  loadOrder(orderId: number): void {
    this.orderService.getOrderById(orderId).subscribe(
      (data) => {
        this.order = data;
        this.errorMessage = null; // Clear error message on success
      },
      (error: HttpErrorResponse) => {
        if (error.error && error.error.message) {
          this.errorMessage = error.error.message;
        } else {
          this.errorMessage = 'An error occurred while fetching order details.';
        }
        this.order = null; // Ensure order is not displayed when there's an error
      }
    );
  }

  updateOrderStatus(): void {
    if (this.order && this.newStatus) {
      const requestBody: any = {
        status: this.newStatus,
      };
      if (this.newStatus === 'Rejected') {
        requestBody.reason = this.rejectionReason;
      }

      this.orderService
        .updateOrderStatus(this.order.orderID, requestBody)
        .subscribe({
          next: (response) => {
            if (response.success) {
              alert(response.message || 'Order status updated successfully!');
              this.order!.orderStatus = this.newStatus;
              this.newStatus = '';
              this.rejectionReason = '';
            } else {
              alert(response.message || 'Unable to update order status.');
            }
          },
          error: (err) => {
            console.error('Error updating status:', err);
            alert('Unable to update order status. Please try again later.');
          },
        });
    } else {
      alert('Please select a valid status.');
    }
  }

  // Method to get Bootstrap badge classes based on status
  getStatusBadge(status: string): string {
    switch (status) {
      case 'Confirmed':
        return 'badge bg-success';
      case 'Rejected':
        return 'badge bg-danger';
      case 'Shipped':
        return 'badge bg-primary';
      case 'Completed':
        return 'badge bg-secondary';
      default:
        return 'badge bg-light text-dark';
    }
  }
}
