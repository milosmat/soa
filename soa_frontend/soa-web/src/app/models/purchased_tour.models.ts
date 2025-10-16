import {TourPoint} from './tour_point.models';

export interface PurchasedTour {
  tourId: string;
  title: string;
  description: string;
  difficulty: string;
  price: number;
  lengthKm: number;
  points: TourPoint[];
}
