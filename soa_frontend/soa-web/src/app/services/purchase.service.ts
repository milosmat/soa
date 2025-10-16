// purchase.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { Observable, interval } from 'rxjs';
import { switchMap, take, filter, timeout, catchError } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class PurchaseService {
  private base = environment.purchaseApi;

  constructor(private http: HttpClient, private auth: AuthService) {}

  private get headers() {
    return {
      headers: new HttpHeaders({
        Authorization: `Bearer ${this.auth.token}`
      })
    };
  }

  getCart() {
    return this.http.get<any>(`${this.base}/cart`, this.headers);
  }

  addToCart(tourId: string, tourName: string, price: number) {
    return this.http.post<any>(`${this.base}/cart/add`, { tourId, tourName, price }, this.headers);
  }

  // ⬇️ bitno: tražimo ceo HttpResponse da bismo videli status 202
  checkout(): Observable<HttpResponse<any>> {
    return this.http.post<any>(`${this.base}/cart/checkout`, {}, { ...this.headers, observe: 'response' });
  }

  myPurchases() {
    return this.http.get<{ tourId: string, purchasedAt: string }[]>(
      `${this.base}/purchases/mine`,
      this.headers
    );
  }

  // ⬇️ helper: poll-uje purchases do prvog uspeha ili timeout-a
  waitUntilCartEmpty(maxMs = 30000, intervalMs = 800) {
    return interval(intervalMs).pipe(
      switchMap(() => this.getCart()),
      filter(c => c && Array.isArray(c.items) && c.items.length === 0),
      take(1),
      timeout({ each: maxMs })
    );
  }

}
