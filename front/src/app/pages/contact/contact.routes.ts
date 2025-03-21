import { Routes } from '@angular/router';
import { ContactComponent } from './contact.component';

export const CONTACT_ROUTES: Routes = [
  {
    path: 'info',
    component: ContactComponent
  },
  { path: "**", redirectTo: "info" },
];
