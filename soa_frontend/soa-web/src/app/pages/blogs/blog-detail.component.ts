import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { BlogService } from '../../services/blog.service';
import { Blog, Comment } from '../../models/blog';
import { FormsModule } from '@angular/forms';
import { MarkdownPipe } from '../../pipes/markdown.pipe';
import { SecureImgDirective } from '../../directives/secure-img.directive';
import { AuthService } from '../../services/auth.service';

@Component({
  standalone: true,
  selector: 'app-blog-detail',
  templateUrl: './blog-detail.component.html',
  styleUrls: ['./blog-detail.component.css'],
  imports: [CommonModule, FormsModule, DatePipe, MarkdownPipe, SecureImgDirective]
})
export class BlogDetailComponent implements OnInit {
  blog?: Blog;
  loading = false;
  newComment = '';
  editCommentId: string | null = null;
  editText = '';

  constructor(private route: ActivatedRoute, private blogSvc: BlogService, public auth: AuthService) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.load(id);
  }

  load(id: string) {
    this.loading = true;
    this.blogSvc.get(id).subscribe({ next: b => {
      // normalize for template
      b.images = b.images ?? [];
      b.comments = b.comments ?? [];
      b.likedBy = b.likedBy ?? [] as any;
      this.blog = b;
    }, error: _ => {}, complete: () => this.loading = false });
  }

  like() {
    if (!this.blog) return;
    const id = this.blog.id;
    this.blogSvc.like(id).subscribe({ next: _ => this.load(id), error: err => console.error('Like failed', err) });
  }
  unlike() {
    if (!this.blog) return;
    const id = this.blog.id;
    this.blogSvc.unlike(id).subscribe({ next: _ => this.load(id), error: err => console.error('Unlike failed', err) });
  }

  addComment() {
    if (!this.blog || !this.newComment.trim()) return;
    this.blogSvc.addComment(this.blog.id, { text: this.newComment.trim() }).subscribe(c => {
      if (!this.blog) return;
      this.blog = { ...this.blog, comments: [...(this.blog.comments||[]), c] };
      this.newComment = '';
    });
  }

  startEdit(c: Comment) { this.editCommentId = c.id; this.editText = c.text; }
  cancelEdit() { this.editCommentId = null; this.editText = ''; }
  saveEdit() {
    if (!this.blog || !this.editCommentId) return;
    this.blogSvc.updateComment(this.blog.id, this.editCommentId, this.editText).subscribe(updated => {
      if (!this.blog) return;
      const comments = this.blog.comments.map(c => c.id === updated.id ? updated : c);
      this.blog = { ...this.blog, comments };
      this.cancelEdit();
    });
  }

}
