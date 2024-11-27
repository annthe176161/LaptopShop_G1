import { Component, OnInit } from '@angular/core';
import { Product } from '../model/Products.models';
import { ProductService } from '../service/product.service';
import { BrandService } from '../service/brand.service';
import { Brand } from '../model/Brand.model';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  products: Product[] = []; // Danh sách sản phẩm
  brands: Brand[] = []; // Danh sách thương hiệu

  filteredProducts: Product[] = []; // Sản phẩm sau khi áp dụng bộ lọc
  paginatedProducts: Product[] = []; // Sản phẩm hiển thị trên trang hiện tại

  selectedBrand: number | null = null; // Thương hiệu được chọn
  selectedPriceRange: { min: number; max: number } | null = null; // Khoảng giá được chọn

  // Phân trang
  productCurrentPage: number = 1; // Trang hiện tại
  productItemsPerPage: number = 5; // Số sản phẩm hiển thị trên mỗi trang

  constructor(
    private productService: ProductService,
    private brandService: BrandService
  ) {}

  ngOnInit(): void {
    this.loadProducts(); // Tải danh sách sản phẩm
    this.loadBrands(); // Tải danh sách thương hiệu
  }

  // Tải tất cả sản phẩm từ ProductService
  loadProducts(): void {
    this.productService.getAllProducts().subscribe((response) => {
      this.products = response;
      this.filteredProducts = [...this.products]; // Hiển thị tất cả sản phẩm ban đầu
      this.updatePagination(); // Cập nhật danh sách sản phẩm hiển thị theo trang
    });
  }

  // Tải danh sách thương hiệu từ BrandService
  loadBrands(): void {
    this.brandService.getAllBrands().subscribe((response) => {
      this.brands = response;
    });
  }

  // Lọc sản phẩm theo thương hiệu
  filterByBrand(brandID: number | null): void {
    this.selectedBrand = brandID;
    this.applyFilters();
  }

  // Lọc sản phẩm theo khoảng giá
  filterByPriceRange(min: number | null, max: number | null): void {
    if (min !== null && max !== null) {
      this.selectedPriceRange = { min, max };
    } else {
      this.selectedPriceRange = null; // Bỏ chọn khoảng giá
    }
    this.applyFilters();
  }

  // Áp dụng tất cả bộ lọc
  applyFilters(): void {
    if (this.selectedBrand === null && this.selectedPriceRange === null) {
      // Nếu không áp dụng bất kỳ bộ lọc nào, hiển thị tất cả sản phẩm
      this.filteredProducts = [...this.products];
    } else {
      // Lọc theo thương hiệu và khoảng giá
      this.filteredProducts = this.products.filter((product) => {
        const matchesBrand =
          this.selectedBrand === null || product.brandID === this.selectedBrand;
        const matchesPrice =
          this.selectedPriceRange === null ||
          (product.price >= this.selectedPriceRange.min &&
            product.price <= this.selectedPriceRange.max);
        return matchesBrand && matchesPrice;
      });
    }
    this.productCurrentPage = 1; // Reset về trang đầu tiên khi lọc
    this.updatePagination(); // Cập nhật phân trang sau khi áp dụng bộ lọc
  }

  // Cập nhật danh sách sản phẩm hiển thị trên trang hiện tại
  updatePagination(): void {
    const startIndex = (this.productCurrentPage - 1) * this.productItemsPerPage;
    const endIndex = startIndex + this.productItemsPerPage;
    this.paginatedProducts = this.filteredProducts.slice(startIndex, endIndex);
  }

  // Chuyển trang
  changePage(newPage: number): void {
    this.productCurrentPage = newPage;
    this.updatePagination();
  }

  // Lấy tổng số trang
  totalProductPages(): number {
    return Math.ceil(this.filteredProducts.length / this.productItemsPerPage);
  }

  // Lấy danh sách số trang
  getPages(): number[] {
    return Array.from({ length: this.totalProductPages() }, (_, i) => i + 1);
  }
}
