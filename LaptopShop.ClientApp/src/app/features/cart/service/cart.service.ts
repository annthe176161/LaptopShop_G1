import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { CartItem } from '../model/cart-model';
import { Product } from '../../homepage/model/Products.models';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  constructor(private http: HttpClient) {}

  private apiUrl = 'https://localhost:7204/api/Order';

  private readonly CART_STORAGE_KEY = 'cart_items';
  private readonly TOTAL_PRICE_KEY = 'cart_total_price';
  private readonly TOTAL_QUANTITY_KEY = 'cart_total_quantity';

  items: CartItem[] = this.getStoredCartItems();

  private totalPriceSource = new BehaviorSubject<number>(
    this.getStoredTotalPrice()
  );
  private totalQuantitySource = new BehaviorSubject<number>(
    this.getStoredTotalQuantity()
  );

  totalPrice$ = this.totalPriceSource.asObservable();
  totalQuantity$ = this.totalQuantitySource.asObservable();

  get localStorageAvailable(): boolean {
    return typeof window !== 'undefined' && typeof localStorage !== 'undefined';
  }

  private saveCartToLocalStorage() {
    if (this.localStorageAvailable) {
      localStorage.setItem(this.CART_STORAGE_KEY, JSON.stringify(this.items));
    }
  }

  private saveTotalPriceToLocalStorage(totalPrice: number) {
    if (this.localStorageAvailable) {
      localStorage.setItem(this.TOTAL_PRICE_KEY, totalPrice.toString());
    }
  }

  private saveTotalQuantityToLocalStorage(totalQuantity: number) {
    if (this.localStorageAvailable) {
      localStorage.setItem(this.TOTAL_QUANTITY_KEY, totalQuantity.toString());
    }
  }

  private getStoredCartItems(): CartItem[] {
    if (this.localStorageAvailable) {
      try {
        const storedItems = localStorage.getItem(this.CART_STORAGE_KEY);
        return storedItems ? JSON.parse(storedItems) : [];
      } catch (error) {
        console.error('Error parsing stored cart items:', error);
        return [];
      }
    }
    return [];
  }

  private getStoredTotalPrice(): number {
    if (this.localStorageAvailable) {
      const storedPrice = localStorage.getItem(this.TOTAL_PRICE_KEY);
      return storedPrice ? parseFloat(storedPrice) : 0;
    }
    return 0;
  }

  private getStoredTotalQuantity(): number {
    if (this.localStorageAvailable) {
      const storedQuantity = localStorage.getItem(this.TOTAL_QUANTITY_KEY);
      return storedQuantity ? parseInt(storedQuantity, 10) : 0;
    }
    return 0;
  }

  addToCart(product: Product, quantity: number) {
    const index = this.items.findIndex(
      (item) => item.productID === product.productID
    );
    if (index >= 0) {
      this.items[index].quantity += quantity;
    } else {
      this.items.push({
        productID: product.productID,
        quantity: quantity,
        productName: product.name,
        productPrice: product.price,
        imageURL: product.imageURL,
      });
    }
    this.saveCartToLocalStorage();
    this.updateTotalPrice();
    this.updateTotalQuantity();
  }

  getItems(): CartItem[] {
    return this.items;
  }

  clearCart() {
    this.items = [];
    if (this.localStorageAvailable) {
      localStorage.removeItem(this.CART_STORAGE_KEY);
      localStorage.removeItem(this.TOTAL_PRICE_KEY);
      localStorage.removeItem(this.TOTAL_QUANTITY_KEY);
    }
    this.totalPriceSource.next(0);
    this.totalQuantitySource.next(0);
  }

  findQuantity(productId: number): number {
    const item = this.items.find((item) => item.productID === productId);
    return item ? item.quantity : 0;
  }

  removeItem(productID: number) {
    const index = this.items.findIndex((item) => item.productID === productID);
    if (index >= 0) {
      this.items.splice(index, 1);
      this.saveCartToLocalStorage();
      this.updateTotalPrice();
      this.updateTotalQuantity();
    }
  }

  private updateTotalPrice() {
    const total = this.items.reduce(
      (total, item) => total + item.productPrice * item.quantity,
      0
    );
    this.totalPriceSource.next(total);
    this.saveTotalPriceToLocalStorage(total);
  }

  private updateTotalQuantity() {
    const totalQuantity = this.items.reduce(
      (total, item) => total + item.quantity,
      0
    );
    this.totalQuantitySource.next(totalQuantity);
    this.saveTotalQuantityToLocalStorage(totalQuantity);
  }

  getTotalPrice(): number {
    return this.totalPriceSource.value;
  }

  getTotalQuantity(): number {
    return this.totalQuantitySource.value;
  }

  placeOrder(
    items: CartItem[],
    shippingAddress: string,
    paymentMethod: string,
    shippingMethod: string,
    notes: string,
    email: string
  ): Observable<any> {
    const url = `${this.apiUrl}/PlaceOrder?shippingAddress=${encodeURIComponent(
      shippingAddress
    )}&paymentMethod=${encodeURIComponent(
      paymentMethod
    )}&shippingMethod=${encodeURIComponent(
      shippingMethod
    )}&notes=${encodeURIComponent(notes)}&email=${email}`;
    return this.http.post(url, items);
  }
}
