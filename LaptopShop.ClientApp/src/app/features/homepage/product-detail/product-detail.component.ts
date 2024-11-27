import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ProductService } from '../service/product.service';
import { Product } from '../model/Products.models';
import { CartService } from '../../cart/service/cart.service';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.css',
})
export class ProductDetailComponent implements OnInit {
  product!: Product;
  totalPrice: number = 0;
  quantity: number = 1;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    const productID = Number(this.route.snapshot.paramMap.get('id'));
    if (productID) {
      this.loadProduct(productID);
    }
  }
  loadProduct(id: number): void {
    this.productService.getProductById(id).subscribe({
      next: (response) => {
        this.product = response;
      },
      error: (err) => {
        console.error('Error fetching product:', err);
      },
    });
  }

  addToCart(formValues: { productId: number; quantity: number }) {
    const { productId, quantity } = formValues; // Lấy productId và quantity từ form values
    this.productService.getProductById(productId).subscribe({
      next: (product) => {
        this.cartService.addToCart(product, quantity); // Gửi sản phẩm và số lượng vào giỏ hàng
        this.totalPrice = this.cartService.getTotalPrice();
        alert('Product added to cart successfully!');
      },
    });
  }
}
