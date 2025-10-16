export interface CreateTourReq {
  title: string;
  description: string;
  difficulty: string;
  tags: string[];
}

export interface TourPointReq {
  lat: number;
  lng: number;
  name: string;
  description?: string;
  imageUrl?: string;
}

export interface TourPoint {
  id: string;
  lat: number;
  lng: number;
  name: string;
  description?: string;
  imageUrl?: string;
}

export interface Tour {
  id: string;
  authorId: string;
  title: string;
  description: string;
  difficulty: string;
  tags: string[];
  price: number;
  status: string;
  lengthKm: number;
  points: TourPoint[];
}
