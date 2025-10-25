import { Directive, ElementRef, Input, OnChanges, OnDestroy, Renderer2, SimpleChanges } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Subscription } from 'rxjs';
import { environment } from '../../environments/environment';

@Directive({
  selector: 'img[secureImgSrc]',
  standalone: true,
})
export class SecureImgDirective implements OnChanges, OnDestroy {
  @Input() secureImgSrc: string | null | undefined;

  private sub?: Subscription;
  private objectUrl?: string;

  constructor(private el: ElementRef<HTMLImageElement>, private http: HttpClient, private renderer: Renderer2) {}

  ngOnChanges(changes: SimpleChanges): void {
    if ('secureImgSrc' in changes) {
      this.load();
    }
  }

  private toAbsolute(url: string): string {
    if (!url) return '';
    if (/^https?:\/\//i.test(url)) return url;
    const path = url.startsWith('/') ? url : `/${url}`;
    return `${environment.blogApi}${path}`;
  }

  private load() {
    // cleanup previous
    if (this.sub) { this.sub.unsubscribe(); this.sub = undefined; }
    if (this.objectUrl) { URL.revokeObjectURL(this.objectUrl); this.objectUrl = undefined; }

    const src = this.secureImgSrc ? this.toAbsolute(this.secureImgSrc) : '';
    if (!src) {
      this.renderer.setAttribute(this.el.nativeElement, 'src', '');
      return;
    }
    // Fetch as blob so interceptor adds Authorization
    this.sub = this.http.get(src, { responseType: 'blob' }).subscribe({
      next: blob => {
        this.objectUrl = URL.createObjectURL(blob);
        this.renderer.setAttribute(this.el.nativeElement, 'src', this.objectUrl);
      },
      error: _ => {
        // fallback to direct URL (may 401 if not logged in)
        this.renderer.setAttribute(this.el.nativeElement, 'src', src);
      }
    });
  }

  ngOnDestroy(): void {
    if (this.sub) this.sub.unsubscribe();
    if (this.objectUrl) URL.revokeObjectURL(this.objectUrl);
  }
}
