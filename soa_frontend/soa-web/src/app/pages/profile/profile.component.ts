import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ProfileService } from '../../services/profile.service';

@Component({
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  f!: FormGroup;
  constructor(private fb: FormBuilder, private prof: ProfileService) {
    this.f = this.fb.group({ firstName:'', lastName:'', motto:'', bio:'' });
  }
  ngOnInit(){
    this.prof.me().subscribe(p => this.f.patchValue(p as any));
  }
  save(){ this.prof.update(this.f.value).subscribe(); }
}
