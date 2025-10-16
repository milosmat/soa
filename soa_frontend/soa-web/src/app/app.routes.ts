import { Routes } from '@angular/router';
import { canActivate } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'tours', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./pages/register/register.component').then(m => m.RegisterComponent) },
  { path: 'profile', loadComponent: () => import('./pages/profile/profile.component').then(m => m.ProfileComponent), canActivate: [canActivate] },
  { path: 'tours', loadComponent: () => import('./pages/tours/tours.component').then(m => m.ToursComponent), canActivate: [canActivate] },
  { path: 'tours/:id', loadComponent: () => import('./pages/tour-detail/tour-detail.component').then(m => m.TourDetailComponent), canActivate: [canActivate] },
  { path: 'simulator', loadComponent: () => import('./pages/simulator/simulator.component').then(m => m.SimulatorComponent), canActivate: [canActivate] },
  { path: 'cart', loadComponent: () => import('./pages/cart/cart.component').then(m => m.CartComponent) },
  {
    path: 'market',
    loadComponent: () => import('./pages/market/market.component').then(m => m.MarketComponent)
  },
  { path: 'purchases', loadComponent: () => import('./pages/purchases/purchases.component').then(m => m.PurchasesComponent), canActivate: [canActivate] },
  {
    path: 'purchases/:id',
    loadComponent: () => import('./pages/purchased-tour-detail/purchased-tour-detail.component')
      .then(m => m.PurchasedTourDetailComponent),
    canActivate: [canActivate]
  }

];
