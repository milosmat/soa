import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { BlogService } from '../../services/blog.service';
import { Blog } from '../../models/blog';

@Component({
  standalone: true,
  selector: 'app-blog-list',
  templateUrl: './blog-list.component.html',
  styleUrls: ['./blog-list.component.css'],
  imports: [CommonModule, RouterLink, DatePipe]
})
export class BlogListComponent implements OnInit {
  blogs: Blog[] = [];
  loading = false;
  constructor(private blogSvc: BlogService) {}
  ngOnInit(): void {
    this.loading = true;
    this.blogSvc.list().subscribe({
      next: b => { this.blogs = b.sort((a,b)=> (a.createdAt < b.createdAt ? 1 : -1)); },
      error: _ => {},
      complete: () => this.loading = false,
    });
  }
  likeCount(b: Blog): number { return b.likedBy?.length ?? 0; }
}
