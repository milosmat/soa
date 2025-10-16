import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ToursService } from '../../services/tour.service';
import { PurchaseService } from '../../services/purchase.service';
import { Tour } from '../../models';

@Component({
  standalone: true,
  imports: [CommonModule],
  templateUrl: './market.component.html',
  styleUrls: ['./market.component.css']
})
export class MarketComponent implements OnInit {
  tours: Tour[] = [];

  constructor(private toursApi: ToursService, private purchase: PurchaseService, private router: Router) {}

  ngOnInit() {
    this.toursApi.allTours().subscribe(ts => {
      // filtriraj samo published
      this.tours = ts.filter(t => t.status === 'published');
    });
  }

  addToCart(t: Tour) {
    this.purchase.addToCart(t.id!, t.title, t.price).subscribe(() => {
      alert('Tura dodata u korpu!');
    });
  }
}
