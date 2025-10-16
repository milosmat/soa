import { Component, AfterViewInit } from '@angular/core';
import * as L from 'leaflet';
import { PositionService } from '../../services/position.service';

// Fix za Angular + Leaflet marker ikone
import 'leaflet/dist/images/marker-shadow.png';
import 'leaflet/dist/images/marker-icon.png';

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
  templateUrl: './simulator.component.html',
  styleUrls: ['./simulator.component.css']
})
export class SimulatorComponent implements AfterViewInit {
  map!: L.Map;
  marker?: L.Marker;

  constructor(private pos: PositionService) {}

  ngAfterViewInit() {
    this.map = L.map('sim-map').setView([44.8125, 20.4612], 13);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { attribution: '© OSM' }).addTo(this.map);

    // povuci poslednju poziciju korisnika
    this.pos.getMyPosition().subscribe(p => {
      if (p && p.lat && p.lng) {
        this.marker = L.marker([p.lat, p.lng]).addTo(this.map);
        this.map.setView([p.lat, p.lng], 13);
      }
    });

    this.map.on('click', (e:any) => {
      const lat = e.latlng.lat, lng = e.latlng.lng;
      if (this.marker) this.marker.remove();
      this.marker = L.marker([lat,lng]).addTo(this.map);

      this.pos.setPosition(lat, lng).subscribe();
    });
  }
}
