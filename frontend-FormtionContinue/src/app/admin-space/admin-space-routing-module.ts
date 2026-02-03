import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminLayout } from './admin-layout/admin-layout';
import { Dashboard } from './dashboard/dashboard';
import { Professeurs } from './professeurs/professeurs';
import { Utilisateurs } from './utilisateurs/utilisateurs';
import { Supervision } from './supervision/supervision';
import { Categories } from './categories/categories';
import { Dashboard as ProfDashboardAdmin } from './professeurs/dashboard/dashboard';
import { Services } from './services/services';
import { Statuts } from './statuts/statuts';


const routes: Routes = [
  {
    path: '',
    component: AdminLayout,
    children: [
      { path: '', component: Dashboard, pathMatch: 'full' },
      { path: 'professeurs', component: Professeurs },
      { path: 'professeurs/:id/dashboard', component: ProfDashboardAdmin },
      { path: 'utilisateurs', component: Utilisateurs },
      { path: 'supervision', component: Supervision },
      { path: 'categories', component: Categories },
      { path: 'services', component: Services },
      { path: 'statuts', component: Statuts },

    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminSpaceRoutingModule {}
