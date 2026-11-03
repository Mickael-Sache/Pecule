import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth-guard';

export const routes: Routes = [
  {
    path: '',
    title: 'Accueil',
    loadComponent: () => import('./features/home/home').then((m) => m.Home),
  },
  {
    path: 'login',
    title: 'Connexion',
    loadComponent: () => import('./features/login/login').then((m) => m.Login),
  },
  {
    path: 'register',
    title: 'Créer un compte',
    loadComponent: () => import('./features/register/register').then((m) => m.Register),
  },
  {
    path: 'accounts',
    title: 'Mes comptes',
    canActivate: [authGuard],
    loadComponent: () => import('./features/accounts/accounts').then((m) => m.Accounts),
  },
  {
    path: 'settings',
    title: 'Paramètres du compte',
    canActivate: [authGuard],
    loadComponent: () => import('./features/settings/settings').then((m) => m.Settings),
  },
];