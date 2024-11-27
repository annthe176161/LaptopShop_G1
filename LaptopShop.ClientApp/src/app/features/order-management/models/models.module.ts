export interface OrderDetail {
  productID: number;
  productName: string;
  productPrice: number;
  quantity: number;
  subtotal: number;
}

export interface Order {
  orderID: number;
  userID: string;
  customerName: string;
  orderDate: string;
  totalAmount: number;
  orderStatus: string;
  paymentMethod: string;
  shippingAddress: string;
  shippingMethod: string;
  notes: string;
  phoneNumber: string;
  orderDetails: OrderDetail[];
}
