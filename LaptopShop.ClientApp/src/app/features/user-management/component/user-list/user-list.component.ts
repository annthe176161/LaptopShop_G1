import { Component, OnInit } from '@angular/core';
import { User } from '../../model/user.model';
import { UserService } from '../../service/user.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css',
})
export class UserListComponent implements OnInit {
  users: User[] = [];
  pageNumber = 1; // Trang hiện tại
  pageSize = 10; // Số bản ghi trên mỗi trang
  searchTerm = '';
  searchSubject = new Subject<string>();
  inactivePeriod = -1; // -1: Tất cả, 7: 1 tuần, 30: 1 tháng, 365: 1 năm

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.loadUsers();

    // Lắng nghe sự thay đổi của input search
    this.searchSubject.pipe(debounceTime(300)).subscribe(() => {
      this.pageNumber = 1;
      this.loadUsers();
    });
  }

  // Load danh sách người dùng
  loadUsers(): void {
    // Kiểm tra nếu searchTerm rỗng hoặc chỉ chứa khoảng trắng
    const searchValue = this.searchTerm.trim();

    this.userService
      .getUsers(
        this.pageNumber,
        this.pageSize,
        searchValue,
        this.inactivePeriod
      )
      .subscribe({
        next: (data) => {
          if (data.length === 0 && this.pageNumber > 1) {
            // Nếu không còn bản ghi nào trên trang hiện tại, quay về trang trước
            this.pageNumber--;
            this.loadUsers();
          } else {
            this.users = data;
          }
        },
        error: (err) => console.error('Error loading users:', err),
      });
  }

  // Tìm kiếm
  onSearchChange(): void {
    this.searchSubject.next(this.searchTerm);
  }

  // Thay đổi số trang
  onPageChange(newPage: number): void {
    if (
      newPage < 1 ||
      (newPage > this.pageNumber && this.users.length < this.pageSize)
    ) {
      // Không cho phép chuyển trang nếu vượt qua trang cuối
      return;
    }
    this.pageNumber = newPage;
    this.loadUsers();
  }

  // Thay đổi số bản ghi trên mỗi trang
  onPageSizeChange(): void {
    this.pageNumber = 1;
    this.loadUsers();
  }

  // Lọc theo trạng thái
  onFilterChange(): void {
    this.pageNumber = 1;
    this.loadUsers();
  }

  inviteUser(userId: string): void {
    this.userService.inviteUser(userId).subscribe({
      next: () => {
        alert('Invitation sent successfully!');
      },
      error: (err) => console.error('Error sending invitation:', err),
    });
  }

  exportUsers(): void {
    this.userService.exportUsers(this.inactivePeriod).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = `UserList-${new Date().toISOString()}.xlsx`;
        anchor.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => console.error('Error exporting users:', err),
    });
  }

  toggleAccess(userId: string): void {
    Swal.fire({
      title: 'Are you sure?',
      text: "Do you want to change this user's access?",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes',
      cancelButtonText: 'No',
    }).then((result) => {
      if (result.isConfirmed) {
        this.userService.toggleAccess(userId).subscribe({
          next: () => {
            Swal.fire(
              'Success!',
              'User access updated successfully.',
              'success'
            ).then(() => {
              location.reload(); // Reload toàn bộ trang sau khi xác nhận
            });
          },
          error: (err) => {
            console.error('Error Response:', err); // Log lỗi chi tiết
            Swal.fire('Error', 'Failed to update user access.', 'error');
          },
        });
      } else {
        Swal.fire('Cancelled', 'No changes were made.', 'info');
      }
    });
  }
}
