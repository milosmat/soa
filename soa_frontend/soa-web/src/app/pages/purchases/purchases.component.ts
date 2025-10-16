import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { PurchaseService } from '../../services/purchase.service';
import { ToursService } from '../../services/tour.service';

@Component({
  standalone: true,
  imports: [CommonModule],
  templateUrl: './purchases.component.html',
  styleUrls: ['./purchases.component.css']
})
export class PurchasesComponent implements OnInit {
  purchases: any[] = [];

  constructor(
    private purchase: PurchaseService,
    private tours: ToursService,
    private router: Router
  ) {}

  ngOnInit() {
    this.purchase.myPurchases().subscribe(tokens => {
      this.purchases = [];
      tokens.forEach(token => {
        this.tours.getTour(token.tourId).subscribe(t => {
          this.purchases.push({ ...t, purchasedAt: token.purchasedAt });
        });
      });
    });
  }

  open(t: any) {
    this.router.navigate(['/purchases', t.id]);
  }
}
