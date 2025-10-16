import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class PositionService {
  private base = environment.toursApi;

  constructor(private http: HttpClient, private auth: AuthService) {}

  setPosition(lat: number, lng: number) {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${this.auth.token}`
    });
    return this.http.post(`${this.base}/positions`, { lat, lng }, { headers });
  }

  getMyPosition() {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${this.auth.token}`
    });
    return this.http.get<{ lat: number, lng: number }>(`${this.base}/positions/me`, { headers });
  }
}
