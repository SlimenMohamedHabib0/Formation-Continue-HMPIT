import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserLayout } from './user-layout/user-layout';
import { Accueil } from './accueil/accueil';
import { Cours } from './cours/cours';
import { MesInscriptions } from './mes-inscriptions/mes-inscriptions';
import { MesResultats } from './mes-resultats/mes-resultats';
import { LectureCours } from './lecture-cours/lecture-cours';
import { PasserTest } from './passer-test/passer-test';

const routes: Routes = [
  {
    path: '',
    component: UserLayout,
    children: [
      { path: '', component: Accueil, pathMatch: 'full' },
      { path: 'cours', component: Cours },
      { path: 'mes-inscriptions', component: MesInscriptions },
      { path: 'mes-resultats', component: MesResultats },
      { path: 'lecture/:courseId', component: LectureCours },
      { path: 'passer-test/:courseId', component: PasserTest },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PublicRoutingModule {}
