import { RouterOutletComponent } from '@abp/ng.core';
import { Routes } from '@angular/router';
import { OPACComponent } from './components/o-pAC.component';

export const oPACRoutes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: RouterOutletComponent,
    children: [
      {
        path: '',
        component: OPACComponent,
      },
    ],
  },
];
