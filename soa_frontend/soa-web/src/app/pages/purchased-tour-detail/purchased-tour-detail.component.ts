import { Component, AfterViewInit, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ToursService } from '../../services/tour.service';
import * as L from 'leaflet';

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
  imports: [CommonModule],
  templateUrl: './purchased-tour-detail.component.html',
  styleUrls: ['./purchased-tour-detail.component.css']
})
export class PurchasedTourDetailComponent implements OnInit, AfterViewInit {
  id!: string;
  tour: any;
  map!: L.Map;

  constructor(private route: ActivatedRoute, private tours: ToursService) {}

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id')!;
    this.tours.getTour(this.id).subscribe(t => {
      this.tour = t;
      setTimeout(() => this.initMap(), 0);
    });
  }

  ngAfterViewInit() {}

  private initMap() {
    if (!this.tour) return;

    this.map = L.map('map').setView([44.8125, 20.4612], 13);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OSM'
    }).addTo(this.map);

    if (this.tour.points && this.tour.points.length > 0) {
      const coords = this.tour.points.map((p: any) => [p.lat, p.lng]);

      this.tour.points.forEach((p: any, i: number) => {
        L.marker([p.lat, p.lng])
          .addTo(this.map)
          .bindPopup(`${i + 1}. ${p.name || 'Checkpoint'}`);
      });

      L.polyline(coords, { color: 'blue' }).addTo(this.map);
      this.map.fitBounds(coords);
    }
  }
}
