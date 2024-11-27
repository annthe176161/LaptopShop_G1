import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../service/user.service';
import { FormsModule } from '@angular/forms';
import * as XLSX from 'xlsx';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css',
})
export class UserListComponent implements OnInit {
  customers: any[] = []; // Danh sách khách hàng
  page: number = 1; // Trang hiện tại
  pageSize: number = 10; // Số lượng khách hàng mỗi trang
  totalItems: number = 0; // Tổng số người dùng
  searchTerm: string = ''; // Chuỗi tìm kiếm
  inactivePeriod: number = -1; // Bộ lọc thời gian không hoạt động
  isLoading: boolean = false; // Trạng thái đang tải dữ liệu
  searchSubject: Subject<string> = new Subject<string>();

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.loadCustomers();

    // Lắng nghe sự thay đổi từ ô tìm kiếm
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.searchTerm = term;
        this.page = 1;
        this.loadCustomers();
      });
  }

  // Tải danh sách khách hàng từ API
  loadCustomers(): void {
    this.isLoading = true;

    this.userService
      .getCustomers(this.page, this.pageSize, this.searchTerm)
      .subscribe(
        (data: any) => {
          this.customers = Array.isArray(data) ? data : data.items || [];
          this.totalItems = this.customers.length;
          this.isLoading = false;
        },
        (error: any) => {
          console.error('Error fetching customers:', error);
          this.customers = [];
          this.isLoading = false;
        }
      );
  }

  // Xử lý tìm kiếm
  onSearch(term: string): void {
    this.searchSubject.next(term);
  }

  // Chuyển trang
  onPageChange(newPage: number): void {
    if (newPage > 0) {
      this.page = newPage;
      this.loadCustomers();
    }
  }

  // Thay đổi quyền truy cập với xác nhận
  toggleAccess(userId: string, isActive: boolean): void {
    const action = isActive ? 'disable' : 'enable';
    const confirmed = window.confirm(
      `Are you sure you want to ${action} access for this user?`
    );

    if (confirmed) {
      this.userService.toggleUserAccess(userId).subscribe(
        () => {
          alert(
            `User access ${isActive ? 'disabled' : 'enabled'} successfully!`
          );
          window.location.reload(); // Tải lại toàn bộ trang
        },
        (error: any) => {
          console.error('Error toggling user access:', error);
          alert('Failed to change user access.');
        }
      );
    }
  }

  // Gửi lời mời cho người dùng với xác nhận
  inviteUser(userId: string): void {
    const confirmed = window.confirm(
      'Are you sure you want to send an invitation to this user?'
    );

    if (confirmed) {
      this.userService.inviteUser(userId).subscribe(
        () => {
          alert('Invitation sent successfully!');
          window.location.reload(); // Tải lại toàn bộ trang
        },
        (error) => {
          console.error('Error inviting user:', error);
          alert('Failed to send invitation.');
        }
      );
    }
  }

  // Lọc người dùng không hoạt động và tự động tải lại dữ liệu
  onFilterInactive(): void {
    if (this.inactivePeriod !== -1) {
      this.userService.getInactiveUsers(this.inactivePeriod).subscribe(
        (data: any[]) => {
          this.customers = data;
          this.totalItems = data.length;
        },
        (error: any) => {
          console.error('Error fetching inactive users:', error);
        }
      );
    } else {
      this.loadCustomers(); // Hiển thị tất cả người dùng
    }
  }

  // Xuất danh sách người dùng ra file Excel
  exportUserList(): void {
    const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(
      this.customers.map((user) => ({
        'User ID': user.id,
        Name: user.name,
        Email: user.email,
        'Is Active': user.isActive ? 'Yes' : 'No',
        'Last Login Date': user.lastLoginDate,
      }))
    );

    const workbook: XLSX.WorkBook = {
      Sheets: { Users: worksheet },
      SheetNames: ['Users'],
    };

    XLSX.writeFile(workbook, `UserList-${new Date().toISOString()}.xlsx`);
  }
}
