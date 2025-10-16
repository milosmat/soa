import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  f!: FormGroup;
  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
    this.f = this.fb.group({ username: '', email: '', password: '', role: 'GUIDE' });
  }
  submit() {
    this.auth.register(this.f.value as any).subscribe(() => this.router.navigateByUrl('/login'));
  }
}
