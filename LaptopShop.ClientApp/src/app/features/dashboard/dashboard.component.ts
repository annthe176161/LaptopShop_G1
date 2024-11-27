import { Component, OnInit } from '@angular/core';
import { Brand } from './model/Brand.model';

import { BrandService } from './service/brand.service';
import { ProductService } from './service/product.service';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Product } from './model/Products.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit {
  // Danh sách thương hiệu và sản phẩm
  brands: Brand[] = [];
  products: Product[] = [];
  paginatedBrands: Brand[] = [];
  paginatedProducts: Product[] = [];
  // Trạng thái phân trang
  brandCurrentPage: number = 1;
  brandItemsPerPage: number = 5; // Số thương hiệu mỗi trang
  productCurrentPage: number = 1;
  productItemsPerPage: number = 5; // Số sản phẩm mỗi trang
  //
  selectedProduct: Product = this.resetProduct();

  // Trạng thái hiển thị bảng
  selectedTab: string = 'products'; // Tab mặc định là Products
  showBrandsTable: boolean = false;
  showProductsTable: boolean = false;

  // Điều khiển modal
  isBrandModalVisible: boolean = false;
  isProductModalVisible: boolean = false;

  // Trạng thái thông báo
  isSuccess: boolean = false;
  isDeleteSuccess: boolean = false;
  isUpdateSuccess: boolean = false;

  // Model cho form Thương hiệu
  newBrand: Brand = { brandID: 0, brandName: '', isDeleted: false };

  constructor(
    private brandService: BrandService,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    this.loadBrands(); // Tải danh sách thương hiệu
    this.loadProducts(); // Tải danh sách sản phẩm
  }

  // Tải danh sách thương hiệu từ server
  loadBrands(): void {
    this.brandService.getAllBrands().subscribe(
      (response) => {
        this.brands = response;
        this.updatePagination('brands'); // Cập nhật phân trang
      }, // Cập nhật phân trang
      (error) => console.error('Error loading brands:', error)
    );
  }

  // Tải danh sách sản phẩm từ server
  loadProducts(): void {
    this.productService.getAllProducts().subscribe(
      (response) => {
        this.products = response;
        this.updatePagination('products'); // Cập nhật phân trang
      },
      (error) => console.error('Error loading products:', error)
    );
  }

  // Hiển thị bảng thương hiệu
  showBrands(): void {
    this.showBrandsTable = true;
    this.showProductsTable = false;
    this.selectedTab = 'brands';
  }

  // Hiển thị bảng sản phẩm
  showProducts(): void {
    this.showProductsTable = true;
    this.showBrandsTable = false;
    this.selectedTab = 'products';
  }

  // Thêm mới hoặc chỉnh sửa thương hiệu
  saveBrand(): void {
    if (this.newBrand.brandID === 0) {
      this.brandService.addBrand(this.newBrand).subscribe(() => {
        this.isSuccess = true;
        this.loadBrands(); // Reload danh sách thương hiệu
        this.isBrandModalVisible = false;
        setTimeout(() => (this.isSuccess = false), 3000);
      });
    } else {
      this.brandService.updateBrand(this.newBrand).subscribe(() => {
        this.isUpdateSuccess = true;
        this.loadBrands(); // Reload danh sách thương hiệu
        this.isBrandModalVisible = false;
        setTimeout(() => (this.isUpdateSuccess = false), 3000);
      });
    }
  }

  // Sửa thương hiệu
  editBrand(brand: Brand): void {
    this.newBrand = { ...brand }; // Gắn dữ liệu thương hiệu vào form
    this.isBrandModalVisible = true;
  }

  // Xóa thương hiệu
  deleteBrand(id: number): void {
    if (confirm('Are you sure you want to delete this brand?')) {
      this.brandService.deleteBrand(id).subscribe(() => {
        this.isDeleteSuccess = true;
        this.loadBrands(); // Reload danh sách thương hiệu
        setTimeout(() => (this.isDeleteSuccess = false), 3000);
      });
    }
  }

  // Thêm mới hoặc chỉnh sửa sản phẩm
  saveProduct(): void {
    if (this.selectedProduct.productID === 0) {
      this.productService.addProduct(this.selectedProduct).subscribe(
        () => {
          this.loadProducts();
          this.isProductModalVisible = false;
        },
        (error) => console.error('Error adding product:', error)
      );
    } else {
      this.productService.updateProduct(this.selectedProduct).subscribe(
        () => {
          this.loadProducts();
          this.isProductModalVisible = false;
        },
        (error) => console.error('Error updating product:', error)
      );
    }
  }

  // Sửa sản phẩm
  editProduct(product: Product): void {
    this.selectedProduct = { ...product };
    this.isProductModalVisible = true;
  }

  // Xóa sản phẩm
  deleteProduct(id: number): void {
    if (confirm('Are you sure you want to delete this product?')) {
      this.productService.deleteProduct(id).subscribe(
        () => this.loadProducts(),
        (error) => console.error('Error deleting product:', error)
      );
    }
  }

  resetProduct(): Product {
    return {
      productID: 0,
      name: '',
      brandID: 0,
      price: 0,
      description: '',
      stockQuantity: 0,
      imageURL: '',
      createdDate: new Date(),
      isDeleted: false,
      brandName: '',
    };
  }
  // Phân trang
  updatePagination(type: 'brands' | 'products') {
    if (type === 'brands') {
      const start = (this.brandCurrentPage - 1) * this.brandItemsPerPage;
      this.paginatedBrands = this.brands.slice(
        start,
        start + this.brandItemsPerPage
      );
    } else {
      const start = (this.productCurrentPage - 1) * this.productItemsPerPage;
      this.paginatedProducts = this.products.slice(
        start,
        start + this.productItemsPerPage
      );
    }
  }
  // Tính tổng số trang
  totalBrandPages(): number {
    return Math.ceil(this.brands.length / this.brandItemsPerPage);
  }

  totalProductPages(): number {
    return Math.ceil(this.products.length / this.productItemsPerPage);
  }

  // Lấy danh sách số trang
  getPages(type: 'brands' | 'products'): number[] {
    const totalPages =
      type === 'brands' ? this.totalBrandPages() : this.totalProductPages();
    return Array.from({ length: totalPages }, (_, i) => i + 1);
  }
  // Chuyển trang
  changePage(newPage: number, type: 'brands' | 'products') {
    if (type === 'brands') {
      this.brandCurrentPage = newPage;
      this.updatePagination('brands');
    } else {
      this.productCurrentPage = newPage;
      this.updatePagination('products');
    }
  }
}
