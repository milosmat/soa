import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  f!: FormGroup;
  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
    this.f = this.fb.group({ usernameOrEmail: '', password: '' });
  }
  submit() {
    this.auth.login(this.f.value as any).subscribe(r => {
      this.auth.setToken(r.token);
      this.router.navigateByUrl('/tours');
    });
  }
}
