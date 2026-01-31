import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminLayout } from './admin-layout/admin-layout';
import { Dashboard } from './dashboard/dashboard';
import { Professeurs } from './professeurs/professeurs';
import { Utilisateurs } from './utilisateurs/utilisateurs';
import { Supervision } from './supervision/supervision';
import { Categories } from './categories/categories';

const routes: Routes = [
  {
    path: '',
    component: AdminLayout,
    children: [
      { path: '', component: Dashboard, pathMatch: 'full' },
      { path: 'professeurs', component: Professeurs },
      { path: 'utilisateurs', component: Utilisateurs },
      { path: 'supervision', component: Supervision },
      { path: 'categories', component: Categories },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminSpaceRoutingModule {}
