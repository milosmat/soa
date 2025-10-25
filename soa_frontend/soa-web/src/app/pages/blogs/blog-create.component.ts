import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { BlogService } from '../../services/blog.service';
import { SecureImgDirective } from '../../directives/secure-img.directive';

@Component({
  standalone: true,
  selector: 'app-blog-create',
  templateUrl: './blog-create.component.html',
  styleUrls: ['./blog-create.component.css'],
  imports: [CommonModule, FormsModule, SecureImgDirective]
})
export class BlogCreateComponent {
  title = '';
  description = '';
  images: string[] = [];
  saving = false;

  constructor(private blogSvc: BlogService, private router: Router) {}
  removeImage(i: number){ this.images.splice(i,1); }

  onFilesSelected(ev: Event){
    const input = ev.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;
    const files = Array.from(input.files);
    // Upload each file sequentially
    files.reduce((prev, file) => prev.then(() => new Promise<void>(resolve => {
      this.blogSvc.upload(file).subscribe({
        next: res => { this.images.push(res.url); resolve(); },
        error: _ => resolve()
      });
    })), Promise.resolve());
    // Clear the input
    input.value = '';
  }

  create(){
    if (!this.title.trim() || !this.description.trim()) return;
    this.saving = true;
    this.blogSvc.create({ title: this.title.trim(), description: this.description, images: this.images }).subscribe({
      next: b => this.router.navigate(['/blogs', b.id]),
      error: _ => this.saving = false,
      complete: () => this.saving = false
    });
  }
}
