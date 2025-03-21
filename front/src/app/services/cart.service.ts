import { Injectable } from '@angular/core';
import { signal, computed } from '@angular/core';
import { Product } from 'app/products/data-access/product.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cartItems = signal<{ product: Product, quantity: number }[]>([]);

  public readonly items = computed(() => this.cartItems());

  public add(product: Product): void {
    const current = this.cartItems();
    const existing = current.find(item => item.product.id === product.id);

    if (existing) {
      existing.quantity++;
    } else {
      current.push({ product, quantity: 1 });
    }

    this.cartItems.set([...current]);
  }

  public remove(product: Product): void {
    const updated = this.cartItems().filter(item => item.product.id !== product.id);
    this.cartItems.set(updated);
  }

  public clear(): void {
    this.cartItems.set([]);
  }
  public formatPrice(price: number): string {
    return new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR' }).format(price);
  }

  public increase(product: Product): void {
    const updated = this.cartItems().map(item =>
      item.product.id === product.id
        ? { ...item, quantity: item.quantity + 1 }
        : item
    );
    this.cartItems.set(updated);
  }
  
  public decrease(product: Product): void {
    const updated = this.cartItems()
      .map(item =>
        item.product.id === product.id && item.quantity > 1
          ? { ...item, quantity: item.quantity - 1 }
          : item
      );
    this.cartItems.set(updated);
  }
  
}