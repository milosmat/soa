import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { AuthResponse, LoginReq, RegisterReq } from '../models';

const TOKEN_KEY = 'jwt';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private base = environment.stakeholdersApi;

  constructor(private http: HttpClient) {}

  register(body: RegisterReq) {
    return this.http.post(`${this.base}/auth/register`, body);
  }
  login(body: LoginReq) {
    return this.http.post<AuthResponse>(`${this.base}/auth/login`, body);
  }

  setToken(token: string) { localStorage.setItem(TOKEN_KEY, token); }
  get token(): string | null { return localStorage.getItem(TOKEN_KEY); }
  logout() { localStorage.removeItem(TOKEN_KEY); }
  get isLoggedIn(): boolean { return !!this.token; }
}
