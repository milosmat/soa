import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { AddCommentReq, Blog, Comment, CreateBlogReq } from '../models/blog';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class BlogService {
  private base = environment.blogApi + '/api/blogs';
  constructor(private http: HttpClient) {}

  list(): Observable<Blog[]> { return this.http.get<Blog[]>(this.base); }
  get(id: string): Observable<Blog> { return this.http.get<Blog>(`${this.base}/${id}`); }
  create(body: CreateBlogReq): Observable<Blog> { return this.http.post<Blog>(this.base, body); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }

  addComment(id: string, body: AddCommentReq): Observable<Comment> {
    return this.http.post<Comment>(`${this.base}/${id}/comments`, body);
  }
  updateComment(id: string, commentId: string, text: string): Observable<Comment> {
    return this.http.put<Comment>(`${this.base}/${id}/comments/${commentId}`, text, {
      headers: { 'Content-Type': 'text/plain; charset=utf-8' }
    });
  }

  like(id: string): Observable<number> { return this.http.post<number>(`${this.base}/${id}/like`, {}); }
  unlike(id: string): Observable<number> { return this.http.post<number>(`${this.base}/${id}/unlike`, {}); }

  upload(file: File): Observable<{ url: string }> {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<{ url: string }>(`${this.base}/upload`, form);
  }
}
