// cart.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PurchaseService } from '../../services/purchase.service';
import {finalize, switchMap} from 'rxjs/operators';

@Component({
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  cart: any;
  isProcessing = false;

  constructor(private purchase: PurchaseService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.purchase.getCart().subscribe(c => this.cart = c);
  }

  checkout() {
    this.isProcessing = true;

    this.purchase.checkout().pipe(
      // Bez obzira da li je 200 ili 202, čekamo da korpa postane prazna
      switchMap(_ => this.purchase.waitUntilCartEmpty(30000, 800)),
      finalize(() => this.isProcessing = false)
    ).subscribe({
      next: _ => {
        alert('Kupovina uspešna!');
        this.load();
      },
      error: (err) => {
        alert('Greška prilikom kupovine: ' + (err?.message || 'Nepoznata greška'));
      }
    });
  }

}
