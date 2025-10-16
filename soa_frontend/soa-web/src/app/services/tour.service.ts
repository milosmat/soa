import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { CreateTourReq, Tour, TourPointReq } from '../models';

@Injectable({ providedIn: 'root' })
export class ToursService {
  private base = environment.toursApi;
  constructor(private http: HttpClient) {}
  createTour(body: CreateTourReq) { return this.http.post<{id:string}>(`${this.base}/tours`, body); }
  myTours() { return this.http.get<Tour[]>(`${this.base}/tours/mine`); }
  addPoint(id: string, body: TourPointReq) { return this.http.post(`${this.base}/tours/${id}/points`, body); }
  allTours() {
    return this.http.get<Tour[]>(`${this.base}/tours`);
  }
  getTour(id: string) {
    return this.http.get<Tour>(`${this.base}/tours/${id}`);
  }

  updateTour(id: string, body: any) {
    return this.http.put(`${this.base}/tours/${id}`, body);
  }
}
