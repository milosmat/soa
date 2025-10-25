import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { marked } from 'marked';
import DOMPurify from 'dompurify';

// Full markdown rendering using 'marked' with sanitization via DOMPurify
@Pipe({ name: 'markdown', standalone: true })
export class MarkdownPipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) {}
  transform(value?: string): SafeHtml {
    if (!value) return '' as unknown as SafeHtml;
    // marked.parse types allow Promise<string>, but we use it synchronously here
    const dirty = marked.parse(value, { breaks: true }) as string;
    const clean = DOMPurify.sanitize(dirty);
    return this.sanitizer.bypassSecurityTrustHtml(clean);
  }
}
