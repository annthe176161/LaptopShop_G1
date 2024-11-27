import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { HomeComponent } from './features/homepage/home/home.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { InfoComponent } from './features/personal_info/info/info.component';
import { LogoutComponent } from './features/auth/logout/logout.component';
import { OrderListComponent } from './features/order-management/components/order-list/order-list.component';
import { OrderDetailComponent } from './features/order-management/components/order-detail/order-detail.component';
import { AuthService } from './features/auth/service/auth.service';
import { ProductDetailComponent } from './features/homepage/product-detail/product-detail.component';
import { CartComponent } from './features/cart/cart.component';
import { UserListComponent } from './features/user-management/component/user-list/user-list.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { ProfileComponent } from './features/user/profile/profile.component';
import { ForgotPasswordComponent } from './features/auth/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './features/auth/reset-password/reset-password.component';
export const routes: Routes = [
  // Đường dẫn mặc định cho web app
  { path: '', component: HomeComponent },
  // Đường dẫn đến trang đăng nhập
  { path: 'login', component: LoginComponent },
  // Đường dẫn đến trang đăng ký
  { path: 'register', component: RegisterComponent },
  // Đường dẫn đến trang quản lý người dùng
  {
    path: 'user-list',
    component: UserListComponent,
    // canActivate: [AuthService],
    // data: { role: ['ADMIN'] },
  },

  // Đường dẫn đến trang thông tin người dùng
  {
    path: 'user-profile',
    component: InfoComponent,
    canActivate: [AuthService],
    data: { role: ['customer', 'admin'] },
  },
  // Đường dẫn đến trang đăng xuất
  { path: 'logout', component: LogoutComponent },
  // Đường dẫn đến trang quên mật khẩu
  { path: 'forgot-password', component: ForgotPasswordComponent },
  // Đường dẫn đến trang đặt lại mật khẩu
  { path: 'reset-password', component: ResetPasswordComponent },
  // Đường dẫn đến trang quản lý đơn hàng
  {
    path: 'orders',
    component: OrderListComponent,
    canActivate: [AuthService],
    data: { role: ['admin'] },
  },
  // Đường dẫn đến trang chi tiết đơn hàng
  {
    path: 'orders/:orderId',
    component: OrderDetailComponent,
    canActivate: [AuthService],
    data: { role: ['admin'] },
  },
  // Đường dẫn đến trang chi tiết sản phẩm với id sản phẩm
  { path: 'product-detail/:id', component: ProductDetailComponent }, // Chi tiết sản phẩm
  // Đường dẫn đến trang quản lý người dùng
  {
    path: 'user-management',
    component: UserListComponent,
    canActivate: [AuthService],
    data: { role: ['admin'] },
  },
  // Route 404 cho các đường dẫn không tồn tại
  // { path: '**', redirectTo: '' },
  {
    path: 'cart',
    component: CartComponent,
    canActivate: [AuthService],
    data: { role: ['customer'] },
  },
  //Đường dẫn đến trang dashboard
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AuthService],
    data: { role: ['admin'] },
  },
  // Đường dẫn đến trang thông tin cá nhân
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: [AuthService],
    data: { role: ['customer', 'admin'] },
  },
];
