export interface Product {
  productID: number;
  name: string;
  brandID: number;
  brandName: string;
  price: number;
  description: string;
  stockQuantity: number;
  imageURL: string;
  createdDate: Date;
  isDeleted: boolean;
}
