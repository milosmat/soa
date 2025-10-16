import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Profile } from '../models';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private base = environment.stakeholdersApi;
  constructor(private http: HttpClient) {}
  me() { return this.http.get<Profile>(`${this.base}/me/profile`); }
  update(body: Partial<Profile>) { return this.http.put(`${this.base}/me/profile`, body); }
}
