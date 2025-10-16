import { Component, AfterViewInit, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import * as L from 'leaflet';
import { ToursService } from '../../services/tour.service';

const iconRetinaUrl = 'assets/leaflet/marker-icon-2x.png';
const iconUrl = 'assets/leaflet/marker-icon.png';
const shadowUrl = 'assets/leaflet/marker-shadow.png';

L.Marker.prototype.options.icon = L.icon({
  iconRetinaUrl,
  iconUrl,
  shadowUrl,
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  shadowSize: [41, 41]
});



@Component({
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './tour-detail.component.html',
  styleUrls: ['./tour-detail.component.css']
})
export class TourDetailComponent implements OnInit, AfterViewInit {
  id!: string;
  map!: L.Map;
  f!: FormGroup;

  constructor(private route: ActivatedRoute, private api: ToursService, private fb: FormBuilder) {
    this.f = this.fb.group({
      title: [''],
      description: [''],
      difficulty: ['easy'],
      price: [0],
      status: ['draft']
    });
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id')!;
    this.api.getTour(this.id).subscribe(t => this.f.patchValue(t));
  }

  ngAfterViewInit(): void {
    this.map = L.map('map').setView([44.8125, 20.4612], 13);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { attribution: '© OSM' }).addTo(this.map);
    this.map.on('click', (e: any) => {
      const lat = e.latlng.lat, lng = e.latlng.lng;
      L.marker([lat, lng]).addTo(this.map);
      this.api.addPoint(this.id, { lat, lng, name: `pt-${Date.now()}` }).subscribe();
    });
  }

  save() {
    this.api.updateTour(this.id, this.f.value).subscribe(() => {
      alert('Tour updated!');
    });
  }
}
