import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { ToursService } from '../../services/tour.service';
import { Tour } from '../../models';

@Component({
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './tours.component.html',
  styleUrls: ['./tours.component.css']
})
export class ToursComponent implements OnInit {
  tours: Tour[] = [];
  f!: FormGroup;

  constructor(private fb: FormBuilder, private api: ToursService, private router: Router){
    this.f = this.fb.group({ title:'', description:'', difficulty:'easy', tags:'' });
  }

  ngOnInit(){ this.load(); }
  load(){ this.api.myTours().subscribe(t => this.tours = t); }
  create(){
    const tags = (this.f.value.tags || '').split(',').map((x: string) => x.trim()).filter(Boolean);
    this.api.createTour({ ...this.f.value, tags } as any).subscribe(()=>{
      this.f.reset({difficulty:'easy'}); this.load();
    });
  }
  open(t: Tour){ this.router.navigate(['/tours', t.id]); }
}
