import {
  Component,
  inject,
  signal,
} from "@angular/core";
import { RouterModule } from "@angular/router";
import { SplitterModule } from 'primeng/splitter';
import { ToolbarModule } from 'primeng/toolbar';
import { SidebarModule } from 'primeng/sidebar';
import { DialogModule } from 'primeng/dialog';
import { PanelMenuComponent } from "./shared/ui/panel-menu/panel-menu.component";
import { Product } from "./products/data-access/product.model";
import { CartService } from "./services/cart.service";
import { ButtonModule } from "primeng/button";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: true,
  imports: [RouterModule, SplitterModule, ToolbarModule, PanelMenuComponent, SidebarModule, ButtonModule, DialogModule],
})
export class AppComponent {
  public readonly cartService = inject(CartService);
  title = "ALTEN SHOP";
  public isCartVisible = false;
  public cart = signal<{ product: Product, quantity: number }[]>([]);
  sidebarVisible: boolean = false;

  public addToCart(product: Product): void {
    const currentCart = this.cart();
    const existing = currentCart.find(item => item.product.id === product.id);

    if (existing) {
      existing.quantity++;
    } else {
      currentCart.push({ product, quantity: 1 });
    }

    this.cart.set([...currentCart]);
  }

  public formatPrice(price: number): string {
    return this.cartService.formatPrice(price);
  }

  public getCartTotal(): number {
    return this.cartService.items().reduce((total, item) => total + item.product.price * item.quantity, 0);
  }

  public removeFromCart(product: Product): void {
    this.cartService.remove(product);
  }

  public increaseQuantity(product: Product): void {
    this.cartService.increase(product);
  }
  
  public decreaseQuantity(product: Product): void {
    this.cartService.decrease(product);
  }
  
}
